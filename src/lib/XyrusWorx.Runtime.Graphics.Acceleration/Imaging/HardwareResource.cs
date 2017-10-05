using System;
using JetBrains.Annotations;
using SlimDX.Direct3D11;

namespace XyrusWorx.Runtime.Imaging 
{
	[PublicAPI]
	public abstract class HardwareResource : Resource
	{
		private Device mDevice;
		private AccelerationDevice mRegistry;

		internal HardwareResource([NotNull] AccelerationDevice device)
		{
			if (device == null)
			{
				throw new ArgumentNullException(nameof(device));
			}

			mRegistry = device;
			mDevice = mRegistry.GetDevice();
			
			device.AddResource(this);
		}

		protected sealed override void DisposeOverride()
		{
			try
			{
				DisposeResource();
			}
			finally
			{
				mRegistry?.RemoveResource(this);
				mRegistry = null;
				mDevice = null;
			}
		}
		protected virtual void DisposeResource(){}
		
		[NotNull]
		protected IDeviceContext Context => mRegistry;

		[NotNull]
		internal Device Device
		{
			get => mDevice ?? throw new ObjectDisposedException(GetType().Name);
		}
	}
}