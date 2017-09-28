using System;
using JetBrains.Annotations;

namespace XyrusWorx.Runtime 
{
	[PublicAPI]
	public interface IWritable
	{
		void Write(IntPtr source, int writeOffset, long bytesToWrite);
	}
}