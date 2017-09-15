using System;
using System.Diagnostics;
using System.Threading;
using JetBrains.Annotations;
using XyrusWorx.Windows.ViewModels;

namespace XyrusWorx.Runtime.Graphics 
{
	[PublicAPI]
	public class RenderLoop<TReactor, TPresenter> : ViewModel, IRenderLoop 
		where TReactor: class, IReactor
		where TPresenter: class, IPresenter
	{
		private readonly object mFrameLock = new object();
		private readonly Scope mRunningScope = new Scope();
		
		private readonly IServiceLocator mServices;

		public RenderLoop() : this(null) {}
		public RenderLoop(IServiceLocator services)
		{
			mServices = services ?? ServiceLocator.Default;
			CurrentReactor = mServices.CreateInstance<TReactor>();
			CurrentPresenter = mServices.CreateInstance<TPresenter>();
		}
		
		public double Clock { get; private set; }
		public double FramesPerSecond { get; private set; }

		[NotNull]
		public TReactor CurrentReactor { get; }
		IReactor IRenderLoop.CurrentReactor => CurrentReactor;
		
		[NotNull]
		public TPresenter CurrentPresenter { get; }
		
		public void Run(CancellationToken cancellationToken)
		{
			if (mRunningScope.IsInScope)
			{
				return;
			}
			
			using (mRunningScope.Enter())
			{
				var host = mServices.Resolve<IApplicationHost>();
				var watch = new Stopwatch();
				
				const double tIter = 1.0 / 30d;

				while (!cancellationToken.IsCancellationRequested)
				{
					watch.Restart();

					lock (mFrameLock)
					{
						CurrentReactor.Update(this);
						host.Execute(() => CurrentPresenter.Present(CurrentReactor, this));
					}
					
					var t = watch.Elapsed.TotalSeconds;
					var fps = 1.0 / t;
					var dt = tIter - t;

					FramesPerSecond = FramesPerSecond <= 0 ? fps : FramesPerSecond * 0.9 + fps * 0.1;
					Clock += t;
					
					if (dt > 0)
					{
						Thread.Sleep(TimeSpan.FromSeconds(dt));
					}
				}
			}
			
			
		}
		public void WaitForFrame() => Monitor.Wait(mFrameLock);
	}
}