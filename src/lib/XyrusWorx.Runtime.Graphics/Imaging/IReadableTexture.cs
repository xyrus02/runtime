using JetBrains.Annotations;

namespace XyrusWorx.Runtime.Graphics.Imaging 
{
	[PublicAPI]
	public interface IReadableTexture : IReadableMemory, ITexture
	{
		Vector4<byte> this[Int2 xy] { get; }
		Vector4<byte> this[int x, int y] { get; }
	}
}