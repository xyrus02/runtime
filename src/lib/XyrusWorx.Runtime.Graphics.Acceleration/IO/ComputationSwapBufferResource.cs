using System;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using SlimDX.Direct3D11;
using XyrusWorx.Runtime.IO;
using D3DBuffer = SlimDX.Direct3D11.Buffer;

namespace XyrusWorx.Runtime.Graphics.IO
{
	[PublicAPI]
	public abstract class ComputationSwapBufferResource : StructuredHardwareBufferResource, IStructuredReadOnlyBuffer
	{
		public abstract void Read(IntPtr buffer, int index, int count);
		public abstract TElement Read<TElement>(int index = 0) where TElement : struct;
	}

	[PublicAPI]
	public class ComputationSwapBufferResource<T> : ComputationSwapBufferResource where T : struct
	{
		private ComputationDataStream<T> mBufferedReadStream;
		private readonly AcceleratedComputationProvider mProvider;
		private readonly D3DBuffer mHardwareBuffer;

		private int mArrayLength;
		private int mElementSize;

		public ComputationSwapBufferResource([NotNull] AcceleratedComputationProvider provider, int arrayLength)
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

			var description = new BufferDescription
			{
				BindFlags = BindFlags.None,
				CpuAccessFlags = CpuAccessFlags.Read | CpuAccessFlags.Write,
				OptionFlags = ResourceOptionFlags.StructuredBuffer,
				SizeInBytes = mArrayLength * mElementSize,
				StructureByteStride = mElementSize,
				Usage = ResourceUsage.Staging
			};

			mHardwareBuffer = new D3DBuffer(provider.HardwareDevice, description);
		}

		public ComputationDataStream<T> Read()
		{
			if (mBufferedReadStream != null)
			{
				return mBufferedReadStream;
			}

			var mappedResource = mProvider.HardwareDevice.ImmediateContext.MapSubresource(mHardwareBuffer, MapMode.Read, MapFlags.None);
			var streamSource = mappedResource.Data;

			return mBufferedReadStream = new ComputationDataStream<T>(mProvider, streamSource, mHardwareBuffer, 0);
		}
		public ComputationDataStream<T> Write()
		{
			var mappedResource = mProvider.HardwareDevice.ImmediateContext.MapSubresource(mHardwareBuffer, MapMode.Write, MapFlags.None);
			var streamSource = mappedResource.Data;

			return new ComputationDataStream<T>(mProvider, streamSource, mHardwareBuffer, 0);
		}

		public void FetchResource([NotNull] StructuredHardwareBufferResource source)
		{
			if (source == null)
			{
				throw new ArgumentNullException(nameof(source));
			}

			mProvider.HardwareDevice.ImmediateContext.CopyResource(source.HardwareBuffer, mHardwareBuffer);
			mBufferedReadStream = null;
		}
		public void SendResource([NotNull] StructuredHardwareBufferResource target)
		{
			if (target == null)
			{
				throw new ArgumentNullException(nameof(target));
			}

			mProvider.HardwareDevice.ImmediateContext.CopyResource(mHardwareBuffer, target.HardwareBuffer);
		}

		public sealed override int BufferSize => mArrayLength*mElementSize;
		public sealed override int ElementCount => mArrayLength;

		internal sealed override D3DBuffer HardwareBuffer => mHardwareBuffer;
		internal sealed override ShaderResourceView View => null;

		public override void Read(IntPtr buffer, int index, int count)
		{
			using (var stream = Read())
			{
				var sizeOfElement = BufferSize / ElementCount;
				var offset = stream.Data + index * sizeOfElement;
				CopyMemory(buffer, offset, sizeOfElement * count);
			}
		}
		public override TElement Read<TElement>(int index = 0)
		{
			if (index < 0 || index >= ElementCount)
			{
				throw new IndexOutOfRangeException();
			}

			var sizeOfTarget = Marshal.SizeOf<TElement>();
			var sizeOfElement = BufferSize / ElementCount;

			if (sizeOfTarget != sizeOfElement)
			{
				throw new ArgumentException($"The type \"{typeof(TElement).FullName}\" does not have the expected size of {sizeOfElement} bytes");
			}

			using (var stream = Read())
			{
				return Marshal.PtrToStructure<TElement>(stream.Data + index * sizeOfElement);
			}
		}
	}
}