using System;
using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;

namespace XyrusWorx.Runtime.IO
{
	[PublicAPI]
	public interface IDataStream<T> : IDisposable where T : struct
	{
		long Seek(long offset, SeekOrigin origin);
		long Position { get; }
		long Length { get; }

		T Read();
		T[] Read(int count);
		int Read([NotNull] byte[] buffer, int offset, int length);
		int Read([NotNull] T[] buffer, int offset, int length);

		void Write(T value);
		void Write([NotNull] IEnumerable<T> values);
		void Write([NotNull] byte[] buffer, int offset, int length);
		void Write([NotNull] T[] buffer, int offset, int length);

		bool CanRead { get; }
		bool CanWrite { get; }
		bool CanSeek { get; }

		void Flush();
	}
}