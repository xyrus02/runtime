using JetBrains.Annotations;

namespace XyrusWorx.Runtime.Imaging 
{
	[PublicAPI]
	public interface IReadableTexture : IReadable, ITexture
	{
		Vector4<byte> this[Int2 xy] { get; }
		Vector4<byte> this[int x, int y] { get; }
	}
}