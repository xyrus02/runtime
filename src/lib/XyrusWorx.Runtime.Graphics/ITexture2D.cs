using System;
using JetBrains.Annotations;
using XyrusWorx.Runtime.IO;

namespace XyrusWorx.Runtime.Graphics
{
	[PublicAPI]
	public interface ITexture2D : IStructuredBuffer
	{
		int Stride { get; }
		int Width { get; }
		int Height { get; }

		void Write(byte[] data);
		void Write(byte[] data, int offset, int count);
		void Write(IntPtr ptr);
	}
}