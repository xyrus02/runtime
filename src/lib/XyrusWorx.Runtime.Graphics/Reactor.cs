using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace XyrusWorx.Runtime.Graphics 
{

	[PublicAPI]
	public abstract class Reactor : IReactor
	{
		private readonly IReactorContext mContext;

		protected Reactor()
		{
			mContext = new ReactorContext(this);
		}
		
		public void Update(IRenderLoop renderLoop)
		{
			if (renderLoop == null)
			{
				throw new ArgumentNullException(nameof(renderLoop));
			}

			if (BackBuffer == IntPtr.Zero)
			{
				if (BackBufferWidth <= 0 || BackBufferHeight <= 0)
				{
					throw new InvalidOperationException("The buffer dimensions are invalid.");
				}

				BackBuffer = Marshal.AllocHGlobal(new IntPtr((long)((IReactor)this).BackBufferStride * BackBufferHeight));
			}

			UpdateOverride(renderLoop, mContext);
		}
		public void Dispose()
		{
			if (BackBuffer != IntPtr.Zero)
			{
				Marshal.FreeHGlobal(BackBuffer);
				BackBuffer = IntPtr.Zero;
			}
		}

		protected abstract void UpdateOverride([NotNull] IRenderLoop renderLoop, [NotNull] IReactorContext context);

		public IntPtr BackBuffer { get; private set; }
		public abstract int BackBufferWidth { get; }
		public abstract int BackBufferHeight { get; }

		int IReactor.BackBufferStride => BackBufferWidth * 4;

		sealed class ReactorContext : IReactorContext
		{
			private readonly Reactor mReactor;

			internal ReactorContext([NotNull] Reactor reactor)
			{
				if (reactor == null)
				{
					throw new ArgumentNullException(nameof(reactor));
				}
			
				mReactor = reactor;
			}

			public Int2 GetBackBufferSize() => new Int2(
				mReactor.BackBufferWidth,
				mReactor.BackBufferHeight);

			public void Kernel(Action<int> kernel, ParallelOptions parallelOptions = null)
			{
				Parallel.For(0, mReactor.BackBufferWidth * mReactor.BackBufferHeight, parallelOptions ?? new ParallelOptions(), kernel);
			}

			public Int2 Map(Float2 uv) => new Int2(
				(int)(uv.x * mReactor.BackBufferWidth) % mReactor.BackBufferWidth,
				(int)(uv.y * mReactor.BackBufferHeight) % mReactor.BackBufferHeight);

			public Float2 Map(Int2 pixel) => new Float2(
				pixel.x / (float)mReactor.BackBufferWidth,
				pixel.y / (float)mReactor.BackBufferHeight);

			public Float4 Rgba(Int2 pixel)
			{
				var x = pixel.x;
				var y = pixel.y;
				
				if (x < 0 || y < 0 || x >= mReactor.BackBufferWidth || y >= mReactor.BackBufferHeight)
				{
					return new Float4();
				}

				var addr = (x << 2) + y * ((IReactor)mReactor).BackBufferStride;
				var p = new byte[4];
			
				Marshal.Copy(mReactor.BackBuffer + addr, p, 0, p.Length);
			
				// BGRA --> RGBA
				return new Float4(
					p[2] / 255f, 
					p[1] / 255f,
					p[0] / 255f,
					p[3] / 255f);
			}
			public void Rgba(Int2 pixel, Float4 rgba)
			{
				var x = pixel.x;
				var y = pixel.y;
				
				if (x < 0 || y < 0 || x >= mReactor.BackBufferWidth || y >= mReactor.BackBufferHeight)
				{
					return;
				}
			
				var addr = (x << 2) + y * ((IReactor)mReactor).BackBufferStride;
			
				// RGBA --> BGRA
				var p = new[]
				{
					(byte)(rgba.z * 255f),
					(byte)(rgba.y * 255f),
					(byte)(rgba.x * 255f),
					(byte)(rgba.w * 255f)
				};
			
				Marshal.Copy(p, 0, mReactor.BackBuffer + addr, p.Length);
			}
		}
	}
}