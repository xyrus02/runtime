using System;
using JetBrains.Annotations;

namespace XyrusWorx.Runtime.IO
{
	[PublicAPI]
	public abstract class BufferBase<T> : BufferBase where T: struct
	{
		public abstract T this[int offset] { get; set; }
	}

	[PublicAPI]
	public abstract class BufferBase : IDisposable
	{
		public abstract int Length { get; }
		public abstract void Dispose();
	}
}