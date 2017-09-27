using System;
using JetBrains.Annotations;

namespace XyrusWorx.Runtime 
{
	[PublicAPI]
	public interface IUnmanagedMemory : IDisposable
	{
		IntPtr Pointer { get; }
	}
}