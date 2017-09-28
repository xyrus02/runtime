using System;
using JetBrains.Annotations;
using SlimDX.Direct3D11;
using SlimDX.DXGI;
using Device = SlimDX.Direct3D11.Device;

namespace XyrusWorx.Runtime.Graphics
{
	[PublicAPI]
	public sealed class AccelerationDevice : Resource
	{
		private Device mDevice;
		private Factory mFactory;
		
		public AccelerationDevice()
		{
			mFactory = new Factory();
		}

		internal Device GetDevice()
		{
			if (mFactory == null)
			{
				throw new ObjectDisposedException(nameof(AccelerationDevice));
			}
			
			EnsureDevicePresent();
			return mDevice;
		}
		internal SwapChain CreateSwapChain(SwapChainDescription description)
		{
			if (mFactory == null)
			{
				throw new ObjectDisposedException(nameof(AccelerationDevice));
			}
			
			return new SwapChain(mFactory, mDevice, description);
		}

		private void EnsureDevicePresent()
		{
			if (mDevice != null)
			{
				return;
			}
			
			var flags =
				#if DEBUG
					DeviceCreationFlags.Debug;
				#else
					DeviceCreationFlags.None;
				#endif
			
			var adapterCount = mFactory.GetAdapterCount();
			var error = (Exception)null;

			for (var i = 0; i < adapterCount; i++)
			{
				var adapter = mFactory.GetAdapter(i);
				var featureLevel = Device.GetSupportedFeatureLevel(adapter);

				if (featureLevel < FeatureLevel.Level_11_0)
				{
					continue;
				}

				try
				{
					mDevice = new Device(adapter, flags, FeatureLevel.Level_11_0);
				}
				catch (Exception ex)
				{
					flags = DeviceCreationFlags.None;
					error = ex;
					continue;
				}

				break;
			}

			if (mDevice == null)
			{
				if (error != null)
				{
					throw new Exception(string.Format(error.Message, error));
				}

				throw new NotSupportedException("At least one device is required which supports Direct3D 11.0-features.");
			}
		}
		
		protected override void DisposeOverride()
		{
			mDevice?.Dispose();
			mFactory?.Dispose();
			
			mDevice = null;
			mFactory = null;
		}
	}
}