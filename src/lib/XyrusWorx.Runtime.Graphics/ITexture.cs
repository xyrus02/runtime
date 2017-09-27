using JetBrains.Annotations;

namespace XyrusWorx.Runtime.Graphics
{
	[PublicAPI]
	public interface ITexture : IMemoryBlock
	{
		int Stride { get; }
		int Height { get; }
	}

}
