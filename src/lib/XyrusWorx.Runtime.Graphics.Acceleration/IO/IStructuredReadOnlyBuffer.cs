using System;
using JetBrains.Annotations;

namespace XyrusWorx.Runtime.IO
{
	[PublicAPI]
	public interface IStructuredReadOnlyBuffer : IStructuredBuffer
	{
		void Read(IntPtr buffer, int index, int count);
		T Read<T>(int index = 0) where T : struct;
	}
}