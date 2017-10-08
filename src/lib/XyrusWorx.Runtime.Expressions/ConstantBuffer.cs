using System;
using System.Diagnostics;
using JetBrains.Annotations;

namespace XyrusWorx.Runtime.Expressions 
{
	[PublicAPI]
	[DebuggerDisplay("cbuffer {Label} : {Key}")]
	public sealed class ConstantBuffer : Register
	{
		public ConstantBuffer([NotNull] Symbol label, int index, params Declaration[] elements) : base(label, index)
		{
			if (index >= 15)
			{
				throw new ArgumentOutOfRangeException(nameof(index));
			}
			
			Elements = elements ?? new Declaration[0];

			if (Elements.Length == 0)
			{
				throw new ArgumentException("Constant buffers must contain at least one element.", nameof(elements));
			}
		}

		[NotNull]
		public Declaration[] Elements { get; }

		protected override char Group => 'b';
	}
}