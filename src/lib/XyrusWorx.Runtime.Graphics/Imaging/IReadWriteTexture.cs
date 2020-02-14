using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace XyrusWorx.Runtime.Imaging 
{
	[PublicAPI]
	[SuppressMessage("ReSharper", "PossibleInterfaceMemberAmbiguity")]
	public interface IReadWriteTexture : IReadableTexture, IWritableTexture
	{
		new uint this[int address] { get; set; }
		new Vector4<byte> this[Int2 xy] { get; set; }
		new Vector4<byte> this[int x, int y] { get; set; }
	}
}