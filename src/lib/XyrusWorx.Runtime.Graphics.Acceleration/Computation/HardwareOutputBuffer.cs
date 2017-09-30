using System;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using SlimDX;
using SlimDX.Direct3D11;
using Buffer = SlimDX.Direct3D11.Buffer;

namespace XyrusWorx.Runtime.Computation 
{
	[PublicAPI]
	public sealed class HardwareOutputBuffer : HardwareBuffer, IReadable
	{
		private UnorderedAccessView mUnorderedAccessView;
		private ShaderResourceView mResourceView;
		private Buffer mHardwareBuffer;
		private Buffer mSwapBuffer;

		public HardwareOutputBuffer([NotNull] AccelerationDevice provider, [NotNull] Type itemType, int itemCount) : base(provider, Marshal.SizeOf(itemType) * itemCount)
		{
			if (itemType == null)
			{
				throw new ArgumentNullException(nameof(itemType));
			}
			
			var bufferDescription = new BufferDescription
			{
				BindFlags = BindFlags.ShaderResource | BindFlags.UnorderedAccess,
				CpuAccessFlags = CpuAccessFlags.None,
				OptionFlags = ResourceOptionFlags.StructuredBuffer,
				SizeInBytes = Marshal.SizeOf(itemType) * itemCount,
				StructureByteStride = Marshal.SizeOf(itemType),
				Usage = ResourceUsage.Default
			};

			var swapDescription = new BufferDescription
			{
				BindFlags = BindFlags.None,
				CpuAccessFlags = CpuAccessFlags.Read | CpuAccessFlags.Write,
				OptionFlags = ResourceOptionFlags.StructuredBuffer,
				SizeInBytes = bufferDescription.SizeInBytes,
				StructureByteStride = bufferDescription.StructureByteStride,
				Usage = ResourceUsage.Staging
			};

			var viewDescription = new UnorderedAccessViewDescription
			{
				ElementCount = itemCount,
				Format = SlimDX.DXGI.Format.Unknown,
				Dimension = UnorderedAccessViewDimension.Buffer,
				Flags = UnorderedAccessViewBufferFlags.None
			};

			mSwapBuffer = new Buffer(Device, swapDescription);
			mHardwareBuffer = new Buffer(Device, bufferDescription);
			mResourceView = new ShaderResourceView(Device, mHardwareBuffer);
			mUnorderedAccessView = new UnorderedAccessView(Device, mHardwareBuffer, viewDescription);
		}
		
		[Pure]
		public UnmanagedBlock Read(int offset = 0, long size = 0)
		{
			var block = new UnmanagedBlock(size);
			Read(block.Pointer, offset, size <= 0 ? Size : size);
			return block;
		}
		public void Read(IntPtr target, int readOffset, long bytesToRead)
		{
			DataBox mappedResource = null;
			
			try
			{
				Device.ImmediateContext.CopyResource(mHardwareBuffer, mSwapBuffer);
				mappedResource = Device.ImmediateContext.MapSubresource(mSwapBuffer, MapMode.Read, MapFlags.None);
				UnmanagedBlock.Copy(mappedResource.Data.DataPointer, target, readOffset, 0, bytesToRead);
			}
			finally
			{
				if (mappedResource != null)
				{
					Device.ImmediateContext.UnmapSubresource(mSwapBuffer, 0);
				}
			}
		}
		
		protected override void DisposeResource()
		{
			mHardwareBuffer?.Dispose();
			mUnorderedAccessView?.Dispose();
			mResourceView?.Dispose();
			mSwapBuffer?.Dispose();
		}

		internal UnorderedAccessView GetUnorderedAccessView() => mUnorderedAccessView;
		internal override ShaderResourceView GetShaderResourceView() => mResourceView;
		internal override Buffer GetBuffer() => mHardwareBuffer;
	}
}