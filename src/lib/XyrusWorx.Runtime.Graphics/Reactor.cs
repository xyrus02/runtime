using System;
using System.Runtime.InteropServices;
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

		public void InvalidateState()
		{
			mContext.Cache.Clear();
			
			if (BackBuffer == IntPtr.Zero)
			{
				if (BackBufferWidth <= 0 || BackBufferHeight <= 0)
				{
					throw new InvalidOperationException("The buffer dimensions are invalid.");
				}

				BackBuffer = Marshal.AllocHGlobal(new IntPtr((long)((IReactor)this).BackBufferStride * BackBufferHeight));
			}
			
			InvalidateStateOverride(mContext);
		}
		public void Update(IRenderLoop renderLoop)
		{
			if (renderLoop == null)
			{
				throw new ArgumentNullException(nameof(renderLoop));
			}

			UpdateOverride(renderLoop, mContext);
		}
		public void Dispose()
		{
			mContext.Cache.Clear();
			
			if (BackBuffer != IntPtr.Zero)
			{
				Marshal.FreeHGlobal(BackBuffer);
				BackBuffer = IntPtr.Zero;
			}
		}

		protected abstract void InvalidateStateOverride([NotNull] IReactorContext context);
		protected abstract void UpdateOverride([NotNull] IRenderLoop renderLoop, [NotNull] IReactorContext context);

		public IntPtr BackBuffer { get; private set; }
		public abstract int BackBufferWidth { get; }
		public abstract int BackBufferHeight { get; }

		int IReactor.BackBufferStride => BackBufferWidth * 4;
	}

}