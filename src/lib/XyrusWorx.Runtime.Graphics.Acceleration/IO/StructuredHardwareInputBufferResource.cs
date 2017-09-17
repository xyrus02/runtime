using System;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using SlimDX;
using SlimDX.Direct3D11;
using XyrusWorx.Runtime.IO;
using D3DBuffer = SlimDX.Direct3D11.Buffer;

namespace XyrusWorx.Runtime.Graphics.IO
{
	[PublicAPI]
	public abstract class StructuredHardwareInputBufferResource : StructuredHardwareBufferResource, IStructuredWriteOnlyBuffer
	{
		public abstract void Write<T>(T data, int index = 0) where T : struct;
	}

	[PublicAPI]
	public class StructuredHardwareInputBufferResource<T> : StructuredHardwareInputBufferResource where T : struct
	{
		private readonly AcceleratedComputationProvider mDevice;
		private readonly ShaderResourceView mView;
		private readonly D3DBuffer mHardwareBuffer;

		private int mArrayLength;
		private int mElementSize;

		public StructuredHardwareInputBufferResource([NotNull] AcceleratedComputationProvider device, int arrayLength) : this(device, arrayLength, IntPtr.Zero)
		{
		}
		public StructuredHardwareInputBufferResource([NotNull] AcceleratedComputationProvider device, int arrayLength, IntPtr sourcePointer)
		{
			if (device == null)
			{
				throw new ArgumentNullException(nameof(device));
			}
			
			if (arrayLength <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(arrayLength));
			}

			mDevice = device;
			mArrayLength = arrayLength;
			mElementSize = Marshal.SizeOf(typeof (T));

			var description = new BufferDescription
			{
				BindFlags = BindFlags.ShaderResource,
				CpuAccessFlags = CpuAccessFlags.Write,
				OptionFlags = ResourceOptionFlags.StructuredBuffer,
				SizeInBytes = mArrayLength * mElementSize,
				StructureByteStride = mElementSize,
				Usage = ResourceUsage.Dynamic
			};

			if (sourcePointer != IntPtr.Zero)
			{
				var stream = new DataStream(sourcePointer, description.SizeInBytes, true, false);

				stream.Position = 0;
				mHardwareBuffer = new D3DBuffer(device.HardwareDevice, stream, description);
			}
			else
			{
				mHardwareBuffer = new D3DBuffer(device.HardwareDevice, description);
			}

			mView = new ShaderResourceView(device.HardwareDevice, mHardwareBuffer);
		}

		public sealed override int BufferSize => mElementSize * mArrayLength;
		public sealed override int ElementCount => mArrayLength;

		internal sealed override D3DBuffer HardwareBuffer => mHardwareBuffer;
		internal sealed override ShaderResourceView View => mView;

		public void Write(T[] data)
		{
			Write(data, 0, data.Length);
		}
		public void Write(T[] data, int offset, int count)
		{
			var context = mDevice.HardwareDevice.ImmediateContext;
			var box = context.MapSubresource(mHardwareBuffer, MapMode.WriteDiscard, MapFlags.None);

			box.Data.WriteRange(data, offset, count);
			context.UnmapSubresource(mHardwareBuffer, 0);
		}
		public void Write(IntPtr ptr)
		{
			var context = mDevice.HardwareDevice.ImmediateContext;
			var box = context.MapSubresource(mHardwareBuffer, MapMode.WriteDiscard, MapFlags.None);

			box.Data.WriteRange(ptr, mArrayLength * mElementSize);
			context.UnmapSubresource(mHardwareBuffer, 0);
		}

		public override void Write<TElement>(TElement data, int index = 0)
		{
			if (index < 0 || index >= ElementCount)
			{
				throw new IndexOutOfRangeException();
			}

			var sizeOfTarget = Marshal.SizeOf<TElement>();
			var sizeOfElement = mElementSize;

			if (sizeOfTarget != sizeOfElement)
			{
				throw new ArgumentException($"The type \"{typeof(TElement).FullName}\" does not have the expected size of {sizeOfElement} bytes");
			}

			var memory = Marshal.AllocHGlobal(sizeOfElement);
			Marshal.StructureToPtr(data, memory, false);

			try
			{
				Write(memory);
			}
			finally
			{
				Marshal.FreeHGlobal(memory);
			}
		}

		protected override void OnCleanup()
		{
			mView?.Dispose();
			base.OnCleanup();
		}
	}
}