using System.Threading;
using JetBrains.Annotations;

namespace XyrusWorx.Runtime 
{
	[PublicAPI]
	public interface IRenderLoop
	{
		void Run(CancellationToken cancellationToken);
		void WaitForFrame();
		
		[CanBeNull]
		IReactor CurrentReactor { get; }
		
		float Clock { get; }
		float FramesPerSecond { get; }
		float MaximumFramesPerSecond { get; set; }
	}
}