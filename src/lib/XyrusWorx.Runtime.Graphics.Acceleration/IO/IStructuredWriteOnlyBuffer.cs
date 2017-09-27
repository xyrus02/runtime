using System;
using JetBrains.Annotations;

namespace XyrusWorx.Runtime.IO
{
	[PublicAPI]
	public interface IStructuredWriteOnlyBuffer : IStructuredBuffer
	{
		void Write(IntPtr rawData, int index, int count);
		void Write<T>(T data, int index = 0) where T : struct;
	}
}