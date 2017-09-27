using System;
using JetBrains.Annotations;

namespace XyrusWorx.Runtime.IO
{
	[PublicAPI]
	public class Buffer2D<T> : BufferBase<T>, IAccessibleBuffer<T, Int2> where T : struct
	{
		private bool mDisposedValue;
		private T[,] mBuffer;

		~Buffer2D()
		{
			Dispose(false);
		}
		public Buffer2D(int width, int height)
		{
			if (width <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(width));
			}
			
			if (height <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(height));
			}

			mBuffer = Alloc(width, height);
			Width = width;
			Height = height;
		}
		public override void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public T this[int x, int y]
		{
			get => mBuffer[x,y];
			set => mBuffer[x,y] = value;
		}
		public override T this[int address]
		{
			get => this[address%Width, address/Width];
			set => this[address%Width, address/Width] = value;
		}

		public int Width { get; }
		public int Height { get; }

		public override int Length
		{
			get => Width * Height;
		}

		public virtual void Clear()
		{
			Array.Clear(mBuffer, 0, Width * Height);
		}

		protected virtual void CleanupOverride()
		{
		}

		private void Dispose(bool disposing)
		{
			if (mDisposedValue)
			{
				return;
			}
			
			if (disposing)
			{
				CleanupOverride();
			}

			mBuffer = null;
			mDisposedValue = true;
		}
		private static T[,] Alloc(int pWidth, int pHeight) => new T[pWidth,pHeight];

		public T Get(Int2 address) => this[address.x, address.y];
		public void Set(Int2 address, T data) => this[address.x, address.y] = data;
		public void Load(IntPtr dataPtr, int dataLength)
		{
			throw new NotSupportedException();
		}
		public void Flush()
		{
		}

		public T[] AsArray()
		{
			var temp = new T[Width * Height];
			for (var j = 0; j < Height; j++)
			{
				for (var i = 0; i < Width; i++)
				{
					temp[j*Width + i] = mBuffer[i, j];
				}
			}
			return temp;
		}
	}
}