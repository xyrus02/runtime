using System;
using JetBrains.Annotations;

namespace XyrusWorx.Runtime.IO
{
	[PublicAPI]
	public interface IUnmanagedBuffer : IDisposable
	{
		IntPtr Data { get; }
		long SizeInBytes { get; }
	}
}