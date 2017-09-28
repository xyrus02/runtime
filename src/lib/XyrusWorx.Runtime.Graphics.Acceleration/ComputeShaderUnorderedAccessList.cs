namespace XyrusWorx.Runtime.Graphics 
{
	class ComputeShaderUnorderedAccessList : AcceleratedKernelResourceList<SharedMemoryBlock>, IResourcePool<IReadable>
	{
		private readonly AcceleratedComputationKernel mParent;

		public ComputeShaderUnorderedAccessList(AcceleratedComputationKernel parent)
		{
			mParent = parent;
		}

		protected override void SetElement(SharedMemoryBlock item, int index)
		{
			mParent.Device.ImmediateContext.ComputeShader.SetUnorderedAccessView(item?.GetUnorderedAccessView(), index);
		}

		IReadable IResourcePool<IReadable>.this[int slot]
		{
			set => SetElement(value as SharedMemoryBlock, slot);
		}
	}
}