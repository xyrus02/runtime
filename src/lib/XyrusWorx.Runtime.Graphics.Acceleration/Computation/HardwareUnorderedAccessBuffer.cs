using JetBrains.Annotations;
using SlimDX.Direct3D11;
using Buffer = SlimDX.Direct3D11.Buffer;

namespace XyrusWorx.Runtime.Computation 
{

	[PublicAPI]
	public sealed class HardwareUnorderedAccessBuffer : HardwareBuffer
	{
		private UnorderedAccessView mUnorderedAccessView;
		private ShaderResourceView mResourceView;
		private Buffer mHardwareBuffer;

		public HardwareUnorderedAccessBuffer([NotNull] AccelerationDevice provider, long size) : base(provider, size)
		{
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
		
		protected override void DisposeResource()
		{
			mHardwareBuffer?.Dispose();
			mUnorderedAccessView?.Dispose();
			mResourceView?.Dispose();
		}

		internal UnorderedAccessView GetUnorderedAccessView() => mUnorderedAccessView;
		internal override ShaderResourceView GetShaderResourceView() => mResourceView;
		internal override Buffer GetBuffer() => mHardwareBuffer;
	}

}