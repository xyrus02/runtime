using System;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace XyrusWorx.Runtime 
{
	[PublicAPI]
	public class UnmanagedArray : Resource, IUnmanagedArray
	{
		private readonly IUnmanagedMemory mAllocatedMemory;
		
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
		
		public bool IsAllocated
		{
			get => mAllocatedMemory.Pointer != IntPtr.Zero;
		}
		
		protected override void DisposeOverride()
		{
			mAllocatedMemory.Dispose();
		}
	}
}