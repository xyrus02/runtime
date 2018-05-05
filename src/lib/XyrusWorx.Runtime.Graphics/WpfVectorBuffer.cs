using System;
using System.Windows.Media;

namespace XyrusWorx.Runtime 
{
	class WpfVectorBuffer : IVectorBuffer
	{
		private DrawingGroup mDrawingGroup = new DrawingGroup();
		private WpfVectorFrame mFrame;

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