using System;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace XyrusWorx.Runtime
{
	[PublicAPI]
	public sealed class UnmanagedBlock : Resource, IUnmanagedBlock
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
	}
}
