using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SlimDX.Direct3D11;
using XyrusWorx.Runtime.Graphics.IO;
using XyrusWorx.Runtime.IO;

namespace XyrusWorx.Runtime.Graphics
{
	[PublicAPI]
	public class AcceleratedComputationKernel : AcceleratedKernel, IComputationKernel
	{
		private ComputeShader mComputeShader;

		private int mThreadGroupCountX;
		private int mThreadGroupCountY;
		private int mThreadGroupCountZ;

		public AcceleratedComputationKernel([NotNull] AcceleratedComputationProvider provider, [NotNull] AcceleratedKernelBytecode bytecode) : base(provider, bytecode)
		{
			if (bytecode.KernelType != AcceleratedComputationKernelType.ComputeShader)
			{
				throw new ArgumentException("A compute shader kernel source was expected.");
			}

			ThreadGroupCountX = 32;
			ThreadGroupCountY = 32;
			ThreadGroupCountZ = 1;

			Resources = new ShaderResourceList(this);
			Outputs = new UnorderedAccessResourceList(this);
		}

		public IList<IStructuredBuffer> Resources { get; }
		public IList<IDeviceBuffer> Outputs { get; }

		public int ThreadGroupCountX
		{
			get { return mThreadGroupCountX; }
			set
			{
				if (value <= 0 || value > 1024)
				{
					throw new ArgumentOutOfRangeException(nameof(value));
				}
				mThreadGroupCountX = value;
			}
		}
		public int ThreadGroupCountY
		{
			get { return mThreadGroupCountY; }
			set
			{
				if (value <= 0 || value > 1024)
				{
					throw new ArgumentOutOfRangeException(nameof(value));
				}
				mThreadGroupCountY = value;
			}
		}
		public int ThreadGroupCountZ
		{
			get { return mThreadGroupCountZ; }
			set
			{
				if (value <= 0 || value > 1024)
				{
					throw new ArgumentOutOfRangeException(nameof(value));
				}
				mThreadGroupCountZ = value;
			}
		}
		
		protected override void OnInitialize()
		{
			var device = Provider.HardwareDevice;
			var context = device.ImmediateContext;

			mComputeShader = new ComputeShader(device, Bytecode.HardwareBytecode);
			context.ComputeShader.Set(mComputeShader);
		}
		protected override void OnDestroy()
		{
			Constants?.Clear();
			Resources?.Clear();
			Outputs?.Clear();

			Provider?.HardwareDevice.ImmediateContext.ComputeShader.Set(null);

			mComputeShader?.Dispose();
			mComputeShader = null;
		}

		protected override void SetConstant(StructuredHardwareResource constant, int address)
		{
			Provider.HardwareDevice.ImmediateContext.ComputeShader.SetConstantBuffer(constant?.HardwareBuffer, address);
		}

		public void Compute()
		{
			Provider.HardwareDevice.ImmediateContext.Dispatch(ThreadGroupCountX, ThreadGroupCountY, ThreadGroupCountZ);
		}

		class ShaderResourceList : AcceleratedKernelResourceList<IStructuredBuffer>
		{
			private readonly AcceleratedComputationKernel mParent;

			public ShaderResourceList(AcceleratedComputationKernel parent)
			{
				mParent = parent;
			}

			protected override void SetElement(IStructuredBuffer item, int index)
			{
				var rv = new[]
				{
					item.CastTo<HardwareTexture2D>()?.ResourceView,
					item.CastTo<StructuredHardwareBufferResource>()?.View
				};

				var rvv = rv.FirstOrDefault();
				
				mParent.Provider.HardwareDevice.ImmediateContext.ComputeShader.SetShaderResource(rvv, index);
			}
		}
		class UnorderedAccessResourceList : AcceleratedKernelResourceList<IDeviceBuffer>
		{
			private readonly AcceleratedComputationKernel mParent;

			public UnorderedAccessResourceList(AcceleratedComputationKernel parent)
			{
				mParent = parent;
			}

			protected override void SetElement(IDeviceBuffer item, int index)
			{
				mParent.Provider.HardwareDevice.ImmediateContext.ComputeShader.SetUnorderedAccessView(item?.CastTo<StructuredHardwareOutputBufferResource>()?.AccessView, index);
			}
		}
	}
}