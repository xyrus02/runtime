using System;
using JetBrains.Annotations;
using XyrusWorx.Runtime.Imaging;

namespace XyrusWorx.Runtime 
{
	[PublicAPI]
	public abstract class Reactor : IReactor
	{
		private TextureFormat mBackBufferPixelFormat;
		private IWritableMemory mBackBufferMemory;
		private TextureView mBackBuffer;
		private ReactorRenderContext mContext;

		protected Reactor()
		{
			mContext = new ReactorRenderContext(this);
		}

		public void Update(IRenderLoop renderLoop)
		{
			if (renderLoop == null)
			{
				throw new ArgumentNullException(nameof(renderLoop));
			}

			UpdateOverride(renderLoop, mContext);
		}

		void IReactor.SetBackBuffer(IWritableMemory writableMemory, TextureFormat pixelFormat, int stride)
		{
			mBackBufferMemory = writableMemory;
			mBackBufferPixelFormat = pixelFormat;
			mBackBuffer = new TextureView(writableMemory, stride, pixelFormat);

			InitializeOverride();
		}

		public void Dispose()
		{
			mBackBufferPixelFormat = default;
			mBackBufferMemory = null;
			mBackBuffer = null;
		}

		protected abstract void InitializeOverride();
		protected abstract void UpdateOverride([NotNull] IRenderLoop renderLoop, IRenderContext context);

		public IReadWriteTexture BackBuffer => mBackBuffer;

		public int BackBufferWidth => mBackBuffer?.Width ?? 0;
		public int BackBufferHeight => mBackBuffer?.Height ?? 0;

		class ReactorRenderContext : IRenderContext
		{
			private readonly Reactor mReactor;

			public ReactorRenderContext(Reactor reactor)
			{
				mReactor = reactor;
			}

			public void Blit(IReadableMemory pixels)
			{
				mReactor.mBackBufferMemory.Write(pixels);
			}

			public void Blit(IReadableTexture texture)
			{
				mReactor.mBackBuffer.Write(texture.RawMemory);
			}
		}
	}

	public interface IRenderContext
	{
		void Blit(IReadableMemory pixels);
		void Blit(IReadableTexture texture);
	}

}