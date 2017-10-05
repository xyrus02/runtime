using System;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using SlimDX;
using SlimDX.Direct3D11;
using XyrusWorx.Diagnostics;
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

			if (!itemType.IsValueType)
			{
				throw new ArgumentException("Can't use a reference type as buffer structure.", nameof(itemType));
			}

			var nStructure = Marshal.SizeOf(itemType);
			var nBytes = nStructure * itemCount;

			if (Context.IsDebugModeEnabled)
			{
				Context.DiagnosticsWriter.WriteDebug("Allocating {0:###,###,###,###,###,##0} bytes of UAV memory with a structure stride of {1:###,###,###,###,###,##0} bytes",
					nBytes, nStructure);
			}
			
			var bufferDescription = new BufferDescription
			{
				BindFlags = BindFlags.ShaderResource | BindFlags.UnorderedAccess,
				CpuAccessFlags = CpuAccessFlags.None,
				OptionFlags = ResourceOptionFlags.StructuredBuffer,
				SizeInBytes = nBytes,
				StructureByteStride = nStructure,
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
				
				if (Context.IsDebugModeEnabled)
				{
					Context.DiagnosticsWriter.WriteDebug(
						"Reading {2:###,###,###,###,###,##0} bytes from staging buffer (0x{0:X16}+{3:X8}) into RAM (0x{1:X16})", 
						mappedResource.Data.DataPointer.ToInt64(), 
						target.ToInt64(),
						bytesToRead, 
						readOffset);
				}
				
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
			mUnorderedAccessView?.Dispose();
			mResourceView?.Dispose();
			mHardwareBuffer?.Dispose();
			mSwapBuffer?.Dispose();
		}

		internal UnorderedAccessView GetUnorderedAccessView() => mUnorderedAccessView;
		internal override ShaderResourceView GetShaderResourceView() => mResourceView;
		internal override Buffer GetBuffer() => mHardwareBuffer;
	}
}