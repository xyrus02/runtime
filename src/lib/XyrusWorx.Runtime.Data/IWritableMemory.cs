using System;
using JetBrains.Annotations;

namespace XyrusWorx.Runtime 
{
	[PublicAPI]
	public interface IWritableMemory : IMemoryBlock
	{
		void Write(IntPtr source, int writeOffset, long bytesToWrite);
	}
}