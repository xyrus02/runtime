using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using JetBrains.Annotations;
using XyrusWorx.Windows.Runtime;

namespace XyrusWorx.Runtime 
{
	[PublicAPI]
	public abstract class WpfGraphicsApplication<TReactor> : WpfApplication<WpfPresenter, RenderLoop<TReactor>> where TReactor: class, IReactor
	{
		public IRenderLoop RenderLoop => GetViewModel<IRenderLoop>();
		public IWpfPresenter Presenter => GetView<WpfPresenter>();

		protected virtual void OnInitialize([NotNull] IWpfPresenter view, [NotNull] TReactor reactor){}
		protected virtual void OnTerminate([NotNull] IWpfPresenter view, [NotNull] TReactor reactor){}
		protected virtual void OnConfigureWindowOverride([NotNull] Window window) { }

		protected sealed override void OnConfigureWindow(Window window)
		{
			window.SizeToContent = SizeToContent.WidthAndHeight;
			window.ResizeMode = ResizeMode.NoResize;

			OnConfigureWindowOverride(window);
		}
		protected sealed override Task OnInitialize(RenderLoop<TReactor> viewModel)
		{
			var view = GetView<WpfPresenter>();
			
			viewModel.Reactor = ServiceLocator.Default.CreateInstance<TReactor>();
			view.Background = Brushes.Black;
			view.Loaded += (o, e) => view.Run();

			OnInitialize(view, viewModel.Reactor);

			return Task.CompletedTask;
		}
		protected sealed override Task OnShutdown(RenderLoop<TReactor> viewModel)
		{
			var view = GetView<WpfPresenter>();

			if (view != null)
			{
				if (viewModel.Reactor != null) 
				{
					OnTerminate(view, viewModel.Reactor);
				
					viewModel.Reactor.Dispose();
					viewModel.Reactor = null;
				}
			}

			return Task.CompletedTask;
		}
	}
}