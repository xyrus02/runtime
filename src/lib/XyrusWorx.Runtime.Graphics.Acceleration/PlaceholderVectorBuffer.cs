using System;
using System.Windows.Media;

namespace XyrusWorx.Runtime 
{
	class PlaceholderVectorBuffer : IVectorBuffer
	{
		private PlaceholderVectorFrame mFrame;

		public IDisposable BeginFrame()
		{
			return new Scope(() => mFrame = new PlaceholderVectorFrame(), () => mFrame = null);
		}
		public IVectorFrame GetCurrentFrame()
		{
			if (mFrame == null)
			{
				throw new InvalidOperationException("Can't operate on a closed frame.");
			}

			return mFrame;
		}

		public DrawingGroup ToDrawingGroup() => null;
	}
}