using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using XyrusWorx.Runtime.Graphics.IO;
using XyrusWorx.Runtime.IO;

namespace XyrusWorx.Runtime.Graphics
{
	[PublicAPI]
	public abstract class AcceleratedExecutionContext : Resource
	{
		private readonly AccelerationDevice mProvider;
		private bool mIsInitialized;

		protected AcceleratedExecutionContext([NotNull] AccelerationDevice provider, [NotNull] AcceleratedKernel bytecode)
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
		public AcceleratedKernel Bytecode { get; }

		protected AccelerationDevice Provider => mProvider;

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
			private readonly AcceleratedExecutionContext mParent;

			public ConstantResourceList(AcceleratedExecutionContext parent)
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
