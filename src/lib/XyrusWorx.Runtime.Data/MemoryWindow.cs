using System;
using JetBrains.Annotations;

namespace XyrusWorx.Runtime 
{
	[PublicAPI]
	public sealed class MemoryWindow : IReadableMemory, IWritableMemory
	{
		private readonly bool mReadable;
		private readonly bool mWritable;

		public MemoryWindow([NotNull] IMemoryBlock memoryBlock, int offset, long size)
		{
			if (memoryBlock == null)
			{
				throw new ArgumentNullException(nameof(memoryBlock));
			}

			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(offset));
			}

			if (size > memoryBlock.Size - offset)
			{
				throw new ArgumentOutOfRangeException(nameof(size), "The window size is larger than the size of the target block.");
			}
			
			Pointer = memoryBlock.GetPointer() + offset;
			Size = size;

			mReadable = memoryBlock is IReadableMemory;
			mWritable = memoryBlock is IWritableMemory;

			if (!mReadable && !mWritable)
			{
				// neither readable nor writable!? must be some unknown memory restriction...
				mReadable = mWritable = true;
			}
		}

		IntPtr IMemoryBlock.GetPointer() => Pointer;

		public IntPtr Pointer { get; }
		public long Size { get; }

		public void Read(IntPtr target, int readOffset, long bytesToRead)
		{
			if (!mReadable)
			{
				throw new AccessViolationException("The underlying memory block does not allow reading.");
			}
			
			UnmanagedBlock.Copy(Pointer, target, readOffset, 0, bytesToRead);
		}
		public void Write(IntPtr source, int writeOffset, long bytesToWrite)
		{
			if (!mReadable)
			{
				throw new AccessViolationException("The underlying memory block does not allow writing.");
			}
			
			UnmanagedBlock.Copy(source, Pointer, 0, writeOffset, bytesToWrite);
		}
	}
}