using System;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using SlimDX.Direct3D11;
using XyrusWorx.Runtime.IO;
using D3DBuffer = SlimDX.Direct3D11.Buffer;

namespace XyrusWorx.Runtime.Graphics.IO
{
	[PublicAPI]
	public abstract class StructuredHardwareResource : HardwareBufferResource, IStructuredReadWriteBuffer
	{
		public sealed override int ElementCount => 1;

		internal abstract D3DBuffer HardwareBuffer { get; }

		public void Write(IntPtr rawData, int byteOffset)
		{
			WriteUnmanaged(rawData, byteOffset);
		}
		public void Write<T>(T data, int index = 0) where T : struct
		{
			if (index < 0 || index >= ElementCount)
			{
				throw new IndexOutOfRangeException();
			}

			var sizeOfTarget = Marshal.SizeOf<T>();
			var sizeOfElement = BufferSize / ElementCount;

			if (sizeOfTarget != sizeOfElement)
			{
				throw new ArgumentException($"The type \"{typeof(T).FullName}\" does not have the expected size of {sizeOfElement} bytes");
			}

			var memory = Marshal.AllocHGlobal(sizeOfElement);

			Marshal.StructureToPtr(data, memory, false);
			WriteUnmanaged(memory, sizeOfElement * index);

			Marshal.FreeHGlobal(memory);
		}
		public T Read<T>(int index = 0) where T : struct
		{
			if (index < 0 || index >= ElementCount)
			{
				throw new IndexOutOfRangeException();
			}

			var sizeOfTarget = Marshal.SizeOf<T>();
			var sizeOfElement = BufferSize/ElementCount;

			if (sizeOfTarget != sizeOfElement)
			{
				throw new ArgumentException($"The type \"{typeof(T).FullName}\" does not have the expected size of {sizeOfElement} bytes");
			}

			var sourcePtr = ReadUnmanaged(index * sizeOfElement);
			if (sourcePtr == IntPtr.Zero)
			{
				throw new NotSupportedException("The buffer doesn't support copying structures to system memory.");
			}

			var result = Marshal.PtrToStructure<T>(sourcePtr);
			Marshal.FreeHGlobal(sourcePtr);
			return result;
		}

		protected abstract IntPtr ReadUnmanaged(int offset);
		protected abstract void WriteUnmanaged(IntPtr memory, int offset);

		protected override void OnCleanup()
		{
			HardwareBuffer?.Dispose();
		}
	}

	[PublicAPI]
	public class StructuredHardwareResource<T> : StructuredHardwareResource where T : struct
	{
		private readonly AcceleratedComputationProvider mProvider;
		private readonly D3DBuffer mHardwareBuffer;

		private int mElementSize;
		private T mData;

		public StructuredHardwareResource(AcceleratedComputationProvider provider, T data = default(T))
		{
			mProvider = provider;
			mElementSize = Marshal.SizeOf(typeof(T));

			var bufferSize = Math.Max(16, (mElementSize + 15) / 16 * 16);
			var description = new BufferDescription
			{
				BindFlags = BindFlags.ConstantBuffer,
				CpuAccessFlags = CpuAccessFlags.Write,
				OptionFlags = ResourceOptionFlags.None,
				SizeInBytes = bufferSize,
				Usage = ResourceUsage.Dynamic
			};

			mHardwareBuffer = new D3DBuffer(provider.HardwareDevice, description);

			Data = data;
		}
		public T Data
		{
			get => mData;
			set
			{
				if (Equals(value, mData))
				{
					return;
				}

				mData = value;

				var db = mProvider.HardwareDevice.ImmediateContext.MapSubresource(mHardwareBuffer, MapMode.WriteDiscard, MapFlags.None);
				db.Data.Write(value);
				mProvider.HardwareDevice.ImmediateContext.UnmapSubresource(mHardwareBuffer, 0);
			}
		}
		public sealed override int BufferSize => mElementSize;

		internal sealed override D3DBuffer HardwareBuffer => mHardwareBuffer;

		protected sealed override IntPtr ReadUnmanaged(int offset)
		{
			var mem = Marshal.AllocHGlobal(Marshal.SizeOf<T>());
			Marshal.StructureToPtr(mData, mem, false);
			return mem;
		}
		protected override void WriteUnmanaged(IntPtr memory, int offset)
		{
			mData = Marshal.PtrToStructure<T>(memory);
		}
	}
}