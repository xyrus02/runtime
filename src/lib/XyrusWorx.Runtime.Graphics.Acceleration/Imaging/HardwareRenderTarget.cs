using System;
using JetBrains.Annotations;
using SlimDX.Direct3D11;
using SlimDX.DXGI;

namespace XyrusWorx.Runtime.Imaging
{
	[PublicAPI]
	public class HardwareRenderTarget : HardwareResource, IReadable
	{
		private Texture2D mTexture;
		private RenderTargetView mRenderTargetView;

		internal HardwareRenderTarget([NotNull] AccelerationDevice device, [NotNull] SwapChain swapChain) : base(device)
		{
			if (swapChain == null)
			{
				throw new ArgumentNullException(nameof(swapChain));
			}

			mTexture = SlimDX.Direct3D11.Resource.FromSwapChain<Texture2D>(swapChain, 0);
			mRenderTargetView = new RenderTargetView(Device, mTexture);
		}

		[Pure, NotNull]
		public HardwareRenderTargetData LockBits() => new HardwareRenderTargetData(this);
		
		internal Texture2D GetTexture2D() => mTexture;
		internal RenderTargetView GetRenderTargetView() => mRenderTargetView;

		protected override void DisposeResource()
		{
			mRenderTargetView?.Dispose();
			mTexture?.Dispose();

			mRenderTargetView = null;
			mTexture = null;
		}
		
		void IReadable.Read(IntPtr target, int readOffset, long bytesToRead)
		{
			using (var bits = LockBits())
			{
				bits.Read(target, readOffset, bytesToRead);
			}
		}
	}
}