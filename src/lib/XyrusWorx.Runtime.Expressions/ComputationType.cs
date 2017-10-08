using System;
using System.Diagnostics;
using JetBrains.Annotations;

namespace XyrusWorx.Runtime.Expressions 
{
	[PublicAPI]
	[DebuggerDisplay("{Key}")]
	public abstract class ComputationType
	{
		internal ComputationType() {}

		[CanBeNull]
		public abstract Type CreateClrType();
		
		[NotNull]
		public abstract string Key { get; }
		public override string ToString() => Key;
	}
}