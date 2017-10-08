using System;
using System.Diagnostics;
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
	}
}