using System;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace XyrusWorx.Runtime 
{
	[PublicAPI]
	public sealed class ArrayReader<T> where T : struct
	{
		private readonly IReadable mMemory;
		private readonly int mSizeOfT;

		public ArrayReader([NotNull] IReadable memory)
		{
			if (memory == null)
			{
				throw new ArgumentNullException(nameof(memory));
			}
			
			mMemory = memory;
			mSizeOfT = Marshal.SizeOf<T>();
		}

		public T this[int index]
		{
			get => Read(index);
		}

		public void Read([NotNull] T[] target) => Read(target, 0, target.Length);
		public void Read([NotNull] T[] target, int index, int count)
		{
			if (target == null)
			{
				throw new ArgumentNullException(nameof(target));
			}
			
			if (index < 0)
			{
				throw new IndexOutOfRangeException();
			}

			if (count <= 0)
			{
				return;
			}
				
			using (var buf = new UnmanagedBlock(mSizeOfT * count))
			{
				mMemory.Read(buf.Pointer, 0, mSizeOfT * count);

				for (int offset = index * mSizeOfT, i = 0; i < count; i++, offset += mSizeOfT)
				{
					if (i >= target.Length)
					{
						break;
					}
					
					target[i] = Marshal.PtrToStructure<T>(buf.Pointer + offset);
				}
			}
		}
		
		public T Read(int index) => Read(index, 1)[0];
		public T[] Read(int index, int count)
		{
			if (index < 0)
			{
				throw new IndexOutOfRangeException();
			}

			if (count <= 0)
			{
				return new T[0];
			}
				
			using (var buf = new UnmanagedBlock(mSizeOfT))
			{
				var outArr = new T[count];
				
				for (int offset = index * mSizeOfT, i = 0; i < count; i++, offset += mSizeOfT)
				{
					mMemory.Read(buf.Pointer + offset * mSizeOfT, offset, mSizeOfT);
					outArr[i] = Marshal.PtrToStructure<T>(buf);
				}

				return outArr;
			}
		}
	}
}