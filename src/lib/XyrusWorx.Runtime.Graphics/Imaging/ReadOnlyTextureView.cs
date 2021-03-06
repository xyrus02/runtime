using System;
using JetBrains.Annotations;

namespace XyrusWorx.Runtime.Imaging 
{
	[PublicAPI]
	public class ReadOnlyTextureView : IReadableTexture, IView
	{
		private readonly IReadableMemory mMemory;
		private readonly TextureFormat mFormat;
		
		private readonly int mWidth;
		private readonly int mHeight;
		private readonly int mStride;

		public ReadOnlyTextureView([NotNull] IReadableMemory memory, int stride, TextureFormat format)
		{

			if (stride <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(stride));
			}
			
			mMemory = memory ?? throw new ArgumentNullException(nameof(memory));
			mFormat = format;

			mStride = stride;
			mWidth = stride >> 2;
			mHeight = (int)(memory.Size / stride);
		}

		public IMemoryBlock RawMemory => mMemory;
		public TextureFormat Format => mFormat;
		
		public int Width => mWidth;
		public int Stride => mStride;
		public int Height => mHeight;

		public unsafe uint this[int address]
		{
			get => *((uint*)(void*)(mMemory.GetPointer() + address));
		}
		public Vector4<byte> this[Int2 xy]
		{
			get => this[xy.x, xy.y];
		} 
		public unsafe Vector4<byte> this[int x, int y]
		{
			get
			{
				var offset = y * mStride + (x << 2);
				var pPixel = (uint*)(void*)(mMemory.GetPointer() + offset);
				
				return mFormat.Unpack(*pPixel);
			}
		}

		IReadableMemory IReadableTexture.RawMemory => mMemory;
		IntPtr IReadableTexture.Pointer => throw new NotSupportedException("Direct memory access is not allowed for read-only blocks.");

		public void Read(IntPtr target, int readOffset, long bytesToRead) => UnmanagedBlock.Copy(mMemory.GetPointer(), target, readOffset, 0, bytesToRead);
	}
}