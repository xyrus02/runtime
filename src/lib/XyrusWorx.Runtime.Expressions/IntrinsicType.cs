using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using XyrusWorx.Collections;

namespace XyrusWorx.Runtime.Expressions 
{
	[PublicAPI]
	[DebuggerDisplay("{Primitive}<{Affinity}>")]
	public sealed class IntrinsicType : ComputationType
	{
		private object mDefaultValue;

		public IntrinsicType(Primitive primitive, Affinity affinity = null)
		{
			if (primitive == Primitive.Undefined)
			{
				throw new ArgumentException("Intrinsic types can't use the \"undefined\" primitive.");
			}

			var result = CreateClrType();
			if (result == null)
			{
				throw new NotSupportedException($"The primitive type \"{Primitive}\" is not supported.");
			}
			
			Primitive = primitive;
			Affinity = affinity ?? Affinity.Single();
			Key = CreateKey();
		}
		
		public Primitive Primitive { get; }
		
		[NotNull]
		public Affinity Affinity { get; }
		
		[Pure]
		public override Type CreateClrType()
		{
			switch (Primitive)
			{
				case Primitive.Float:
					return Affinity.ExpandType(typeof(float));
				case Primitive.Integer:
					return Affinity.ExpandType(typeof(int));
				case Primitive.UnsignedInteger:
					return Affinity.ExpandType(typeof(uint));
				case Primitive.Boolean:
					return Affinity.ExpandType(typeof(bool));
				default:
					return null;
			}
		}
		public override string Key { get; }
		
		protected override object GetDefaultValue()
		{
			if (mDefaultValue != null)
			{
				return mDefaultValue;
			}
			
			var expandedType = CreateClrType();
			var instance = Activator.CreateInstance(expandedType);

			return mDefaultValue = instance;
		}
		protected override Result<object> Construct(IEnumerable<object> values)
		{
			var expectedCount = Affinity.Count;
			var clrType = CreateClrType();

			if (clrType == null)
			{
				return Result.CreateError($"Can't assign anything to a \"{Primitive}\" primitive.");
			}

			var index = 0;
			var resultObject = GetDefaultValue();
			
			Action<int, object> setValue = (i, o) => { };

			if (Affinity.Size.x > 1)
			{
				setValue = (i, o) => resultObject = resultObject.CastTo<IMatrixCellWriter>()?.Set(i % Affinity.Size.x, i / Affinity.Size.x, o);
			}
			else if (Affinity.Size.y > 1)
			{
				setValue = (i, o) => resultObject = resultObject.CastTo<IVectorRowWriter>()?.Set(i % Affinity.Size.y, o);
			}

			var valueArray = values.AsArray();
			
			foreach (var value in valueArray)
			{
				var inputValue = value;
				if (inputValue == null)
				{
					inputValue = Activator.CreateInstance(clrType);
				}
				
				try
				{
					setValue(index, System.Convert.ChangeType(inputValue, clrType, CultureInfo.InvariantCulture));
				}
				catch (Exception)
				{
					return Result.CreateError($"Element \"{index + 1}\" failed to convert from \"{value.GetType()}\" to \"{Primitive}\".");
				}

				index++;
				
				if (index >= expectedCount)
				{
					break;
				}
			}

			if (index < valueArray.Length)
			{
				return Result.CreateError<Result<object>>($"Expected {expectedCount} values, received {valueArray.Length}");
			}

			return resultObject;
		}
		protected override IEnumerable<object> Deconstruct(object value)
		{
			if (value is IMatrix matrix)
			{
				return matrix.GetRows().SelectMany(x => x.GetComponents()).ToArray();
			}
			
			if (value is IVector vector)
			{
				return vector.GetComponents();
			}

			if (value is IEnumerable enumerable)
			{
				return enumerable.OfType<object>().SelectMany(Deconstruct).ToArray();
			}

			return new[] { value };
		}

		[Pure]
		private string CreateKey()
		{
			var primitiveTypeFields =
				from field in typeof(Primitive).GetFields(BindingFlags.Static)
				let attribute = CustomAttributeExtensions.GetCustomAttribute<RuntimeKeyAttribute>(field)
				where attribute != null
				select new
				{
					Field = field,
					Attribute = attribute
				};

			var primitiveDictionary = primitiveTypeFields.ToDictionary(x => (Primitive)x.Field.GetValue(null), x => x.Attribute.Key);
			var currentKey = primitiveDictionary[Primitive];
			
			if (Affinity.Size.x > 1)
			{
				return $"{currentKey}{Affinity.Size.x}x{Affinity.Size.y}";
			}
			
			if (Affinity.Size.y > 1)
			{
				return $"{currentKey}{Affinity.Size.y}";
			}

			return currentKey;
		}
	}
}