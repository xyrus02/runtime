using System;
using System.Numerics;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace XyrusWorx.Runtime.Graphics 
{
	[PublicAPI]
	public abstract class Reactor : IReactor
	{
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

			UpdateOverride(renderLoop);
		}
		public void Dispose()
		{
			if (BackBuffer != IntPtr.Zero)
			{
				Marshal.FreeHGlobal(BackBuffer);
				BackBuffer = IntPtr.Zero;
			}
		}

		protected abstract void UpdateOverride([NotNull] IRenderLoop renderLoop);

		protected Vector3 Rgb(int x, int y)
		{
			var rgba = Rgba(x, y);
			return new Vector3(rgba.X, rgba.Y, rgba.Z);
		}
		protected Vector4 Rgba(int x, int y)
		{
			if (x < 0 || y < 0 || x >= BackBufferWidth || y >= BackBufferHeight)
			{
				return new Vector4(0, 0, 0, 0);
			}

			var addr = x << 2 + y * ((IReactor)this).BackBufferStride;
			var p = new byte[4];
			
			Marshal.Copy(BackBuffer + addr, p, 0, p.Length);
			
			// BGRA --> RGBA
			return new Vector4(
				p[2] / 255f, 
				p[1] / 255f,
				p[0] / 255f,
				p[3] / 255f);
		}
		
		protected void Rgb(int x, int y, Vector3 rgb)
		{
			var rgba = new Vector4(rgb.X, rgb.Y, rgb.Z, 1f);
			Rgba(x, y, rgba);
		}
		protected void Rgba(int x, int y, Vector4 rgba)
		{
			if (x < 0 || y < 0 || x >= BackBufferWidth || y >= BackBufferHeight)
			{
				return;
			}
			
			var addr = x << 2 + y * ((IReactor)this).BackBufferStride;
			
			// RGBA --> BGRA
			var p = new[]
			{
				(byte)(rgba.Z * 255f),
				(byte)(rgba.Y * 255f),
				(byte)(rgba.X * 255f),
				(byte)(rgba.W * 255f)
			};
			
			Marshal.Copy(p, 0, BackBuffer + addr, p.Length);
		}

		public IntPtr BackBuffer { get; private set; }
		public abstract int BackBufferWidth { get; }
		public abstract int BackBufferHeight { get; }

		int IReactor.BackBufferStride => BackBufferWidth * 4;
	}
}