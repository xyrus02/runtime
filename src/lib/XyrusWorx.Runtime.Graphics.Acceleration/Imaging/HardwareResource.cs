using System;
using JetBrains.Annotations;
using SlimDX.Direct3D11;

namespace XyrusWorx.Runtime.Imaging 
{
	[PublicAPI]
	public abstract class HardwareResource : Resource
	{
		private Device mDevice;
		
		internal HardwareResource([NotNull] AccelerationDevice device)
		{
			if (device == null)
			{
				throw new ArgumentNullException(nameof(device));
			}
			
			mDevice = device.GetDevice();
		}

		protected sealed override void DisposeOverride()
		{
			try
			{
				DisposeResource();
			}
			finally
			{
				mDevice = null;
			}
		}
		protected virtual void DisposeResource(){}

		[NotNull]
		internal Device Device => mDevice;
		
		[NotNull]
		internal abstract ShaderResourceView GetShaderResourceView();
	}
}