using JetBrains.Annotations;

namespace XyrusWorx.Runtime 
{
	[PublicAPI]
	public interface IUnmanagedBlock : IUnmanagedMemory
	{
		long Size { get; }
	}
}