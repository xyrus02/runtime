﻿using JetBrains.Annotations;

namespace XyrusWorx.Runtime.Imaging 
{
	[PublicAPI]
	public interface IWritableTexture : IWritable, ITexture
	{
		Vector4<byte> this[Int2 xy] { set; }
		Vector4<byte> this[int x, int y] { set; }
	}
}