using System;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using SlimDX.Direct3D11;
using XyrusWorx.Runtime.IO;
using D3DBuffer = SlimDX.Direct3D11.Buffer;

namespace XyrusWorx.Runtime.Graphics.IO
{
	[PublicAPI]
	public abstract class StructuredHardwareOutputBufferResource : StructuredHardwareBufferResource, IDeviceBuffer, IStructuredReadOnlyBuffer
	{
		internal abstract UnorderedAccessView AccessView { get; }

		protected override void OnCleanup()
		{
			AccessView?.Dispose();
		}

		public abstract void Read(IntPtr buffer, int index, int count);
		T IStructuredReadOnlyBuffer.Read<T>(int index)
		{
			if (index < 0 || index >= ElementCount)
			{
				throw new IndexOutOfRangeException();
			}
			
			var buffer = IntPtr.Zero;
			try
			{
				var sizeOfTarget = Marshal.SizeOf<T>();
				var sizeOfElement = BufferSize/ElementCount;

				if (sizeOfTarget != sizeOfElement)
				{
					throw new ArgumentException($"The type \"{typeof(T).FullName}\" does not have the expected size of {sizeOfElement} bytes");
				}

				buffer = Marshal.AllocHGlobal(sizeOfElement);
				Read(buffer, index * sizeOfElement, 1);

				return Marshal.PtrToStructure<T>(buffer);
				
			}
			finally
			{
				if (buffer != IntPtr.Zero)
				{
					Marshal.FreeHGlobal(buffer);
				}
			}
		}
	}

	[PublicAPI]
	public class StructuredHardwareOutputBufferResource<T> : StructuredHardwareOutputBufferResource where T : struct
	{
		private readonly AcceleratedComputationProvider mProvider;
		private readonly ShaderResourceView mView;
		private readonly UnorderedAccessView mAccessView;
		private readonly D3DBuffer mHardwareBuffer;

		private int mArrayLength;
		private int mElementSize;

		private StructuredBufferMode mMode;

		public StructuredHardwareOutputBufferResource([NotNull] AcceleratedComputationProvider provider, int arrayLength, StructuredBufferMode mode = StructuredBufferMode.Default)
		{
			if (provider == null)
			{
				throw new ArgumentNullException(nameof(provider));
			}
			
			if (arrayLength <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(arrayLength));
			}

			mProvider = provider;
			mArrayLength = arrayLength;
			mElementSize = Marshal.SizeOf(typeof(T));
			mMode = mode;

			var bufferDescription = new BufferDescription
			{
				BindFlags = BindFlags.ShaderResource | BindFlags.UnorderedAccess,
				CpuAccessFlags = CpuAccessFlags.None,
				OptionFlags = ResourceOptionFlags.StructuredBuffer,
				SizeInBytes = mArrayLength * mElementSize,
				StructureByteStride = mElementSize,
				Usage = ResourceUsage.Default
			};

			var viewDescription = new UnorderedAccessViewDescription
			{
				ElementCount = mArrayLength,
				Format = SlimDX.DXGI.Format.Unknown,
				Dimension = UnorderedAccessViewDimension.Buffer,
				Flags = (UnorderedAccessViewBufferFlags)mode
			};

			mHardwareBuffer = new D3DBuffer(provider.HardwareDevice, bufferDescription);
			mView = new ShaderResourceView(provider.HardwareDevice, mHardwareBuffer);
			mAccessView = new UnorderedAccessView(provider.HardwareDevice, mHardwareBuffer, viewDescription);
		}

		public sealed override int BufferSize => mElementSize * mArrayLength;
		public sealed override int ElementCount => mArrayLength;

		public StructuredBufferMode Mode => mMode;

		internal sealed override D3DBuffer HardwareBuffer => mHardwareBuffer;
		internal sealed override ShaderResourceView View => mView;
		internal sealed override UnorderedAccessView AccessView => mAccessView;

		public override void Read(IntPtr buffer, int index, int count)
		{
			using (var swap = new ComputationSwapBufferResource<T>(mProvider, ElementCount))
			{
				swap.FetchResource(this);
				swap.Read(buffer, index, count);
			}
		}
		
		public void Clear()
		{
			using (var input = new StructuredHardwareInputBufferResource<T>(mProvider, mArrayLength))
				mProvider.HardwareDevice.ImmediateContext.CopyResource(input.HardwareBuffer, mHardwareBuffer);
		}

		protected override void OnCleanup()
		{
			mView?.Dispose();
			base.OnCleanup();
		}
	}
}