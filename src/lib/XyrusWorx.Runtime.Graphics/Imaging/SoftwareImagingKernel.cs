using System;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace XyrusWorx.Runtime.Imaging
{
	[PublicAPI]
	public abstract class SoftwareImagingKernel : Resource, IImagingKernel
	{
		private readonly ResourceList<IWritable> mConstants;
		private readonly ResourceList<IWritable> mTextures;
		
		private ParallelOptions mParallelOptions;
		private TextureView mOutput;
		private UnmanagedBlock mOutputMemory;

		protected SoftwareImagingKernel()
		{
			mParallelOptions = new ParallelOptions();

			mConstants = new ResourceList<IWritable>();
			mTextures = new ResourceList<IWritable>();
		}
		
		public abstract Int2 TextureSize { get; }

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

		public IResourcePool<IWritable> Constants => mConstants;
		public IResourcePool<IWritable> Resources => mTextures;
		public IReadable Output
		{
			get
			{
				if (mOutput == null)
				{
					CreateOutputTexture();
				}

				return mOutput;
			}
		}

		public void Execute()
		{
			if (TextureSize.x <= 0 || TextureSize.y <= 0)
			{
				return;
			}
			
			void ParallelLoopCallback(int index, Int2 arraySize2D)
			{
				var position = new Int2(index % arraySize2D.x, index / arraySize2D.x);
				var psii = new SoftwareImagingKernelContext(new Float2(position.x / (float)arraySize2D.x, position.y / (float)arraySize2D.y));

				var color = ExecuteOverride(psii);
				var icolor = (color * 255f).Clamp(new Float4(), new Float4(255,255,255,255)).Int();
				
				mOutput[position] = new Vector4<byte>((byte)icolor.x, (byte)icolor.y, (byte)icolor.z, (byte)icolor.w);
			}

			try
			{
				Parallel.For(0, TextureSize.x*TextureSize.y, mParallelOptions, i => ParallelLoopCallback(i, TextureSize));
			}
			catch(OperationCanceledException){}
		}
		
		protected abstract Float4 ExecuteOverride(SoftwareImagingKernelContext context);
		protected sealed override void DisposeOverride()
		{
			mOutputMemory?.Dispose();
			mOutput = null;
			
			mConstants.Reset();
			mTextures.Reset();
		}

		private void CreateOutputTexture()
		{
			mOutputMemory?.Dispose();
			mOutputMemory = new UnmanagedBlock(TextureSize.x * TextureSize.y * 4);
			mOutput = new TextureView(mOutputMemory, TextureSize.x << 2, TextureFormat.Bgra);
		}
		
		class ResourceList<T> : IResourcePool<T> where T: class
		{
			private T[] mElements = new T[1024];

			public T this[int slot]
			{
				set => AssignToSlot(slot, value);
			}
			public void Submit()
			{
			}
			
			public void Reset()
			{
				for (var i = 0; i < 1024; i++)
				{
					mElements[i] = null;
				}
			}

			private void AssignToSlot(int slot, T item)
			{
				if (slot >= 1024)
				{
					throw new ArgumentOutOfRangeException(nameof(slot));
				}

				mElements[slot] = item;
			}
		}
	}
}