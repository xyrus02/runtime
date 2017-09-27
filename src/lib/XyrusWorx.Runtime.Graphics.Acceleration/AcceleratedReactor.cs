using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using XyrusWorx.Runtime.Graphics.Computation;
using XyrusWorx.Runtime.Graphics.Imaging;
using XyrusWorx.Runtime.IO;

namespace XyrusWorx.Runtime.Graphics
{

	[PublicAPI]
	public abstract class AcceleratedReactor : IReactor
	{
		private readonly AcceleratedComputationProvider mProvider;
		private readonly IList<IComputationKernel> mHardwareQueue;
		private readonly AcceleratedReactorContext mContext;
		private IImagingKernel mOutputKernel;
		private IntPtr mBackBuffer;

		protected AcceleratedReactor([NotNull] AcceleratedComputationProvider provider)
		{
			mProvider = provider;
			mHardwareQueue = new List<IComputationKernel>();
			mContext = new AcceleratedReactorContext(this);
		}
		
		public abstract int BackBufferWidth { get; }
		public abstract int BackBufferHeight { get; }
		
		public void InvalidateState()
		{
			foreach (var kernel in mHardwareQueue)
			{
				kernel.CastTo<IDisposable>()?.Dispose();
			}

			mHardwareQueue.Clear();
			mOutputKernel?.Dispose();

			if (BackBufferWidth <= 0 || BackBufferHeight <= 0)
			{
				throw new InvalidOperationException("The buffer dimensions are invalid.");
			}
			
			if (mBackBuffer == IntPtr.Zero)
			{
				if (BackBufferWidth <= 0 || BackBufferHeight <= 0)
				{
					throw new InvalidOperationException("The buffer dimensions are invalid.");
				}

				mBackBuffer = Marshal.AllocHGlobal(new IntPtr((long)((IReactor)this).BackBufferStride * BackBufferHeight));
			}
			
			CreateComputeKernels(mHardwareQueue);

			var outputKernel = CreateOutputKernel();
			if (outputKernel == null)
			{
				throw new ArgumentException("The imaging kernel is null.");
			}
			
			mOutputKernel = outputKernel;
		}
		public void Update(IRenderLoop renderLoop)
		{
			BeforeCompute(renderLoop, mContext);
			
			foreach (var kernel in mHardwareQueue ?? new AcceleratedComputationKernel[0])
			{
				kernel.Compute();
			}

			var stream = mOutputKernel?.Compute(BackBufferWidth, BackBufferHeight);
			if (stream == null)
			{
				return;
			}

			using (stream)
			{
				AfterCompute(renderLoop, mContext, stream);
				stream.Seek(0, SeekOrigin.Begin);

				const int chunkSize = 65536;

				var buffer = new byte[chunkSize];
				var readBytes = stream.Read(buffer, 0, chunkSize);
				var backBufferOffset = 0;
			
				while (readBytes > 0)
				{
					Marshal.Copy(buffer, 0, mBackBuffer + backBufferOffset, readBytes);
					backBufferOffset += readBytes;
					readBytes = stream.Read(buffer, 0, chunkSize);
				}
			}
		}

		protected virtual void BeforeCompute([NotNull] IRenderLoop renderLoop, [NotNull] IAcceleratedReactorContext context){}
		protected virtual void AfterCompute([NotNull] IRenderLoop renderLoop, [NotNull] IAcceleratedReactorContext context, [NotNull] IDataStream<Vector4<byte>> outputStream){}

		public void Dispose()
		{
			try
			{
				DisposeOverride();
			}
			finally
			{
				if (mBackBuffer != IntPtr.Zero)
				{
					Marshal.FreeHGlobal(mBackBuffer);
					mBackBuffer = IntPtr.Zero;
				}
			
				foreach (var kernel in mHardwareQueue)
				{
					kernel.CastTo<IDisposable>()?.Dispose();
				}

				mHardwareQueue.Clear();
				mOutputKernel?.Dispose();
			}
		}
		
		protected virtual void DisposeOverride(){}

		[NotNull]
		protected AcceleratedComputationProvider ComputationProvider => mProvider;
		
		[NotNull]
		protected abstract IImagingKernel CreateOutputKernel();
		protected virtual void CreateComputeKernels([NotNull] IList<IComputationKernel> computationQueue){}
		
		IntPtr IReactor.BackBuffer => mBackBuffer;
		int IReactor.BackBufferStride => BackBufferWidth << 2;

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
