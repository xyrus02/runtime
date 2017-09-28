using JetBrains.Annotations;

namespace XyrusWorx.Runtime.Imaging
{
	[PublicAPI]
	public interface ITexture
	{
		TextureFormat Format { get; }
		int Stride { get; }
		int Height { get; }
	}

}
