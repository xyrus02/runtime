using System;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using SlimDX;
using SlimDX.Direct3D11;
using Buffer = SlimDX.Direct3D11.Buffer;

namespace XyrusWorx.Runtime.Computation 
{
	[PublicAPI]
	public sealed class HardwareInputBuffer : HardwareBuffer, IWritable
	{
		private ShaderResourceView mResourceView;
		private Buffer mHardwareBuffer;

		public HardwareInputBuffer([NotNull] AccelerationDevice provider, [NotNull] Type itemType, int itemCount) : base(provider, Marshal.SizeOf(itemType) * itemCount)
		{
			var description = new BufferDescription
			{
				BindFlags = BindFlags.ShaderResource,
				CpuAccessFlags = CpuAccessFlags.Write,
				OptionFlags = ResourceOptionFlags.StructuredBuffer,
				SizeInBytes = Marshal.SizeOf(itemType) * itemCount,
				StructureByteStride = Marshal.SizeOf(itemType),
				Usage = ResourceUsage.Dynamic
			};

			mHardwareBuffer = new Buffer(Device, description);
			mResourceView = new ShaderResourceView(Device, mHardwareBuffer);
		}
		
		public void Write(IntPtr source, int writeOffset, long bytesToWrite)
		{
			var buffer = GetBuffer();
			DataBox mappedResource = null;
			
			try
			{
				mappedResource = Device.ImmediateContext.MapSubresource(buffer, MapMode.WriteDiscard, MapFlags.None);
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
			mResourceView?.Dispose();
		}

		internal override ShaderResourceView GetShaderResourceView() => mResourceView;
		internal override Buffer GetBuffer() => mHardwareBuffer;
	}

}