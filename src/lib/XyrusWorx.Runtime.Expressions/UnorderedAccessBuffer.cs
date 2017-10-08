using System;
using System.Diagnostics;
using JetBrains.Annotations;

namespace XyrusWorx.Runtime.Expressions 
{
	[PublicAPI]
	[DebuggerDisplay("RWStructuredBuffer<{ElementType}> {Label} : {Key}")]
	public sealed class UnorderedAccessBuffer : Register
	{
		public UnorderedAccessBuffer([NotNull] Symbol label, [NotNull] ComputationType elementType, int index) : base(label, index)
		{
			if (index >= 8)
			{
				throw new ArgumentOutOfRangeException(nameof(index));
			}
			
			ElementType = elementType ?? throw new ArgumentNullException(nameof(elementType));
		}
		
		[NotNull]
		public ComputationType ElementType { get; }
		
		protected override char Group => 'u';
	}
}