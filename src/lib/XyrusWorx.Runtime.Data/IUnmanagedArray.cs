using JetBrains.Annotations;

namespace XyrusWorx.Runtime 
{
	[PublicAPI]
	public interface IUnmanagedArray : IMemoryBlock
	{
		int Length { get; }
		int ElementSize { get; }
	}
}