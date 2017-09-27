using System;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace XyrusWorx.Runtime 
{
	[PublicAPI]
	public class UnmanagedArray : Resource, IUnmanagedArray, IReadableMemory, IWritableMemory
	{
		private readonly UnmanagedBlock mAllocatedMemory;
		
		public UnmanagedArray(int elementSize, int length)
		{
			if (elementSize <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(elementSize));
			}
			
			if (length <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(length));
			}

			ElementSize = elementSize;
			Length = length;
			
			mAllocatedMemory = new UnmanagedBlock(ElementSize * (long)Length);
		}

		public static UnmanagedArray FromType([NotNull] Type type, int length)
		{
			if (type == null)
			{
				throw new ArgumentNullException(nameof(type));
			}

			if (!type.IsValueType)
			{
				throw new ArgumentException("Only value types can be used to allocate an unmanaged array.", nameof(type));
			}

			return new UnmanagedArray(Marshal.SizeOf(type), length);
		}
		public static UnmanagedArray FromType<T>(int length)
			where T : struct => FromType(typeof(T), length);
		
		public int Length { get; }
		public int ElementSize { get; }
		
		public IntPtr Pointer => mAllocatedMemory.Pointer;
		
		public bool IsAllocated => mAllocatedMemory.IsAllocated;
		public long Size => (long)Length * ElementSize;
		
		public void Read(IntPtr target, int readOffset, long bytesToRead) => mAllocatedMemory.Read(target, readOffset, bytesToRead);
		public void Write(IntPtr source, int writeOffset, long bytesToWrite) => mAllocatedMemory.Write(source, writeOffset, bytesToWrite);
		
		protected override void DisposeOverride()
		{
			mAllocatedMemory.Dispose();
		}

		IntPtr IMemoryBlock.GetPointer() => Pointer;
	}
}