using System.Collections.Generic;
using JetBrains.Annotations;

namespace XyrusWorx.Runtime.Graphics 
{
	[PublicAPI]
	public interface IAcceleratedReactorContext
	{
		[NotNull]
		IImagingKernel ImagingKernel { get; }
		
		[NotNull]
		IReadOnlyList<IComputationKernel> ComputationQueue { get; }
	}
}