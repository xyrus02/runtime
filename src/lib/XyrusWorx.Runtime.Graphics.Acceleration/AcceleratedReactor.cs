using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using XyrusWorx.Runtime.IO;

namespace XyrusWorx.Runtime.Graphics
{
	[PublicAPI]
	public abstract class AcceleratedReactor : IReactor
	{
		private readonly AcceleratedComputationProvider mProvider;
		private readonly IList<AcceleratedComputationKernel> mHardwareQueue;
		private AcceleratedImagingKernel mOutputKernel;
		private IntPtr mBackBuffer;

		protected AcceleratedReactor([NotNull] AcceleratedComputationProvider provider)
		{
			mProvider = provider;
			mHardwareQueue = new List<AcceleratedComputationKernel>();
		}
		
		public void InvalidateState()
		{
			foreach (var kernel in mHardwareQueue)
			{
				kernel.Dispose();
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
			BeforeCompute(renderLoop);
			
			foreach (var kernel in mHardwareQueue ?? new AcceleratedComputationKernel[0])
			{
				kernel.Compute();
			}

			var stream = mOutputKernel?.Compute(BackBufferWidth, BackBufferHeight);
			if (stream == null)
			{
				return;
			}
			
			AfterCompute(renderLoop, stream);
			stream.Seek(0, SeekOrigin.Begin);

			const int chunkSize = 65536;

			var buffer = new byte[chunkSize];
			var readBytes = stream.Read(buffer, 0, chunkSize);
			
			while (readBytes > 0)
			{
				Marshal.Copy(buffer, 0, mBackBuffer, readBytes);
				readBytes = stream.Read(buffer, 0, chunkSize);
			}
		}

		protected virtual void BeforeCompute(IRenderLoop renderLoop){}
		protected virtual void AfterCompute(IRenderLoop renderLoop, IDataStream<Vector4<byte>> outputStream){}

		public void Dispose()
		{
			if (mBackBuffer != IntPtr.Zero)
			{
				Marshal.FreeHGlobal(mBackBuffer);
				mBackBuffer = IntPtr.Zero;
			}
			
			foreach (var kernel in mHardwareQueue)
			{
				kernel.Dispose();
			}

			mHardwareQueue.Clear();
			mOutputKernel?.Dispose();
		}

		[NotNull]
		protected AcceleratedComputationProvider ComputationProvider => mProvider;
		
		[NotNull]
		protected abstract AcceleratedImagingKernel CreateOutputKernel();
		protected abstract void CreateComputeKernels([NotNull] IList<AcceleratedComputationKernel> computationQueue);
		
		IntPtr IReactor.BackBuffer => mBackBuffer;
		int IReactor.BackBufferStride => BackBufferWidth << 2;
		
		public abstract int BackBufferWidth { get; }
		public abstract int BackBufferHeight { get; }
	}
}
