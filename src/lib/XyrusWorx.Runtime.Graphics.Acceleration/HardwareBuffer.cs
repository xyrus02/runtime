using System;
using JetBrains.Annotations;
using SlimDX;
using SlimDX.Direct3D11;
using XyrusWorx.Runtime.Imaging;
using Buffer = SlimDX.Direct3D11.Buffer;

namespace XyrusWorx.Runtime 
{
	[PublicAPI]
	public abstract class HardwareBuffer : HardwareResource, IReadable, IWritable
	{
		internal HardwareBuffer([NotNull] AccelerationDevice provider, long size) : base(provider)
		{
			if (size <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(size));
			}
			
			Size = size;
		}
		
		public void Read(IntPtr target, int readOffset, long bytesToRead)
		{
			var buffer = GetBuffer();
			DataBox mappedResource = null;
			
			try
			{
				mappedResource = Device.ImmediateContext.MapSubresource(buffer, MapMode.Read, MapFlags.None);
				UnmanagedBlock.Copy(mappedResource.Data.DataPointer, target, readOffset, 0, bytesToRead);
			}
			finally
			{
				if (mappedResource != null)
				{
					Device.ImmediateContext.UnmapSubresource(buffer, 0);
				}
			}
		}
		public void Write(IntPtr source, int writeOffset, long bytesToWrite)
		{
			var buffer = GetBuffer();
			DataBox mappedResource = null;
			
			try
			{
				mappedResource = Device.ImmediateContext.MapSubresource(buffer, MapMode.Write, MapFlags.None);
				UnmanagedBlock.Copy(source, mappedResource.Data.DataPointer, 0, writeOffset, bytesToWrite);
			}
			finally
			{
				if (mappedResource != null)
				{
					Device.ImmediateContext.UnmapSubresource(buffer, 0);
				}
			}
		}
		
		public long Size { get; }
		
		[Pure]
		public UnmanagedBlock Read(int offset = 0, long size = 0)
		{
			var block = new UnmanagedBlock(size);
			Read(block.Pointer, offset, size <= 0 ? Size : size);
			return block;
		}
		public void Write([NotNull] UnmanagedBlock block, int offset = 0, long size = 0)
		{
			if (block == null)
			{
				throw new ArgumentNullException(nameof(block));
			}

			Write(block.Pointer, offset, size <= 0 ? block.Size : size);
		}

		internal abstract Buffer GetBuffer();
	}
}