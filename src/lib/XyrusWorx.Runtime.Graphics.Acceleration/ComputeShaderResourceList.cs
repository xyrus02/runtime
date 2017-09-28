using XyrusWorx.Runtime.Graphics.Imaging;

namespace XyrusWorx.Runtime.Graphics 
{
	class ComputeShaderResourceList : AcceleratedKernelResourceList<HardwareResource>, IResourcePool<IWritable>
	{
		private readonly AcceleratedComputationKernel mParent;

		public ComputeShaderResourceList(AcceleratedComputationKernel parent)
		{
			mParent = parent;
		}

		protected override void SetElement(HardwareResource item, int index)
		{
			mParent.Device.ImmediateContext.ComputeShader.SetShaderResource(item?.GetShaderResourceView(), index);
		}
		
		IWritable IResourcePool<IWritable>.this[int slot]
		{
			set => SetElement(value as SharedMemoryBlock, slot);
		}
	}
}