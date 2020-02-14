using JetBrains.Annotations;

namespace XyrusWorx.Runtime 
{
	[PublicAPI]
	public interface IRenderLoop
	{
		void Reset();
		void NextFrame();

		[CanBeNull]
		IReactor CurrentReactor { get; }
		
		float Clock { get; }
		float FramesPerSecond { get; }
	}
}