using System;
using JetBrains.Annotations;
using SlimDX.Direct3D11;
using SlimDX.DXGI;
using Device = SlimDX.Direct3D11.Device;

namespace XyrusWorx.Runtime.Imaging
{

	[PublicAPI]
	public class HardwareRenderTarget : Resource, IReadable
	{
		private Device mDevice;
		private Texture2D mTexture;
		private RenderTargetView mRenderTargetView;

		internal HardwareRenderTarget([NotNull] AccelerationDevice device, [NotNull] SwapChain swapChain)
		{
			if (device == null)
			{
				throw new ArgumentNullException(nameof(device));
			}
			
			if (swapChain == null)
			{
				throw new ArgumentNullException(nameof(swapChain));
			}

			mDevice = device.GetDevice();
			mTexture = SlimDX.Direct3D11.Resource.FromSwapChain<Texture2D>(swapChain, 0);
			mRenderTargetView = new RenderTargetView(mDevice, mTexture);
		}

		[Pure, NotNull]
		public HardwareRenderTargetData LockBits() => new HardwareRenderTargetData(this);
		
		internal Device Device => mDevice;
		
		internal Texture2D GetTexture2D() => mTexture;
		internal RenderTargetView GetRenderTargetView() => mRenderTargetView;

		protected override void DisposeOverride()
		{
			mRenderTargetView?.Dispose();
			mTexture?.Dispose();

			mRenderTargetView = null;
			mTexture = null;
			mDevice = null;
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