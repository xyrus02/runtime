using JetBrains.Annotations;

namespace XyrusWorx.Runtime.Graphics 
{
	[PublicAPI]
	public interface IPresenter
	{
		void InvalidateBackBuffer();
		void Present([NotNull] IReactor reactor, [NotNull] IRenderLoop renderLoop);
	}
}