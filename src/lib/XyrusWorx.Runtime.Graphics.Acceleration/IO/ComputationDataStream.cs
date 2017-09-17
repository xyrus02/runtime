using System;
using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;
using SlimDX;
using XyrusWorx.Collections;
using XyrusWorx.Runtime.IO;

namespace XyrusWorx.Runtime.Graphics.IO
{
	[PublicAPI]
	public class ComputationDataStream<T> : Stream, IDataStream<T>, IUnmanagedBuffer where T: struct
	{
		private readonly AcceleratedComputationProvider mProvider;
		private readonly DataStream mSource;

		private SlimDX.Direct3D11.Resource mResource;
		private int mSubResource;

		internal ComputationDataStream([NotNull] AcceleratedComputationProvider provider, [NotNull] DataStream source, [NotNull] SlimDX.Direct3D11.Resource resource, int subResource)
		{
			if (provider == null)
			{
				throw new ArgumentNullException(nameof(provider));
			}
			
			if (source == null)
			{
				throw new ArgumentNullException(nameof(source));
			}
			
			if (resource == null)
			{
				throw new ArgumentNullException(nameof(resource));
			}

			mProvider = provider;
			mSource = source;
			mResource = resource;
			mSubResource = subResource;
		}

		public override void Flush()
		{
			mSource.Flush();
		}
		public override void SetLength(long value)
		{
			mSource.SetLength(value);
		}

		public override long Seek(long offset, SeekOrigin origin) => mSource.Seek(offset, origin);
		public override int  Read(byte[] buffer, int offset, int count) => mSource.Read(buffer, offset, count);
		public override void Write(byte[] buffer, int offset, int count) => mSource.Write(buffer, offset, count);

		public override bool CanSeek
		{
			get => mSource.CanSeek;
		}
		public override bool CanRead
		{
			get => mSource.CanRead;
		}
		public override bool CanWrite
		{
			get => mSource.CanWrite;
		}
		public override long Length
		{
			get => mSource.Length;
		}
		public override long Position
		{
			get => mSource.Position;
			set => mSource.Position = value;
		}

		public T Read() => mSource.Read<T>();
		public T[] Read(int count) => mSource.ReadRange<T>(count);
		public int Read(T[] buffer, int offset, int length)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException(nameof(buffer));
			}

			return mSource.ReadRange(buffer, offset, length);
		}

		public void Write(T value)
		{
			mSource.Write(value);
		}
		public void Write(IEnumerable<T> values)
		{
			if (values == null)
			{
				throw new ArgumentNullException(nameof(values));
			}

			mSource.WriteRange(values.AsArray());
		}
		public void Write(T[] buffer, int offset, int length)
		{
			mSource.WriteRange(buffer, offset, length);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && mResource != null)
			{
				mProvider.HardwareDevice.ImmediateContext.UnmapSubresource(mResource, mSubResource);
				mResource = null;
				mSubResource = 0;
			}

			if (disposing)
			{
				OwnedBuffer?.Dispose();
				OwnedResource?.Dispose();

				OwnedBuffer = null;
				OwnedResource = null;
			}

			base.Dispose(disposing);
		}

		internal ComputationSwapBufferResource<T> OwnedBuffer { get; set; }
		internal SlimDX.Direct3D11.Resource OwnedResource { get; set; }

		public IntPtr Data
		{
			get => mSource?.DataPointer ?? IntPtr.Zero;
		}
		public long SizeInBytes
		{
			get => mSource.Length;
		}
	}
}