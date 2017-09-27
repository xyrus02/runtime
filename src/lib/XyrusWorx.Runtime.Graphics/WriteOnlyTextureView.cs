using System;
using JetBrains.Annotations;

namespace XyrusWorx.Runtime.Graphics 
{
	[PublicAPI]
	public class WriteOnlyTextureView : IWritableTexture
	{
		private readonly IWritableMemory mMemory;
		private readonly TextureFormat mFormat;
		
		private readonly bool mReadable;
		private readonly bool mWritable;
		
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

		IntPtr IMemoryBlock.GetPointer() => mMemory.GetPointer();
		long IMemoryBlock.Size => mStride * mHeight;

		public void Write(IntPtr source, int writeOffset, long bytesToWrite) => UnmanagedBlock.Copy(source, mMemory.GetPointer(), 0, writeOffset, bytesToWrite);
	}
}