using System.Threading;
using JetBrains.Annotations;

namespace XyrusWorx.Runtime.Graphics 
{
	[PublicAPI]
	public interface IRenderLoop
	{
		void Run(CancellationToken cancellationToken);
		void WaitForFrame();
		
		[CanBeNull]
		IReactor CurrentReactor { get; }
		
		double Clock { get; }
		double FramesPerSecond { get; }
		double MaximumFramesPerSecond { get; set; }
	}
}