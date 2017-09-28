using System;
using JetBrains.Annotations;

namespace XyrusWorx.Runtime 
{
	[PublicAPI]
	public interface IReadable
	{
		void Read(IntPtr target, int readOffset, long bytesToRead);
	}
}