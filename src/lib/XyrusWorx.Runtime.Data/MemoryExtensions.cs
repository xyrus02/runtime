using System;
using JetBrains.Annotations;

namespace XyrusWorx.Runtime
{
	[PublicAPI]
	public static class MemoryExtensions
	{
		public static void Read([NotNull] this IReadable readableMemory, [NotNull] IWritableMemory targetMemory)
		{
			if (readableMemory == null)
			{
				throw new ArgumentNullException(nameof(readableMemory));
			}
			
			if (targetMemory == null)
			{
				throw new ArgumentNullException(nameof(targetMemory));
			}

			readableMemory.Read(targetMemory.GetPointer(), 0, targetMemory.Size);
		}
		public static void Write([NotNull] this IWritable writableMemory, [NotNull] IReadableMemory sourceMemory)
		{
			if (writableMemory == null)
			{
				throw new ArgumentNullException(nameof(writableMemory));
			}
			
			if (sourceMemory == null)
			{
				throw new ArgumentNullException(nameof(sourceMemory));
			}

			writableMemory.Write(sourceMemory.GetPointer(), 0, sourceMemory.Size);
		}

		[NotNull]
		public static ArrayView<T> AsArray<T>([NotNull] this IReadWriteMemory readWriteMemory) where T : struct
		{
			if (readWriteMemory == null)
			{
				throw new ArgumentNullException(nameof(readWriteMemory));
			}

			return new ArrayView<T>(readWriteMemory);
		}
		
		[NotNull]
		public static ReadOnlyArrayView<T> AsReadOnlyArray<T>([NotNull] this IReadableMemory readableMemory) where T : struct
		{
			if (readableMemory == null)
			{
				throw new ArgumentNullException(nameof(readableMemory));
			}

			return new ReadOnlyArrayView<T>(readableMemory);
		}
		
		[NotNull]
		public static WriteOnlyArrayView<T> AsWriteOnlyArray<T>([NotNull] this IWritableMemory writableMemory) where T : struct
		{
			if (writableMemory == null)
			{
				throw new ArgumentNullException(nameof(writableMemory));
			}

			return new WriteOnlyArrayView<T>(writableMemory);
		}
		
		[NotNull]
		public static MemoryWindow GetWindow([NotNull] this IReadWriteMemory readWriteMemory, int offset, long size)
		{
			if (readWriteMemory == null)
			{
				throw new ArgumentNullException(nameof(readWriteMemory));
			}

			return new MemoryWindow(readWriteMemory, offset, size);
		}
		
		[NotNull]
		public static ReadOnlyMemoryWindow GetReadOnlyWindow([NotNull] this IReadableMemory readableMemory, int offset, long size)
		{
			if (readableMemory == null)
			{
				throw new ArgumentNullException(nameof(readableMemory));
			}

			return new ReadOnlyMemoryWindow(readableMemory, offset, size);
		}
		
		[NotNull]
		public static WriteOnlyMemoryWindow GetWriteOnlyWindow([NotNull] this IWritableMemory writableMemory, int offset, long size)
		{
			if (writableMemory == null)
			{
				throw new ArgumentNullException(nameof(writableMemory));
			}

			return new WriteOnlyMemoryWindow(writableMemory, offset, size);
		}
	}
}
