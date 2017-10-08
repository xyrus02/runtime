using System;
using System.Diagnostics;
using JetBrains.Annotations;

namespace XyrusWorx.Runtime.Expressions 
{
	[PublicAPI]
	[DebuggerDisplay("StructuredBuffer<{ElementType}> {Label} : {Key}")]
	public sealed class InputResourceBuffer : Register
	{
		public InputResourceBuffer([NotNull] Symbol label, [NotNull] ComputationType elementType, int index) : base(label, index)
		{
			if (index >= 128)
			{
				throw new ArgumentOutOfRangeException(nameof(index));
			}
			
			ElementType = elementType ?? throw new ArgumentNullException(nameof(elementType));
		}
		
		[NotNull]
		public ComputationType ElementType { get; }
		
		protected override char Group => 't';
	}
}