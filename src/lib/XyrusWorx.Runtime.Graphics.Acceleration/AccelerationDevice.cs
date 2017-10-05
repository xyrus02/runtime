using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SlimDX.Direct3D11;
using SlimDX.DXGI;
using XyrusWorx.Diagnostics;
using XyrusWorx.Runtime.Imaging;
using Device = SlimDX.Direct3D11.Device;

namespace XyrusWorx.Runtime
{
	[PublicAPI]
	public sealed class AccelerationDevice : Resource
	{
		private Device mDevice;
		private Factory mFactory;
		private HashSet<HardwareResource> mResources;
		private bool mIsDebugModeEnabled;

		public AccelerationDevice(ILogWriter diagnosticsWriter = null)
		{
			mFactory = new Factory();
			mResources = new HashSet<HardwareResource>();
			DiagnosticsWriter = diagnosticsWriter ?? new NullLogWriter();

			#if DEBUG
			mIsDebugModeEnabled = true;
			#endif
		}

		[NotNull]
		public ILogWriter DiagnosticsWriter { get; }

		public bool IsDebugModeEnabled
		{
			get { return mIsDebugModeEnabled; }
			set
			{
				if (Equals(value, mIsDebugModeEnabled))
				{
					return;
				}

				if (mDevice != null)
				{
					throw new InvalidOperationException("Debug mode can't be enabled or disabled after connecting to computation device.");
				}
				
				mIsDebugModeEnabled = value;
			}
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

		internal void AddResource([NotNull] HardwareResource resource)
		{
			if (resource == null)
			{
				throw new ArgumentNullException(nameof(resource));
			}

			if (mResources == null)
			{
				throw new ObjectDisposedException(nameof(AccelerationDevice), "The device is not available anymore.");
			}

			DiagnosticsWriter.WriteDebug("Associating resource \"{0}\" with device.", resource);
			mResources.Add(resource);
		}
		internal void RemoveResource([NotNull] HardwareResource resource)
		{
			if (resource == null)
			{
				throw new ArgumentNullException(nameof(resource));
			}

			mResources?.Remove(resource);
			DiagnosticsWriter.WriteDebug("Removed association of resource \"{0}\" from device.", resource);
		}

		protected override void DisposeOverride()
		{
			DiagnosticsWriter.WriteInformation("Disconnecting from device");
			
			var resources = mResources?.ToArray() ?? new HardwareResource[0];
			if (resources.Length > 0)
			{
				DiagnosticsWriter.WriteWarning("The is being disconnected from but there are still {0} associated resource(s) allocated. The resources will be disposed to avoid memory leaks", resources.Length);
				
				foreach (var resource in resources)
				{
					resource?.Dispose();
				}
			}

			mResources?.Clear();
			mResources = null;
			
			mDevice?.Dispose();
			mFactory?.Dispose();
			
			mDevice = null;
			mFactory = null;
			
			DiagnosticsWriter.WriteInformation("Disconnection completed.");
		}
		
		private void EnsureDevicePresent()
		{
			if (mDevice != null)
			{
				return;
			}
			
			DiagnosticsWriter.WriteInformation("Connecting to computation device");

			var flags = mIsDebugModeEnabled 
				? DeviceCreationFlags.Debug 
				: DeviceCreationFlags.None; 

			if (flags.HasFlag(DeviceCreationFlags.Debug))
			{
				DiagnosticsWriter.WriteWarning("Computations are being run in debug mode. Performance will degrade.");
			}
			
			var adapterCount = mFactory.GetAdapterCount();
			var error = (Exception)null;

			for (var i = 0; i < adapterCount; i++)
			{
				var adapter = mFactory.GetAdapter(i);
				var featureLevel = Device.GetSupportedFeatureLevel(adapter);

				if (featureLevel < FeatureLevel.Level_11_0)
				{
					DiagnosticsWriter.WriteDebug("Skipping \"{0}\" because of an insufficient feature level ({1} < {2})", 
						adapter.Description.Description,
						featureLevel.ToString(),
						nameof(FeatureLevel.Level_11_0));
					
					continue;
				}

				try
				{
					mDevice = new Device(adapter, flags, FeatureLevel.Level_11_0);
					DiagnosticsWriter.WriteInformation("Connection to device established: {0}", adapter.Description.Description);
				}
				catch (Exception ex)
				{
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

				throw new NotSupportedException("No suitable adapter could be found.");
			}
		}
	}
}