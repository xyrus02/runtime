using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;

namespace XyrusWorx.Runtime.Expressions 
{
	[PublicAPI]
	[DebuggerDisplay("{Primitive}<{Affinity}>")]
	public sealed class IntrinsicType : ComputationType
	{
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

		[Pure]
		private string CreateKey()
		{
			var primitiveTypeFields =
				from field in typeof(Primitive).GetFields(BindingFlags.Static)
				let attribute = CustomAttributeExtensions.GetCustomAttribute<RuntimeKeyAttribute>((MemberInfo)field)
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