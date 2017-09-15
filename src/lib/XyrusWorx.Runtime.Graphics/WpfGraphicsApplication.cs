using System.Threading.Tasks;
using System.Windows;
using JetBrains.Annotations;
using XyrusWorx.Threading;
using XyrusWorx.Windows.Runtime;

namespace XyrusWorx.Runtime.Graphics 
{
	[PublicAPI]
	public abstract class WpfGraphicsApplication<TReactor> : WpfApplication<WpfFrontBuffer, RenderLoop<TReactor, WpfFrontBuffer>> where TReactor: class, IReactor
	{
		private RelayOperation mRenderLoopThread;

		protected override void OnConfigureWindow(Window window)
		{
			window.SizeToContent = SizeToContent.WidthAndHeight;
			window.ResizeMode = ResizeMode.NoResize;
		}
		protected sealed override Task OnInitialize(RenderLoop<TReactor, WpfFrontBuffer> viewModel)
		{
			mRenderLoopThread = new RelayOperation(ct => viewModel.Run(ct));
			mRenderLoopThread.DispatchMode = OperationDispatchMode.BackgroundThread;
			mRenderLoopThread.Run();
			
			return Task.CompletedTask;
		}
		protected sealed override Task OnShutdown(RenderLoop<TReactor, WpfFrontBuffer> viewModel)
		{
			// necessary because otherwise WPF causes a deadlock
			GetViewModel<IRenderLoop>()?.WaitForFrame();
			
			mRenderLoopThread.Cancel();
			mRenderLoopThread = null;
			
			return Task.CompletedTask;
		}
	}
}