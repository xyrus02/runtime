using System;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace XyrusWorx.Runtime
{
	[PublicAPI]
	public static class ArrayExtensions
	{
		public static void CopyTo<T>(this IReadableMemory memory, [NotNull] T[] targetArray) where T: struct
		{
			if (targetArray == null)
			{
				throw new ArgumentNullException(nameof(targetArray));
			}

			var targetArrayHandle = default(GCHandle);
			try
			{
				targetArrayHandle = GCHandle.Alloc(targetArray, GCHandleType.Pinned);
				UnmanagedBlock.Copy(memory.GetPointer(), targetArrayHandle.AddrOfPinnedObject(), 0, 0, memory.Size);
			}
			finally
			{
				if (targetArrayHandle.IsAllocated)
				{
					targetArrayHandle.Free();
				}
			}
		}
		public static void FromArray<T>(this IWritableMemory memory, [NotNull] T[] sourceArray) where T: struct
		{
			if (sourceArray == null)
			{
				throw new ArgumentNullException(nameof(sourceArray));
			}

			var sourceArrayHandle = default(GCHandle);
			try
			{
				sourceArrayHandle = GCHandle.Alloc(sourceArray, GCHandleType.Pinned);
				UnmanagedBlock.Copy(sourceArrayHandle.AddrOfPinnedObject(), memory.GetPointer(), 0, 0, memory.Size);
			}
			finally
			{
				if (sourceArrayHandle.IsAllocated)
				{
					sourceArrayHandle.Free();
				}
			}
		}
		
	}
}
