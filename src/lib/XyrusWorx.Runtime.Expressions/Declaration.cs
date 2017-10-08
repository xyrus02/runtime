using System;
using System.Diagnostics;
using JetBrains.Annotations;

namespace XyrusWorx.Runtime.Expressions
{
	[PublicAPI]
	[DebuggerDisplay("{Type} {Label}")]
	public sealed class Declaration
	{
		public Declaration([NotNull] Symbol label, [NotNull] ComputationType type)
		{
			if (label == null)
			{
				throw new ArgumentNullException(nameof(label));
			}

			if (type == null)
			{
				throw new ArgumentNullException(nameof(type));
			}

			Label = label;
			Type = type;
		}

		[NotNull]
		public Symbol Label { get; }
		
		[NotNull]
		public ComputationType Type { get; }
	}

}