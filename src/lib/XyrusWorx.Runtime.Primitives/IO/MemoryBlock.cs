using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using XyrusWorx.Runtime.Native;

namespace XyrusWorx.Runtime.IO
{
	[PublicAPI]
	public class MemoryBlock : MemoryBlock<byte>
	{
		public MemoryBlock(int size) : base(size)
		{
		}
	}

	[PublicAPI]
	public class MemoryBlock<T> : HardwareResource, IDataStream<T>, IUnmanagedBuffer where T : struct
	{
		private int mCurrentOffset;
		private readonly int mSize;
		private IntPtr mPtr;

		public MemoryBlock(int size)
		{
			if (size <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(size));
			}

			mSize = size;
			mPtr = Marshal.AllocHGlobal(size);
		}

		public long Position => mCurrentOffset;
		public long Length => mSize;

		public long Seek(long offset, SeekOrigin origin)
		{
			switch (origin)
			{
				case SeekOrigin.Begin:
					mCurrentOffset = (int)offset;
					break;
				case SeekOrigin.Current:
					mCurrentOffset += (int)offset;
					break;
				case SeekOrigin.End:
					mCurrentOffset = mSize - (int)offset;
					break;
			}

			if (mCurrentOffset < 0)
			{
				mCurrentOffset = 0;
			}

			if (mCurrentOffset >= mSize)
			{
				return mSize - 1;
			}

			return mCurrentOffset;
		}
		public int Read(byte[] buffer, int offset, int count)
		{
			var length = Math.Min(count, mSize - mCurrentOffset);
			Marshal.Copy(mPtr + mCurrentOffset, buffer, offset, length);
			return length;
		}
		public void Write(byte[] buffer, int offset, int count)
		{
			var length = Math.Min(count, mSize - mCurrentOffset);
			Marshal.Copy(buffer, offset, mPtr + mCurrentOffset, length);
		}

		public IntPtr Data => mPtr;
		public long SizeInBytes => mSize;

		public T Read()
		{
			if (mCurrentOffset + Marshal.SizeOf<T>() > mSize)
			{
				return default(T);
			}

			var result = Marshal.PtrToStructure<T>(mPtr + mCurrentOffset);
			Seek(Marshal.SizeOf<T>(), SeekOrigin.Current);
			return result;
		}
		public T[] Read(int count)
		{
			var list = new List<T>();
			for (var i = 0; i < count; i++)
			{
				if (mCurrentOffset + Marshal.SizeOf<T>() > mSize)
				{
					break;
				}

				list.Add(Read());
			}
			return list.ToArray();
		}
		public int Read(T[] buffer, int offset, int length)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException(nameof(buffer));
			}

			var arr = Read(length);
			var count = Math.Min(arr.Length, length);

			Array.Copy(arr, 0, buffer, offset, count);
			return count;
		}

		public void Write(T value)
		{
			if (mCurrentOffset + Marshal.SizeOf<T>() > mSize)
			{
				return;
			}

			Marshal.StructureToPtr(value, mPtr + mCurrentOffset, true);
			Seek(Marshal.SizeOf<T>(), SeekOrigin.Current);
		}
		public void Write(IEnumerable<T> values)
		{
			if (values == null)
			{
				throw new ArgumentNullException(nameof(values));
			}

			foreach (var value in values)
			{
				if (mCurrentOffset + Marshal.SizeOf<T>() > mSize)
				{
					break;
				}

				Write(value);
			}
		}
		public void Write(T[] buffer, int offset, int length)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException(nameof(buffer));
			}

			var arr = new T[length];
			var count = buffer.Length - offset;

			if (count <= 0)
			{
				return;
			}

			Array.Copy(buffer, offset, arr, 0, Math.Min(count, length));
			Write(arr);
		}

		bool IDataStream<T>.CanRead => true;
		bool IDataStream<T>.CanWrite => true;
		bool IDataStream<T>.CanSeek => true;

		void IDataStream<T>.Flush()
		{
		}

		protected override void OnCleanup()
		{
			if (mPtr == IntPtr.Zero)
			{
				return;
			}

			Marshal.FreeHGlobal(mPtr);
			mPtr = IntPtr.Zero;
			mCurrentOffset = 0;
		}
	}
}