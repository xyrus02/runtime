using System;
using System.Diagnostics;
using JetBrains.Annotations;

namespace XyrusWorx.Runtime.Expressions 
{
	[PublicAPI]
	[DebuggerDisplay("{Label} : {Key}")]
	public abstract class Register
	{
		internal Register([NotNull] Symbol label, int index)
		{
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(index));
			}

			Label = label ?? throw new ArgumentNullException(nameof(label));
			Index = index;
		}

		[NotNull]
		public Symbol Label { get; }
		public int Index { get; }

		public string Key => $"{Group}{Index}";
		protected abstract char Group { get; }
	}
}