using System;
using JetBrains.Annotations;
using SlimDX.Direct3D11;
using SlimDX.DXGI;

namespace XyrusWorx.Runtime.Graphics.IO
{
	[PublicAPI]
	public class ImageOutputBuffer : HardwareBufferResource
	{
		private AcceleratedComputationProvider mProvider;
		private SwapChain mSwapChain;
		private Texture2D mHardwareBuffer;
		private RenderTargetView mView;

		private int mArrayWidth;
		private int mArrayHeight;

		internal ImageOutputBuffer([NotNull] AcceleratedComputationProvider provider, [NotNull] SwapChain swapChain)
		{
			if (provider == null)
			{
				throw new ArgumentNullException(nameof(provider));
			}
			
			if (swapChain == null)
			{
				throw new ArgumentNullException(nameof(swapChain));
			}

			mHardwareBuffer = SlimDX.Direct3D11.Resource.FromSwapChain<Texture2D>(swapChain, 0);
			mView = new RenderTargetView(provider.HardwareDevice, mHardwareBuffer);

			mProvider = provider;
			mSwapChain = swapChain;
			mArrayWidth = mHardwareBuffer.Description.Width;
			mArrayHeight = mHardwareBuffer.Description.Height;
		}

		public sealed override int BufferSize => mArrayWidth * mArrayHeight * 4;
		public sealed override int ElementCount => mArrayWidth * mArrayHeight;

		public int Stride => mArrayWidth * 4;
		public int Width => mArrayWidth;
		public int Height => mArrayHeight;

		internal Texture2D HardwareBuffer => mHardwareBuffer;
		internal RenderTargetView View => mView;

		protected override void OnCleanup()
		{
			mView?.Dispose();
			mHardwareBuffer?.Dispose();

			base.OnCleanup();
		}
	}
}