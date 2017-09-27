using System;
using System.IO;
using JetBrains.Annotations;

namespace XyrusWorx.Runtime
{
	[PublicAPI]
	public class UnmanagedStream : Stream
	{
		private readonly IMemoryBlock mMemory;
		private long mCurrentOffset;

		public UnmanagedStream([NotNull] IReadableMemory memory)
		{
			if (memory == null)
			{
				throw new ArgumentNullException(nameof(memory));
			}
			
			mMemory = memory;
			CanRead = true;
			CanSeek = true;
			CanWrite = false;
		}
		public UnmanagedStream([NotNull] IWritableMemory memory)
		{
			if (memory == null)
			{
				throw new ArgumentNullException(nameof(memory));
			}
			
			mMemory = memory;
			CanRead = false;
			CanSeek = false;
			CanWrite = true;
		}
		public UnmanagedStream([NotNull] IReadWriteMemory memory)
		{
			if (memory == null)
			{
				throw new ArgumentNullException(nameof(memory));
			}
			
			mMemory = memory;
			CanRead = true;
			CanSeek = true;
			CanWrite = true;
		}
		
		public override long Seek(long offset, SeekOrigin origin)
		{
			switch (origin)
			{
				case SeekOrigin.Begin:
					mCurrentOffset = Math.Min(offset, mMemory.Size);
					break;
				case SeekOrigin.Current:
					mCurrentOffset = Math.Min(offset + mCurrentOffset, mMemory.Size);
					break;
				case SeekOrigin.End:
					mCurrentOffset = Math.Max(0, mMemory.Size - offset);
					break;
			}

			return mCurrentOffset;
		}
		public override void SetLength(long value)
		{
			throw new NotSupportedException("This method is not supported.");
		}
		public override void Flush()
		{
		}

		public override unsafe int Read(byte[] buffer, int offset, int count)
		{
			fixed (void* pBuffer = buffer)
			{
				var bufferPointer = new IntPtr(pBuffer);
				var sourcePointer = mMemory.GetPointer();

				var length = Math.Min(count, mMemory.Size - mCurrentOffset);
				
				UnmanagedBlock.Copy(sourcePointer, bufferPointer, (int)mCurrentOffset, offset, length);
				mCurrentOffset += length;

				return (int)length;
			}
		}
		public override unsafe void Write(byte[] buffer, int offset, int count)
		{
			fixed (void* pBuffer = buffer)
			{
				var bufferPointer = new IntPtr(pBuffer);
				var targetPointer = mMemory.GetPointer();

				var length = Math.Min(count, mMemory.Size - mCurrentOffset);
				
				UnmanagedBlock.Copy(bufferPointer, targetPointer, offset, (int)mCurrentOffset, length);
				mCurrentOffset += length;
			}
		}

		public override bool CanRead { get; }
		public override bool CanSeek { get; }
		public override bool CanWrite { get; }
		
		public override long Length => mMemory.Size;
		public override long Position
		{
			get => mCurrentOffset;
			set => Seek(value, SeekOrigin.Begin);
		}
	}
}
