using System;
using JetBrains.Annotations;

namespace XyrusWorx.Runtime 
{
	[PublicAPI]
	public sealed class WriteOnlyMemoryWindow : IWritableMemory
	{
		public WriteOnlyMemoryWindow([NotNull] IWritableMemory memoryBlock, int offset, long size)
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
		}

		IntPtr IMemoryBlock.GetPointer() => Pointer;

		public IntPtr Pointer { get; }
		public long Size { get; }

		public void Write(IntPtr source, int writeOffset, long bytesToWrite) => UnmanagedBlock.Copy(source, Pointer, 0, writeOffset, bytesToWrite);
	}
}