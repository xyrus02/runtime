using System;
using JetBrains.Annotations;
using XyrusWorx.Runtime.Imaging;

namespace XyrusWorx.Runtime 
{
	[PublicAPI]
	public abstract class Reactor : IReactor
	{
		private UnmanagedBlock mBackBufferMemory;
		private TextureView mBackBuffer;

		public void InvalidateState()
		{
			if (mBackBuffer == null)
			{
				if (BackBufferWidth <= 0 || BackBufferHeight <= 0)
				{
					throw new InvalidOperationException("The buffer dimensions are invalid.");
				}

				mBackBufferMemory = new UnmanagedBlock(BackBufferWidth * BackBufferHeight * 4);
				mBackBuffer = new TextureView(mBackBufferMemory, BackBufferWidth << 2, TextureFormat.Rgba);
			}
			
			InvalidateStateOverride();
		}
		public void Update(IRenderLoop renderLoop)
		{
			if (renderLoop == null)
			{
				throw new ArgumentNullException(nameof(renderLoop));
			}

			UpdateOverride(renderLoop);
		}
		public void Dispose()
		{
			mBackBufferMemory?.Dispose();
			mBackBufferMemory = null;
			mBackBuffer = null;
		}

		protected abstract void InvalidateStateOverride();
		protected abstract void UpdateOverride([NotNull] IRenderLoop renderLoop);

		public IReadWriteTexture BackBuffer => mBackBuffer;
		
		public abstract int BackBufferWidth { get; }
		public abstract int BackBufferHeight { get; }
	}

}