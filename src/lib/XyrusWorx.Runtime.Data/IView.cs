using JetBrains.Annotations;

namespace XyrusWorx.Runtime 
{
	[PublicAPI]
	public interface IView
	{
		[NotNull]
		IMemoryBlock RawMemory { get; }
	}
}