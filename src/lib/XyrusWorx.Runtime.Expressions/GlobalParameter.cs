using System;
using JetBrains.Annotations;

namespace XyrusWorx.Runtime.Expressions 
{
	[PublicAPI]
	public sealed class GlobalParameter
	{
		public GlobalParameter([NotNull] Declaration parameterDeclaration, [NotNull] Register register)
		{
			Declaration = parameterDeclaration ?? throw new ArgumentNullException(nameof(parameterDeclaration));
			Register = register ?? throw new ArgumentNullException(nameof(register));
		}
		public GlobalParameter([NotNull] Declaration parameterDeclaration, [NotNull] Literal parameterValue)
		{
			Declaration = parameterDeclaration ?? throw new ArgumentNullException(nameof(parameterDeclaration));
			InitialValue = parameterValue ?? throw new ArgumentNullException(nameof(parameterValue));
		}
		
		[NotNull]
		public Declaration Declaration { get; }
		
		[CanBeNull]
		public Literal InitialValue { get; }
		
		[CanBeNull]
		public Register Register { get; }
	}
}