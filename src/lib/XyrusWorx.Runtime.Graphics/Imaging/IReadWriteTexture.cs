using JetBrains.Annotations;

namespace XyrusWorx.Runtime.Graphics.Imaging 
{
	[PublicAPI]
	public interface IReadWriteTexture : IReadableTexture, IWritableTexture
	{
		new Vector4<byte> this[Int2 xy] { get; set; }
		new Vector4<byte> this[int x, int y] { get; set; }
	}
}