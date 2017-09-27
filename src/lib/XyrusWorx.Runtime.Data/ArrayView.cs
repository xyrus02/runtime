using System;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace XyrusWorx.Runtime 
{

	[PublicAPI]
	public sealed class ArrayView<T> : IView where T : struct
	{
		private readonly IMemoryBlock mMemory;
		private readonly bool mReadable;
		private readonly bool mWritable;
		private readonly int mSizeOfT;
		private readonly long mLength;

		public ArrayView([NotNull] IMemoryBlock memory)
		{
			if (memory == null)
			{
				throw new ArgumentNullException(nameof(memory));
			}
			
			mMemory = memory;
			
			mSizeOfT = Marshal.SizeOf<T>();
			mLength = memory.Size / mSizeOfT;
			
			mReadable = memory is IReadableMemory;
			mWritable = memory is IWritableMemory;

			if (!mReadable && !mWritable)
			{
				// neither readable nor writable!? must be some unknown memory restriction...
				mReadable = mWritable = true;
			}
		}

		public IMemoryBlock RawMemory => mMemory;
		
		public T this[int index]
		{
			get
			{
				if (!mReadable)
				{
					throw new AccessViolationException("The underlying memory block does not allow reading.");
				}
				
				if (index < 0 || index >= mLength)
				{
					throw new IndexOutOfRangeException();
				}
				
				var offset = index * mSizeOfT;
				var address = mMemory.GetPointer() + offset;

				return Marshal.PtrToStructure<T>(address);
			}
			set
			{
				if (!mWritable)
				{
					throw new AccessViolationException("The underlying memory block does not allow writing.");
				}
				
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