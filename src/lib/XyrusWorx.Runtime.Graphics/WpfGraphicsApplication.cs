using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using JetBrains.Annotations;
using XyrusWorx.Threading;
using XyrusWorx.Windows.Runtime;

namespace XyrusWorx.Runtime.Graphics 
{
	[PublicAPI]
	public abstract class WpfGraphicsApplication<TReactor> : WpfApplication<WpfFrontBuffer, RenderLoop<TReactor, WpfFrontBuffer>> where TReactor: class, IReactor
	{
		private RelayOperation mRenderLoopThread;

		protected virtual void OnInitialize([NotNull] WpfFrontBuffer view, [NotNull] TReactor reactor){}
		protected virtual void OnTerminate([NotNull] WpfFrontBuffer view, [NotNull] TReactor reactor){}
		
		protected sealed override void OnConfigureWindow(Window window)
		{
			window.SizeToContent = SizeToContent.WidthAndHeight;
			window.ResizeMode = ResizeMode.NoResize;
		}
		protected sealed override Task OnInitialize(RenderLoop<TReactor, WpfFrontBuffer> viewModel)
		{
			var view = GetView<WpfFrontBuffer>();
			
			viewModel.Presenter = view ;
			view.Background = Brushes.Black;

			OnInitialize(view, viewModel.CurrentReactor);
			
			mRenderLoopThread = new RelayOperation(ct => viewModel.Run(ct));
			mRenderLoopThread.DispatchMode = OperationDispatchMode.BackgroundThread;
			mRenderLoopThread.Run();
			
			return Task.CompletedTask;
		}
		protected sealed override Task OnShutdown(RenderLoop<TReactor, WpfFrontBuffer> viewModel)
		{
			var view = GetView<WpfFrontBuffer>();
			
			OnTerminate(view, viewModel.CurrentReactor);
			viewModel.Dispose();
			
			mRenderLoopThread.Cancel();
			mRenderLoopThread = null;
			
			return Task.CompletedTask;
		}
	}
}