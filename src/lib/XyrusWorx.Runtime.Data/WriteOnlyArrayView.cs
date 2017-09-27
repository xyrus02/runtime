using System;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace XyrusWorx.Runtime 
{
	[PublicAPI]
	public sealed class WriteOnlyArrayView<T> where T : struct
	{
		private readonly IWritableMemory mMemory;
		private readonly bool mReadable;
		private readonly bool mWritable;
		private readonly int mSizeOfT;
		private readonly long mLength;

		public WriteOnlyArrayView([NotNull] IWritableMemory memory)
		{
			if (memory == null)
			{
				throw new ArgumentNullException(nameof(memory));
			}
			
			mMemory = memory;
			
			mSizeOfT = Marshal.SizeOf<T>();
			mLength = memory.Size / mSizeOfT;
		}

		public T this[int index]
		{
			set
			{
				if (index < 0 || index >= mLength)
				{
					throw new IndexOutOfRangeException();
				}
				
				var offset = index * mSizeOfT;
				var address = mMemory.GetPointer() + offset;

				Marshal.StructureToPtr(value, address, true);
			}
		}
	}
}