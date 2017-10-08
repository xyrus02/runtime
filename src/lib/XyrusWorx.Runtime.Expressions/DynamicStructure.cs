using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace XyrusWorx.Runtime.Expressions 
{
	[PublicAPI]
	public class DynamicStructure : Resource, IDynamicBufferBuilder, IDynamicBuffer
	{
		private readonly DynamicStructureBuilder mContext;
		private readonly string mTypeName;
		
		private readonly List<StringKey> mFieldNames;
		private readonly Dictionary<StringKey, FieldBuilder> mFieldBuilders;
		private readonly Dictionary<StringKey, FieldInfo> mFields;

		private TypeInfo mType;
		private TypeBuilder mTypeBuilder;
		private UnmanagedBlock mBuffer;
		
		private object mData;
		private bool mIsDisposed;

		private DynamicStructure([NotNull] DynamicStructureBuilder context, [NotNull] string typeName)
		{
			if (context == null)
			{
				throw new ArgumentNullException(nameof(context));
			}
			
			if (typeName.NormalizeNull() == null)
			{
				throw new ArgumentNullException(nameof(typeName));
			}

			if (context.ContainsBuilder(typeName.Trim()))
			{
				throw new ArgumentException(nameof(typeName), $"The type \"{typeName}\" was already defined in the given context.");
			}

			var module = context.GetModuleBuilder();
			if (module == null)
			{
				throw new ObjectDisposedException(nameof(DynamicStructureBuilder), $"The {nameof(DynamicStructureBuilder)} was already disposed.");
			}

			mTypeName = typeName;
			mTypeBuilder = module.DefineType(typeName.Trim(),
				TypeAttributes.Public |
				TypeAttributes.Sealed |
				TypeAttributes.ExplicitLayout |
				TypeAttributes.Serializable |
				TypeAttributes.AnsiClass,
				typeof(ValueType));
			
			mFieldNames = new List<StringKey>();
			mFields = new Dictionary<StringKey, FieldInfo>();
			mFieldBuilders = new Dictionary<StringKey, FieldBuilder>();
			
			var structLayoutAttributeConstructor = typeof(StructLayoutAttribute).GetConstructor(new[] { typeof(LayoutKind) }).AssertNotNull();
			var structLayoutAttributeBuilder = new CustomAttributeBuilder(structLayoutAttributeConstructor, new object[]{LayoutKind.Sequential});
			
			mContext = context;
			mTypeName = typeName;
			
			mTypeBuilder.SetCustomAttribute(structLayoutAttributeBuilder);
			mContext.AddBuilder(mTypeName, this);
		}

		[NotNull]
		public static IDynamicBufferBuilder Create([NotNull] DynamicStructureBuilder context, [NotNull] string typeName) => new DynamicStructure(context, typeName);

		IDynamicBufferBuilder IDynamicBufferAppender.Field(string fieldName, Type fieldType)
		{
			if (fieldName.NormalizeNull() == null)
			{
				throw new ArgumentNullException(nameof(fieldName));
			}
			
			if (fieldType == null)
			{
				throw new ArgumentNullException(nameof(fieldType));
			}

			if (!fieldType.IsValueType)
			{
				throw new ArgumentException("Only value types are allowed.", nameof(fieldType));
			}
			
			if (mTypeBuilder == null)
			{
				throw new ObjectDisposedException(nameof(DynamicStructure), $"The {nameof(DynamicStructure)} was already disposed.");
			}
			
			if (mType != null)
			{
				throw new InvalidOperationException("Altering the buffer layout is not allowed after the buffer has been built.");
			}

			var fieldBuilder = mTypeBuilder.DefineField(fieldName, fieldType, FieldAttributes.Public);
			
			mFieldNames.Add(fieldName);
			mFieldBuilders.Add(fieldName, fieldBuilder);

			var offset = mFieldBuilders.Sum(x => Marshal.SizeOf((Type)x.Value.FieldType));
			fieldBuilder.SetOffset(offset);

			return this;
		}
		
		IDynamicBuffer IDynamicBufferBuilder.Commit()
		{
			mType = mTypeBuilder.CreateTypeInfo();
			mData = Activator.CreateInstance(mType);
			mBuffer = new UnmanagedBlock(Marshal.SizeOf(mType.UnderlyingSystemType));

			foreach (var fieldKey in mFieldNames)
			{
				var field = mType.GetField(fieldKey).AssertNotNull();
				mFields.Add(fieldKey, field);
			}

			UpdateBuffer();
			return this;
		}
		IDynamicBuffer IDynamicBuffer.SetValue(string fieldName, object value)
		{
			if (fieldName == null)
			{
				throw new ArgumentNullException(nameof(fieldName));
			}
			
			if (mTypeBuilder == null)
			{
				throw new ObjectDisposedException(nameof(DynamicStructure), $"The {nameof(DynamicStructure)} was already disposed.");
			}

			if (mType == null)
			{
				throw new InvalidOperationException("Accessing the buffer data is not allowed before the buffer has been built.");
			}

			if (!mFields.ContainsKey(fieldName))
			{
				throw new KeyNotFoundException($"The field \"{fieldName}\" could not be found.");
			}
			
			mFields[fieldName].SetValue(mData, value ?? Activator.CreateInstance(mFieldBuilders[fieldName].FieldType));

			UpdateBuffer();
			return this;
		}

		IWritable IDynamicBuffer.GetUnmanagedBuffer()
		{
			if (mTypeBuilder == null)
			{
				throw new ObjectDisposedException(nameof(DynamicStructure), $"The {nameof(DynamicStructure)} was already disposed.");
			}
			
			if (mBuffer == null)
			{
				throw new InvalidOperationException("Accessing the buffer is not allowed before the buffer has been built.");
			}
			
			return mBuffer;
		}
		TypeInfo IDynamicBuffer.GetClrTypeInfo()
		{
			if (mTypeBuilder == null)
			{
				throw new ObjectDisposedException(nameof(DynamicStructure), $"The {nameof(DynamicStructure)} was already disposed.");
			}
			
			if (mType == null)
			{
				throw new InvalidOperationException("Accessing the CLR type is not allowed before the type has been built.");
			}
			
			return mType;
		}

		private void UpdateBuffer()
		{
			using (var temp = new UnmanagedBlock(Marshal.SizeOf(mType)))
			{
				Marshal.StructureToPtr(mData, temp, true);
				mBuffer.Write(temp, 0, temp.Size);
			}
		}
		
		protected override void DisposeOverride()
		{
			if (mIsDisposed)
			{
				return;
			}

			mFieldBuilders.Clear();
			mFieldNames.Clear();
			mFields.Clear();
			
			mBuffer?.Dispose();
			mContext.RemoveBuilder(mTypeName);

			mTypeBuilder = null;
			mBuffer = null;
			mType = null;
			mData = null;
			
			mIsDisposed = true;
		}
	}
}