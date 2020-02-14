using System;
using System.Windows.Media;
using System.Windows.Threading;

namespace XyrusWorx.Runtime 
{
	class WpfVectorBuffer : IVectorBuffer
	{
		private DrawingGroup mDrawingGroup;
		private WpfVectorFrame mFrame;

		public WpfVectorBuffer(Dispatcher dispatcher)
		{
			mDrawingGroup = dispatcher == null ? new DrawingGroup() : dispatcher.Invoke(() => new DrawingGroup());
		}

		public IDisposable BeginFrame()
		{
			return new Scope(
				() => mFrame = new WpfVectorFrame(mDrawingGroup.Open()), 
				() => mFrame?.Dispose()).Enter();
		}
		public IVectorFrame GetCurrentFrame()
		{
			if (mFrame == null)
			{
				throw new InvalidOperationException("Can't operate on a closed frame.");
			}

			return mFrame;
		}
		
		public DrawingGroup ToDrawingGroup() => mDrawingGroup;
	}
}