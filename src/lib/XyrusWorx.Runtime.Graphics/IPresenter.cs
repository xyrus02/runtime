using JetBrains.Annotations;

namespace XyrusWorx.Runtime 
{
	[PublicAPI]
	public interface IPresenter
	{
		void SetSize(Int2 size);

		bool ShowFramesPerSecond { get; set; }
		bool ShowClock { get; set; }

		void Run();
	}
}