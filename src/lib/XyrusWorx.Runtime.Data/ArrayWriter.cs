using System;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace XyrusWorx.Runtime 
{
	[PublicAPI]
	public sealed class ArrayWriter<T> where T : struct
	{
		private readonly IWritable mMemory;
		private readonly int mSizeOfT;

		public ArrayWriter([NotNull] IWritable memory)
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
			set => Write(new[]{value}, index, 1);
		}

		public void Write(T[] source)
		{
			Write(source, 0, source.Length);
		}
		public void Write(T[] source, int index, int count)
		{
			if (source == null)
			{
				throw new ArgumentNullException(nameof(source));
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
				for (int offset = index * mSizeOfT, i = 0; i < count; i++, offset += mSizeOfT)
				{
					if (i >= source.Length)
					{
						break;
					}
					
					Marshal.StructureToPtr(source[i], buf.Pointer + offset, true);
				}
				
				mMemory.Write(buf.Pointer, 0, buf.Size);
			}
		}
	}
}