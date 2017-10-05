using System;
using JetBrains.Annotations;

namespace XyrusWorx.Runtime.Imaging 
{
	[PublicAPI]
	public class WriteOnlyTextureView : IWritableTexture, IView
	{
		private readonly IWritableMemory mMemory;
		private readonly TextureFormat mFormat;
		
		private readonly int mWidth;
		private readonly int mHeight;
		private readonly int mStride;

		public WriteOnlyTextureView([NotNull] IWritableMemory memory, int stride, TextureFormat format)
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
		
		public unsafe uint this[int address]
		{
			set => *((uint*)(void*)(mMemory.GetPointer() + address)) = value;
		}
		public Vector4<byte> this[Int2 xy]
		{
			set => this[xy.x, xy.y] = value;
		} 
		public unsafe Vector4<byte> this[int x, int y]
		{
			set
			{
				var offset = y * mStride + (x << 2);
				var pPixel = (uint*)(void*)(mMemory.GetPointer() + offset);

				*pPixel = mFormat.Pack(value);
			}
		}

		public void Write(IntPtr source, int writeOffset, long bytesToWrite) => UnmanagedBlock.Copy(source, mMemory.GetPointer(), 0, writeOffset, bytesToWrite);
	}
}