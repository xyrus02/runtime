using System;
using System.Threading;
using JetBrains.Annotations;

namespace XyrusWorx.Runtime.Graphics 
{
	[PublicAPI]
	public interface IRenderLoop : IDisposable
	{
		void Run(CancellationToken cancellationToken);
		void WaitForFrame();
		
		[NotNull]
		IReactor CurrentReactor { get; }
		
		double Clock { get; }
		double FramesPerSecond { get; }
	}
}