using System;
using JetBrains.Annotations;

namespace XyrusWorx.Runtime 
{
	[PublicAPI]
	public interface IMemoryBlock
	{
		IntPtr GetPointer();
		long Size { get; }
	}
}