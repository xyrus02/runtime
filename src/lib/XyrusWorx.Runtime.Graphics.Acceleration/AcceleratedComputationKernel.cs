using System;
using JetBrains.Annotations;
using XyrusWorx.Runtime.Computation;

namespace XyrusWorx.Runtime.Graphics 
{
	[PublicAPI]
	public sealed class AcceleratedComputationKernel : AcceleratedKernel, IComputationKernel
	{
		private ComputeShaderConstantBufferList mConstants;
		private ComputeShaderResourceList mTextures;
		private ComputeShaderUnorderedAccessList mOutputs;

		private AcceleratedComputationKernel([NotNull] AccelerationDevice device) : base(device)
		{
			mConstants = new ComputeShaderConstantBufferList(this);
			mTextures = new ComputeShaderResourceList(this);
			mOutputs = new ComputeShaderUnorderedAccessList(this);
		}
		
		public Vector3<uint> ThreadGroupCount { get; set; }
		public IResourcePool<IWritable> Constants => mConstants;
		public IResourcePool<IWritable> Textures => mTextures;
		public IResourcePool<IReadable> Outputs => mOutputs;
		
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
		public static AcceleratedComputationKernel FromSource([NotNull] AccelerationDevice device, [NotNull] AcceleratedKernelSourceWriter source, CompilerContext context = null)
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