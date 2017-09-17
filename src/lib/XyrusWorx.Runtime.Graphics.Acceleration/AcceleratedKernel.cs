using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using XyrusWorx.Runtime.Graphics.IO;
using XyrusWorx.Runtime.IO;

namespace XyrusWorx.Runtime.Graphics
{
	[PublicAPI]
	public abstract class AcceleratedKernel : Resource
	{
		private readonly AcceleratedComputationProvider mProvider;
		private bool mIsInitialized;

		protected AcceleratedKernel([NotNull] AcceleratedComputationProvider provider, [NotNull] AcceleratedKernelBytecode bytecode)
		{
			if (provider == null)
			{
				throw new ArgumentNullException(nameof(provider));
			}

			if (bytecode == null)
			{
				throw new ArgumentNullException(nameof(bytecode));
			}

			mProvider = provider;

			Constants = new ConstantResourceList(this);
			Bytecode = bytecode;

			Initialize();
		}

		public IList<IStructuredReadWriteBuffer> Constants { get; }
		public AcceleratedKernelBytecode Bytecode { get; }

		protected AcceleratedComputationProvider Provider => mProvider;

		protected abstract void OnInitialize();
		protected abstract void OnDestroy();

		protected abstract void SetConstant(StructuredHardwareResource constant, int address);

		private void Initialize()
		{
			Destroy();
			OnInitialize();
			mIsInitialized = true;
		}
		private void Destroy()
		{
			OnDestroy();
			mIsInitialized = false;
		}

		class ConstantResourceList : AcceleratedKernelResourceList<IStructuredReadWriteBuffer>
		{
			private readonly AcceleratedKernel mParent;

			public ConstantResourceList(AcceleratedKernel parent)
			{
				mParent = parent;
			}

			protected override void SetElement(IStructuredReadWriteBuffer item, int index)
			{
				mParent.SetConstant(item.CastTo<StructuredHardwareResource>(), index);
			}
		}
	}
}
