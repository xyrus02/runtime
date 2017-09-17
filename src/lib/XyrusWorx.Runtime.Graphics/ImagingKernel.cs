using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using JetBrains.Annotations;
using XyrusWorx.Runtime.IO;

namespace XyrusWorx.Runtime.Graphics
{
	[PublicAPI]
	public abstract class ImagingKernel : Resource, IImagingKernel
	{
		private ParallelOptions mParallelOptions;

		protected ImagingKernel()
		{
			mParallelOptions = new ParallelOptions();

			Constants = new List<IStructuredReadWriteBuffer>();
			Resources = new List<ITexture2D>();
		}

		[NotNull]
		public ParallelOptions ParallelOptions
		{
			get => mParallelOptions;
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException(nameof(value));
				}

				mParallelOptions = value;
			}
		}

		public IList<IStructuredReadWriteBuffer> Constants { get; }
		public IList<ITexture2D> Resources { get; }

		public IDataStream<Vector4<byte>> Compute(int arrayWidth, int arrayHeight)
		{
			if (arrayWidth <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(arrayWidth));
			}

			if (arrayHeight <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(arrayHeight));
			}

			var stream = new MemoryBlock<Vector4<byte>>(arrayWidth * arrayHeight * 4);
			var arraySize2D = new Int2(arrayWidth, arrayHeight);

			Parallel.For(0, arrayWidth*arrayHeight, mParallelOptions, i => ProcessPixel(i, stream.Data, arraySize2D));

			return stream;
		}

		protected abstract Float4 Main(KernelIterationContext source);
		private void ProcessPixel(int index, IntPtr memPtr, Int2 arraySize2D)
		{
			var position = new Int2(index % arraySize2D.x, index / arraySize2D.x);
			var psii = new KernelIterationContext(new Float2(position.x / (float)arraySize2D.x, position.y / (float)arraySize2D.y));

			var rgbaF32 = Main(psii);
			var bgraI32 = (rgbaF32 * 255f).Clamp(new Float4(), new Float4(255,255,255,255)).Int().zyxw;
			var bgraU8 = new Vector4<byte>((byte)bgraI32.x, (byte)bgraI32.y, (byte)bgraI32.z, (byte)bgraI32.w);

			Marshal.StructureToPtr(bgraU8, memPtr + index * 4, false);
		}
	}
}