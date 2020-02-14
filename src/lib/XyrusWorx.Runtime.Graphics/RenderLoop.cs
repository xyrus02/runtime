using System.Diagnostics;
using JetBrains.Annotations;
using XyrusWorx.Windows.ViewModels;

namespace XyrusWorx.Runtime 
{
	[PublicAPI]
	public class RenderLoop<TReactor> : ViewModel, IRenderLoop 
		where TReactor: class, IReactor
	{
		private Stopwatch mWatch = new Stopwatch();
		private Stopwatch mFpsWatch = new Stopwatch();

		private float mClock;
		private float mFramesPerSecond;

		public float Clock
		{
			get => mClock;
			private set
			{
				if (value.Equals(mClock)) return;
				mClock = value;
				OnPropertyChanged();
			}
		}
		public float FramesPerSecond
		{
			get => mFramesPerSecond;
			private set
			{
				if (value.Equals(mFramesPerSecond)) return;
				mFramesPerSecond = value;
				OnPropertyChanged();
			}
		}

		[CanBeNull]
		public TReactor Reactor { get; set; }
		IReactor IRenderLoop.CurrentReactor => Reactor;

		public void Reset()
		{
			mFpsWatch.Restart();
		}
		public void NextFrame()
		{
			mWatch.Restart();
			Reactor?.Update(this);

			var t = (float)mWatch.Elapsed.TotalSeconds;

			if (mFpsWatch.ElapsedMilliseconds >= 1000)
			{
				var tt = (float)mWatch.Elapsed.TotalSeconds;
				var fps = 1.0f / tt;

				FramesPerSecond = FramesPerSecond <= 0 ? fps : FramesPerSecond * 0.9f + fps * 0.1f;
				mFpsWatch.Restart();
			}

			Clock += t;
		}
	}
}