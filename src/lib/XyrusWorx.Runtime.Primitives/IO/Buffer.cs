using System;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace XyrusWorx.Runtime.IO
{
	[PublicAPI]
	public class Buffer<T> : BufferBase<T>, IAccessibleBuffer<T, int> where T : struct
	{
		private readonly int mLength;
		private bool mDisposedValue;
		private T[] mBuffer;

		~Buffer()
		{
			Dispose(false);
		}

		public Buffer([NotNull] IDataStream<T> stream)
		{
			if (stream == null)
			{
				throw new ArgumentNullException(nameof(stream));
			}

			var elementSize = Marshal.SizeOf(typeof (T));
			var length = (int)stream.Length/elementSize;
			var data = stream.Read(length);

			mBuffer = data;
			mLength = data.Length;
		}
		public Buffer([NotNull] T[] data)
		{
			if (data == null)
			{
				throw new ArgumentNullException(nameof(data));
			}

			mBuffer = data;
			mLength = data.Length;
		}
		public Buffer(int length)
		{
			if (length <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(length));
			}

			mBuffer = Alloc(length);
			mLength = length;
		}
		public override void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public override T this[int offset]
		{
			get { return mBuffer[offset]; }
			set { mBuffer[offset] = value; }
		}
		public override int Length => mLength;

		public T[] AsArray() => mBuffer;

		public void Clear()
		{
			Array.Clear(mBuffer, 0, mBuffer.Length);
		}

		protected virtual void CleanupOverride()
		{
		}

		private void Dispose(bool disposing)
		{
			if (!mDisposedValue)
			{
				if (disposing)
				{
					CleanupOverride();
				}

				mBuffer = null;
				mDisposedValue = true;
			}
		}
		private static T[] Alloc(int length)
		{
			var ret = new T[length];
			return ret;
		}

		public T Get(int address) => this[address];
		public void Set(int address, T data) => this[address] = data;
		public void Load(IntPtr dataPtr, int dataLength)
		{
			throw new NotSupportedException();
		}
		public void Flush()
		{
		}
	}
}