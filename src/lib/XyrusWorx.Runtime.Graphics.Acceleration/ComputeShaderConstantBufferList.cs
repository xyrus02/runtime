namespace XyrusWorx.Runtime.Graphics 
{
	class ComputeShaderConstantBufferList : AcceleratedKernelResourceList<SharedStructure>, IResourcePool<IWritable>
	{
		private readonly AcceleratedComputationKernel mParent;

		public ComputeShaderConstantBufferList(AcceleratedComputationKernel parent)
		{
			mParent = parent;
		}

		protected override void SetElement(SharedStructure item, int index)
		{
			mParent.Device.ImmediateContext.ComputeShader.SetConstantBuffer(item?.GetBuffer(), index);
		}

		IWritable IResourcePool<IWritable>.this[int slot]
		{
			set => SetElement(value as SharedStructure, slot);
		}
	}
}