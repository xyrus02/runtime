using System;
using JetBrains.Annotations;

namespace XyrusWorx.Runtime.Graphics.Imaging 
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
			if (memory == null)
			{
				throw new ArgumentNullException(nameof(memory));
			}

			if (stride <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(stride));
			}
			
			mMemory = memory;
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

		IntPtr IMemoryBlock.GetPointer() => mMemory.GetPointer();
		long IMemoryBlock.Size => mStride * mHeight;

		public void Read(IntPtr target, int readOffset, long bytesToRead) => UnmanagedBlock.Copy(mMemory.GetPointer(), target, readOffset, 0, bytesToRead);
	}
}