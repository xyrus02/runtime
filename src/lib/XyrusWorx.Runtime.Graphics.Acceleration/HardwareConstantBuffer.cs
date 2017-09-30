using System.Runtime.InteropServices;
using JetBrains.Annotations;
using SlimDX.Direct3D11;

namespace XyrusWorx.Runtime 
{
	[PublicAPI]
	public class HardwareConstantBuffer : HardwareBuffer
	{
		private Buffer mHardwareBuffer;
		private ShaderResourceView mResourceView;

		public HardwareConstantBuffer([NotNull] AccelerationDevice provider, long size) : base(provider, size)
		{
			var bufferDescription = new BufferDescription
			{
				BindFlags = BindFlags.ConstantBuffer,
				CpuAccessFlags = CpuAccessFlags.Write,
				OptionFlags = ResourceOptionFlags.None,
				SizeInBytes = (int)size,
				Usage = ResourceUsage.Dynamic
			};

			mHardwareBuffer = new Buffer(Device, bufferDescription);
			mResourceView = new ShaderResourceView(Device, mHardwareBuffer);
		}
		
		internal override Buffer GetBuffer() => mHardwareBuffer;
		internal override ShaderResourceView GetShaderResourceView() => mResourceView;
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