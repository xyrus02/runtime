using System;
using JetBrains.Annotations;
using XyrusWorx.Runtime.Expressions;
using XyrusWorx.Runtime.Imaging;

namespace XyrusWorx.Runtime.Computation 
{
	[PublicAPI]
	public sealed class AcceleratedComputationKernel : AcceleratedKernel, IComputationKernel
	{
		private DelegatedHardwareResourceList<HardwareConstantBuffer> mConstants;
		private DelegatedHardwareResourceList<HardwareTexture> mTextures;
		private DelegatedHardwareResourceList<HardwareUnorderedAccessBuffer> mOutputs;

		private AcceleratedComputationKernel([NotNull] AccelerationDevice device) : base(device)
		{
			mConstants = new DelegatedHardwareResourceList<HardwareConstantBuffer>(this, (dc, res, slot) => dc.ComputeShader.SetConstantBuffer(res.GetBuffer(), slot));
			mTextures = new DelegatedHardwareResourceList<HardwareTexture>(this, (dc, res, slot) => dc.ComputeShader.SetShaderResource(res.GetShaderResourceView(), slot));
			mOutputs = new DelegatedHardwareResourceList<HardwareUnorderedAccessBuffer>(this, (dc, res, slot) => dc.ComputeShader.SetUnorderedAccessView(res.GetUnorderedAccessView(), slot));
		}
		
		public Vector3<uint> ThreadGroupCount { get; set; }

		public IResourcePool<HardwareConstantBuffer> Constants => mConstants;
		public IResourcePool<HardwareTexture> Textures => mTextures;
		public IResourcePool<HardwareUnorderedAccessBuffer> Outputs => mOutputs;
		
		IResourcePool<IWritable> IComputationKernel.Constants => mConstants;
		IResourcePool<IWritable> IComputationKernel.Textures => mTextures;
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

		protected override void Deallocate()
		{
			mConstants.Clear();
			mTextures.Clear();
			mOutputs.Clear();
		}
		protected override string GetProfileName() => "cs_5_0";
	}
}