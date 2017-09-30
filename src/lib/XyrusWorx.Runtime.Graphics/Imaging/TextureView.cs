using System;
using JetBrains.Annotations;

namespace XyrusWorx.Runtime.Imaging 
{
	[PublicAPI]
	public class TextureView : IReadWriteTexture, IView
	{
		private readonly IMemoryBlock mMemory;
		private readonly TextureFormat mFormat;
		
		private readonly bool mReadable;
		private readonly bool mWritable;
		
		private readonly int mWidth;
		private readonly int mHeight;
		private readonly int mStride;

		public TextureView([NotNull] IMemoryBlock memory, int stride, TextureFormat format)
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
			
			mReadable = memory is IReadableMemory;
			mWritable = memory is IWritableMemory;

			if (!mReadable && !mWritable)
			{
				// neither readable nor writable!? must be some unknown memory restriction...
				mReadable = mWritable = true;
			}
		}

		public IMemoryBlock RawMemory => mMemory;
		public TextureFormat Format => mFormat;
		
		public int Width => mWidth;
		public int Stride => mStride;
		public int Height => mHeight;
		
		public unsafe uint this[int address]
		{
			get => *((uint*)(void*)(mMemory.GetPointer() + address));
			set => *((uint*)(void*)(mMemory.GetPointer() + address)) = value;
		}
		public Vector4<byte> this[Int2 xy]
		{
			get => this[xy.x, xy.y];
			set => this[xy.x, xy.y] = value;
		} 
		public unsafe Vector4<byte> this[int x, int y]
		{
			get
			{
				var offset = y * mStride + (x << 2);
				var pPixel = (uint*)(void*)(mMemory.GetPointer() + offset);
				
				return mFormat.Unpack(*pPixel);
			}
			set
			{
				var offset = y * mStride + (x << 2);
				var pPixel = (uint*)(void*)(mMemory.GetPointer() + offset);

				*pPixel = mFormat.Pack(value);
			}
		}

		public void Read(IntPtr target, int readOffset, long bytesToRead)
		{
			if (!mReadable)
			{
				throw new AccessViolationException("The underlying memory block does not allow reading.");
			}
			
			UnmanagedBlock.Copy(mMemory.GetPointer(), target, readOffset, 0, bytesToRead);
		}
		public void Write(IntPtr source, int writeOffset, long bytesToWrite)
		{
			if (!mReadable)
			{
				throw new AccessViolationException("The underlying memory block does not allow writing.");
			}
			
			UnmanagedBlock.Copy(source, mMemory.GetPointer(), 0, writeOffset, bytesToWrite);
		}
	}
}