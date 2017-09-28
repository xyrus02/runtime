using System;
using JetBrains.Annotations;

namespace XyrusWorx.Runtime.Imaging
{
	[PublicAPI]
	public static class TextureExtensions
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
	}
}
