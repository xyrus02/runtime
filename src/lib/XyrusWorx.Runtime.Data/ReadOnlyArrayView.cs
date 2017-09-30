using System;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace XyrusWorx.Runtime 
{
	[PublicAPI]
	public sealed class ReadOnlyArrayView<T> : IView where T : struct
	{
		private readonly IReadableMemory mMemory;
		private readonly int mSizeOfT;
		private readonly long mLength;

		public ReadOnlyArrayView([NotNull] IReadableMemory memory)
		{
			if (memory == null)
			{
				throw new ArgumentNullException(nameof(memory));
			}
			
			mMemory = memory;
			
			mSizeOfT = Marshal.SizeOf<T>();
			mLength = memory.Size / mSizeOfT;
		}

		public IMemoryBlock RawMemory => mMemory;
		
		public T this[int index]
		{
			get
			{
				if (index < 0 || index >= mLength)
				{
					throw new IndexOutOfRangeException();
				}
				
				var offset = index * mSizeOfT;
				var address = mMemory.GetPointer() + offset;

				return Marshal.PtrToStructure<T>(address);
			}
		}
	}

}