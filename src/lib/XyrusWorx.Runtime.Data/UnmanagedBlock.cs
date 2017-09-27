using System;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace XyrusWorx.Runtime
{
	[PublicAPI]
	public sealed class UnmanagedBlock : Resource, IReadableMemory, IWritableMemory
	{
		public UnmanagedBlock(long size)
		{
			if (size <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(size));
			}
			
			Pointer = Marshal.AllocHGlobal(new IntPtr(size));
			Size = size;
		}

		public IntPtr Pointer { get; private set; }
		
		public long Size { get; }
		public bool IsAllocated
		{
			get => Pointer != IntPtr.Zero;
		}

		protected override void DisposeOverride()
		{
			if (Pointer == IntPtr.Zero)
			{
				return;
			}
			
			Marshal.FreeHGlobal(Pointer);
			Pointer = IntPtr.Zero;
		}

		public static unsafe implicit operator void*(UnmanagedBlock block)
		{
			if (block == null || !block.IsAllocated)
			{
				return null;
			}

			return block.Pointer.ToPointer();
		}
		public static implicit operator IntPtr(UnmanagedBlock block)
		{
			if (block == null || !block.IsAllocated)
			{
				return IntPtr.Zero;
			}

			return block.Pointer;
		}

		public void Read(IntPtr target, int readOffset, long bytesToRead) => Copy(Pointer, target, readOffset, 0, bytesToRead);
		public void Write(IntPtr source, int writeOffset, long bytesToWrite) => Copy(source, Pointer, 0, writeOffset, bytesToWrite);

		internal static unsafe void Copy(IntPtr source, IntPtr target, int sourceOffset, int targetOffset, long size)
		{
			if (source == IntPtr.Zero || target == IntPtr.Zero)
			{
				throw new AccessViolationException();
			}

			if (sourceOffset < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(sourceOffset));
			}
			
			if (targetOffset < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(targetOffset));
			}
			
			if (size < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(size));
			}

			if (size == 0)
			{
				return;
			}
			
			var src = (source + sourceOffset).ToPointer();
			var dst = (target + targetOffset).ToPointer();

			Buffer.MemoryCopy(src, dst, size, size);
		}

		IntPtr IMemoryBlock.GetPointer() => Pointer;
	}
}
