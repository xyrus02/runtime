using System;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using SlimDX;
using SlimDX.Direct3D11;
using Buffer = SlimDX.Direct3D11.Buffer;

namespace XyrusWorx.Runtime 
{
	[PublicAPI]
	public class HardwareConstantBuffer : HardwareBuffer, IWritable
	{
		private Buffer mHardwareBuffer;

		public HardwareConstantBuffer([NotNull] AccelerationDevice provider, long size) : base(provider, size)
		{
			var bufferSize = Math.Max(16, ((int)size + 15) / 16 * 16);
			var bufferDescription = new BufferDescription
			{
				BindFlags = BindFlags.ConstantBuffer,
				CpuAccessFlags = CpuAccessFlags.Write,
				OptionFlags = ResourceOptionFlags.None,
				SizeInBytes = bufferSize,
				Usage = ResourceUsage.Dynamic
			};

			mHardwareBuffer = new Buffer(Device, bufferDescription);
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
		
		internal override Buffer GetBuffer() => mHardwareBuffer;
		internal override ShaderResourceView GetShaderResourceView() => throw new NotSupportedException("A constant buffer cannot be used as a shader resource.");
		
		protected override void DisposeResource()
		{
			mHardwareBuffer?.Dispose();
		}
	}

	[PublicAPI]
	public class HardwareConstantBuffer<TStruct> : HardwareConstantBuffer where TStruct : struct
	{
		private TStruct mStructure;
		
		public HardwareConstantBuffer([NotNull] AccelerationDevice provider) : base(provider, Marshal.SizeOf<TStruct>()) { }
		public TStruct Structure
		{
			get => mStructure;
			set
			{
				using (var buffer = new UnmanagedBlock(Size))
				{
					Marshal.StructureToPtr(value, buffer, true);
					Write(buffer, 0, Size);
					mStructure = value;
				}
			}
		}
	}
}