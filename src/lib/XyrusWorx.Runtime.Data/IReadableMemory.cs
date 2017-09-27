using System;
using JetBrains.Annotations;

namespace XyrusWorx.Runtime 
{
	[PublicAPI]
	public interface IReadableMemory : IMemoryBlock
	{
		void Read(IntPtr target, int readOffset, long bytesToRead);
	}
}