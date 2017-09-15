using System;
using System.Numerics;
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
			
			public void Kernel(Action<Vector2> kernel, ParallelOptions parallelOptions = null)
			{
				var bbw = mReactor.BackBufferWidth;
				var bbh = mReactor.BackBufferHeight;
				
				Parallel.For(0, bbw * bbh, offs =>
				{
					var i = offs % bbw;
					var j = offs / bbw;
				
					kernel(new Vector2(i / (float)bbw, j / (float)bbh));
				});
			}

			public void Map(Vector2 uv, out int x, out int y)
			{
				x = (int)(uv.X * mReactor.BackBufferWidth) % mReactor.BackBufferWidth;
				y = (int)(uv.Y * mReactor.BackBufferHeight) % mReactor.BackBufferHeight;
			}
			public void Map(int x, int y, out Vector2 uv)
			{
				uv = new Vector2(
					x / (float)mReactor.BackBufferWidth,
					y / (float)mReactor.BackBufferHeight);
			}

			public Vector4 Rgba(int x, int y)
			{
				if (x < 0 || y < 0 || x >= mReactor.BackBufferWidth || y >= mReactor.BackBufferHeight)
				{
					return new Vector4(0, 0, 0, 0);
				}

				var addr = (x << 2) + y * ((IReactor)mReactor).BackBufferStride;
				var p = new byte[4];
			
				Marshal.Copy(mReactor.BackBuffer + addr, p, 0, p.Length);
			
				// BGRA --> RGBA
				return new Vector4(
					p[2] / 255f, 
					p[1] / 255f,
					p[0] / 255f,
					p[3] / 255f);
			}
			public void Rgba(int x, int y, Vector4 rgba)
			{
				if (x < 0 || y < 0 || x >= mReactor.BackBufferWidth || y >= mReactor.BackBufferHeight)
				{
					return;
				}
			
				var addr = (x << 2) + y * ((IReactor)mReactor).BackBufferStride;
			
				// RGBA --> BGRA
				var p = new[]
				{
					(byte)(rgba.Z * 255f),
					(byte)(rgba.Y * 255f),
					(byte)(rgba.X * 255f),
					(byte)(rgba.W * 255f)
				};
			
				Marshal.Copy(p, 0, mReactor.BackBuffer + addr, p.Length);
			}
		}
	}
}