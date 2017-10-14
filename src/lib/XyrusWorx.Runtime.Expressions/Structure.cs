using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace XyrusWorx.Runtime.Expressions 
{
	[PublicAPI]
	[DebuggerDisplay("struct {Name}")]
	public sealed class Structure : ComputationType
	{
		private readonly SymbolScope mScope;
		private Type mType;

		public Structure([NotNull] SymbolScope scope, [NotNull] Symbol name, params Declaration[] fields)
		{
			mScope = scope ?? throw new ArgumentNullException(nameof(scope));
			Name = name ?? throw new ArgumentNullException(nameof(name));
			
			Fields = fields ?? new Declaration[0];

			if (Fields.Length == 0)
			{
				throw new ArgumentException("Structures must contain at least one field.", nameof(fields));
			}
		}
		
		[NotNull]
		public Symbol Name { get; }

		[NotNull]
		public Declaration[] Fields { get; }

		public override Type CreateClrType()
		{
			if (mType != null)
			{
				return mType;
			}

			using (var builder = mScope.GetStructureBuilder().CreateBuffer(Name.Key))
			{
				foreach (var field in Fields)
				{
					var clrType = field.Type.CreateClrType();
					if (clrType == null)
					{
						throw new CompilerException($"Unable to map structure because the type \"{field.Type.Key}\" for field \"{field.Label.Key}\" is not supported.");
					}

					builder.Field(field.Label.Key, clrType);
				}

				using (var buffer = builder.Commit())
				{
					mType = buffer.GetClrTypeInfo().UnderlyingSystemType;
				}
			}

			return mType;
		}
		public override string Key => Name.Key;
		
		protected override object GetDefaultValue() => Activator.CreateInstance(CreateClrType());
		protected override Result<object> Construct(IEnumerable<object> values)
		{
			var structureType = CreateClrType();
			var structureSize = Marshal.SizeOf(structureType);
			var instance = GetDefaultValue();
			
			var valueArray = values.ToArray();
			if (valueArray.Length > Fields.Length)
			{
				return Result.CreateError<Result<object>>($"Expected {Fields.Length} values, received {valueArray.Length}");
			}
			
			using (var memory = new UnmanagedBlock(structureSize))
			{
				Marshal.StructureToPtr(instance, memory, true);

				var offset = 0;
				var index = 0;
				
				foreach (var field in Fields)
				{
					var fieldType = field.Type.CreateClrType();
					var fieldSize = Marshal.SizeOf(fieldType);

					object fieldValue;
					try
					{
						fieldValue = System.Convert.ChangeType(valueArray[index], fieldType, CultureInfo.InvariantCulture);
					}
					catch (Exception)
					{
						return Result.CreateError($"Element \"{index + 1}\" failed to convert from \"{valueArray[index].GetType()}\" to \"{field.Type.Key}\".");
					}
					
					Marshal.StructureToPtr(fieldValue, memory.Pointer + offset, true);
					
					offset += fieldSize;
					index++;
				}

				Marshal.PtrToStructure(memory, instance);
			}

			return instance;
		}
		protected override IEnumerable<object> Deconstruct(object value)
		{
			var structureType = CreateClrType();
			var structureSize = Marshal.SizeOf(structureType);

			using (var memory = new UnmanagedBlock(structureSize))
			{
				Marshal.StructureToPtr(value, memory, true);

				var offset = 0;
				
				foreach (var field in Fields)
				{
					var fieldType = field.Type.CreateClrType();
					var fieldSize = Marshal.SizeOf(fieldType);
					var fieldInstance = Activator.CreateInstance(fieldType);
					
					Marshal.PtrToStructure(memory.Pointer + offset, fieldInstance);
					
					yield return fieldInstance;
					offset += fieldSize;
				}
			}
		}
	}
}