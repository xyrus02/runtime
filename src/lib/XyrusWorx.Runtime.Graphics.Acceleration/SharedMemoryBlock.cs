using System;
using JetBrains.Annotations;
using SlimDX;
using SlimDX.Direct3D11;
using XyrusWorx.Runtime.Graphics.Imaging;
using Buffer = SlimDX.Direct3D11.Buffer;

namespace XyrusWorx.Runtime.Graphics 
{
	[PublicAPI]
	public sealed class SharedMemoryBlock : HardwareResource, IReadable, IWritable
	{
		private UnorderedAccessView mUnorderedAccessView;
		private ShaderResourceView mResourceView;
		private Buffer mHardwareBuffer;

		public SharedMemoryBlock([NotNull] AccelerationDevice provider, long size) : base(provider)
		{
			if (size <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(size));
			}
			
			Size = size;

			var bufferDescription = new BufferDescription
			{
				BindFlags = BindFlags.ShaderResource | BindFlags.UnorderedAccess,
				CpuAccessFlags = CpuAccessFlags.None,
				OptionFlags = ResourceOptionFlags.StructuredBuffer,
				SizeInBytes = (int)size,
				StructureByteStride = 1,
				Usage = ResourceUsage.Default
			};

			var viewDescription = new UnorderedAccessViewDescription
			{
				ElementCount = (int)size,
				Format = SlimDX.DXGI.Format.Unknown,
				Dimension = UnorderedAccessViewDimension.Buffer,
				Flags = UnorderedAccessViewBufferFlags.None
			};

			mHardwareBuffer = new Buffer(Device, bufferDescription);
			mResourceView = new ShaderResourceView(Device, mHardwareBuffer);
			mUnorderedAccessView = new UnorderedAccessView(Device, mHardwareBuffer, viewDescription);
		}
		
		public long Size { get; }
		
		public void Read(IntPtr target, int readOffset, long bytesToRead)
		{
			DataBox mappedResource = null;
			try
			{
				mappedResource = Device.ImmediateContext.MapSubresource(mHardwareBuffer, MapMode.Read, MapFlags.None);
				UnmanagedBlock.Copy(mappedResource.Data.DataPointer, target, readOffset, 0, bytesToRead);
			}
			finally
			{
				if (mappedResource != null)
				{
					Device.ImmediateContext.UnmapSubresource(mHardwareBuffer, 0);
				}
			}
		}
		public void Write(IntPtr source, int writeOffset, long bytesToWrite)
		{
			DataBox mappedResource = null;
			try
			{
				mappedResource = Device.ImmediateContext.MapSubresource(mHardwareBuffer, MapMode.Write, MapFlags.None);
				UnmanagedBlock.Copy(source, mappedResource.Data.DataPointer, 0, writeOffset, bytesToWrite);
			}
			finally
			{
				if (mappedResource != null)
				{
					Device.ImmediateContext.UnmapSubresource(mHardwareBuffer, 0);
				}
			}
		}

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

		protected override void DisposeResource()
		{
			mHardwareBuffer?.Dispose();
			mUnorderedAccessView?.Dispose();
			mResourceView?.Dispose();
		}

		internal UnorderedAccessView GetUnorderedAccessView() => mUnorderedAccessView;
		internal override ShaderResourceView GetShaderResourceView() => mResourceView;
	}
}