using JetBrains.Annotations;

namespace XyrusWorx.Runtime 
{
	[PublicAPI]
	public interface IUnmanagedArray : IUnmanagedMemory
	{
		int Length { get; }
		int ElementSize { get; }
	}
}