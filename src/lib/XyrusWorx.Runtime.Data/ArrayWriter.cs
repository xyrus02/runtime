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
			set => Write(index, value);
		}
		public void Write(int index, params T[] elements)
		{
			if (index < 0)
			{
				throw new IndexOutOfRangeException();
			}

			using (var buf = new UnmanagedBlock(mSizeOfT))
			{
				for (int offset = index * mSizeOfT, i = 0; i < elements.Length; i++, offset += mSizeOfT)
				{
					Marshal.StructureToPtr(elements[i], buf, true);
					mMemory.Write(buf, offset, mSizeOfT);
				}
			}
		}
	}
}