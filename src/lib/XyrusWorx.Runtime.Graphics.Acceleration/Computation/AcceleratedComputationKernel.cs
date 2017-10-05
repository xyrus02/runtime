using System;
using JetBrains.Annotations;
using SlimDX.Direct3D11;
using XyrusWorx.Runtime.Expressions;

namespace XyrusWorx.Runtime.Computation 
{
	[PublicAPI]
	public sealed class AcceleratedComputationKernel : AcceleratedKernel, IComputationKernel
	{
		private DelegatedHardwareResourceList<HardwareConstantBuffer> mConstants;
		private DelegatedHardwareResourceList<HardwareInputBuffer> mResources;
		private DelegatedHardwareResourceList<HardwareOutputBuffer> mOutputs;
		private ComputeShader mShader;

		private AcceleratedComputationKernel([NotNull] AccelerationDevice device) : base(device)
		{
			mConstants = new DelegatedHardwareResourceList<HardwareConstantBuffer>(this, (dc, res, slot) => dc.ComputeShader.SetConstantBuffer(res.GetBuffer(), slot));
			mResources = new DelegatedHardwareResourceList<HardwareInputBuffer>(this, (dc, res, slot) => dc.ComputeShader.SetShaderResource(res.GetShaderResourceView(), slot));
			mOutputs = new DelegatedHardwareResourceList<HardwareOutputBuffer>(this, (dc, res, slot) => dc.ComputeShader.SetUnorderedAccessView(res.GetUnorderedAccessView(), slot));
		}
		
		public Vector3<uint> ThreadGroupCount { get; set; }

		public IResourcePool<HardwareConstantBuffer> Constants => mConstants;
		public IResourcePool<HardwareInputBuffer> Resources => mResources;
		public IResourcePool<HardwareOutputBuffer> Outputs => mOutputs;
		
		IResourcePool<IWritable> IComputationKernel.Constants => mConstants;
		IResourcePool<IWritable> IComputationKernel.Resources => mResources;
		IResourcePool<IReadable> IComputationKernel.Outputs => mOutputs;
		
		public void Execute()
		{
			Device.ImmediateContext.Dispatch((int)ThreadGroupCount.x, (int)ThreadGroupCount.y, (int)ThreadGroupCount.z);
		}
		
		[NotNull]
		public static AcceleratedComputationKernel FromBytecode([NotNull] AccelerationDevice device, [NotNull] IReadableMemory bytecode)
		{
			if (device == null)
			{
				throw new ArgumentNullException(nameof(device));
			}
			
			if (bytecode == null)
			{
				throw new ArgumentNullException(nameof(bytecode));
			}

			var kernel = new AcceleratedComputationKernel(device);
			kernel.Load(bytecode);
			return kernel;
		}
		
		[NotNull]
		public static AcceleratedComputationKernel FromSource([NotNull] AccelerationDevice device, [NotNull] KernelSourceWriter source, CompilerContext context = null)
		{
			if (device == null)
			{
				throw new ArgumentNullException(nameof(device));
			}
			
			if (source == null)
			{
				throw new ArgumentNullException(nameof(source));
			}

			var kernel = new AcceleratedComputationKernel(device);
			context = context ?? new CompilerContext();
			kernel.Compile(source, context);
			return kernel;
		}

		protected override void OnBytecodeLoaded()
		{
			mShader = new ComputeShader(Device, Bytecode);
			Device.ImmediateContext.ComputeShader.Set(mShader);
		}
		protected override string GetProfileName() => "cs_5_0";
		protected override void Deallocate()
		{
			mConstants.Reset();
			mResources.Reset();
			mOutputs.Reset();
			
			mShader?.Dispose();
			mShader = null;
		}
	}
}