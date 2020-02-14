using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using JetBrains.Annotations;
using XyrusWorx.Runtime.Computation;
using XyrusWorx.Runtime.Imaging;

namespace XyrusWorx.Runtime
{
	[PublicAPI]
	public abstract class AcceleratedReactor : IReactor
	{
		private readonly AccelerationDevice mProvider;
		private readonly IList<IComputationKernel> mHardwareQueue;
		private readonly AcceleratedReactorContext mContext;
		private AcceleratedImagingKernel mOutputKernel;
		private IWritableMemory mOutputTextureMemory;
		private TextureView mOutputTexture;
		private PlaceholderVectorBuffer mVectorBuffer;

		protected AcceleratedReactor([NotNull] AccelerationDevice provider)
		{
			mProvider = provider;
			mHardwareQueue = new List<IComputationKernel>();
			mContext = new AcceleratedReactorContext(this);
			mVectorBuffer = new PlaceholderVectorBuffer();
		}
		
		public IReadWriteTexture BackBuffer => mOutputTexture;
		public IVectorBuffer VectorBuffer => mVectorBuffer;

		void IReactor.SetBackBuffer(IWritableMemory writableMemory, TextureFormat pixelFormat, int stride)
		{
			foreach (var kernel in mHardwareQueue)
			{
				kernel.CastTo<IDisposable>()?.Dispose();
			}

			mHardwareQueue.Clear();
			mOutputKernel?.Dispose();

			if (writableMemory.Size <= 0)
			{
				throw new InvalidOperationException("The buffer dimensions are invalid.");
			}

			mOutputTextureMemory = writableMemory;
			mOutputTexture = new TextureView(writableMemory, stride, pixelFormat);

			CreateComputeKernels(mHardwareQueue);

			var outputKernel = CreateOutputKernel();
			if (outputKernel == null)
			{
				throw new ArgumentException("The imaging kernel is null.");
			}

			mOutputKernel = outputKernel;
		}

		public void InvalidateState()
		{
			
		}
		public void Update(IRenderLoop renderLoop)
		{
			UpdateOverride(renderLoop, mContext);
			
			foreach (var kernel in mHardwareQueue ?? new AcceleratedComputationKernel[0])
			{
				kernel.Execute();
			}

			if (mOutputKernel != null)
			{
				mOutputKernel.Execute();
				mOutputKernel.Output.Read(mOutputTextureMemory.GetPointer(), 0, mOutputTextureMemory.Size);
			}
		}
		public void Dispose()
		{
			try
			{
				DisposeOverride();
			}
			finally
			{
				foreach (var kernel in mHardwareQueue)
				{
					kernel.CastTo<IDisposable>()?.Dispose();
				}

				mHardwareQueue.Clear();
				mOutputKernel?.Dispose();
			}
		}
		
		protected virtual void UpdateOverride([NotNull] IRenderLoop renderLoop, [NotNull] IAcceleratedReactorContext context){}
		protected virtual void DisposeOverride(){}

		[NotNull]
		protected AccelerationDevice ComputationProvider => mProvider;
		
		[NotNull]
		protected abstract AcceleratedImagingKernel CreateOutputKernel();
		protected virtual void CreateComputeKernels([NotNull] IList<IComputationKernel> computationQueue){}
		
		class AcceleratedReactorContext : IAcceleratedReactorContext
		{
			private readonly AcceleratedReactor mReactor;
			private ReadOnlyCollection<IComputationKernel> mComputationKernels;

			public AcceleratedReactorContext([NotNull] AcceleratedReactor reactor)
			{
				if (reactor == null)
				{
					throw new ArgumentNullException(nameof(reactor));
				}
				
				mReactor = reactor;
				mComputationKernels = new ReadOnlyCollection<IComputationKernel>(mReactor.mHardwareQueue);
			}

			public IImagingKernel ImagingKernel => mReactor.mOutputKernel;
			public IReadOnlyList<IComputationKernel> ComputationQueue => mComputationKernels;
		}
	}
}
