using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using SlimDX.Direct3D11;
using SlimDX.DXGI;
using Device = SlimDX.Direct3D11.Device;

namespace XyrusWorx.Runtime.Graphics
{
	[PublicAPI]
	public class AcceleratedComputationProvider : Resource
	{
		private Device mDevice;
		private Factory mFactory;
		
		public AcceleratedComputationProvider()
		{
			mFactory = new Factory();
		}

		public void Initialize()
		{
			Create();
		}
		public async Task InitializeAsync()
		{
			await Task.Run(new Action(Initialize));
		}

		public void Invalidate()
		{
			Destroy();
		}
		public async Task InvalidateAsync()
		{
			await Task.Run(new Action(Invalidate));
		}

		internal Device HardwareDevice
		{
			get
			{
				if (mDevice == null)
				{
					Create();
				}

				return mDevice;
			}
		}
		internal SwapChain CreateSwapChain(SwapChainDescription description) => new SwapChain(mFactory, mDevice, description);

		protected virtual void OnCleanup(){}
		protected sealed override void DisposeOverride()
		{
			try
			{
				OnCleanup();
			}
			finally
			{
				Destroy();
				mFactory?.Dispose();
			}
		}

		private void Create()
		{
			Destroy();

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

				throw new Exception("Failed to create device.");
			}
		}
		private void Destroy()
		{
			mDevice?.Dispose();
			mDevice = null;
		}
	}
}