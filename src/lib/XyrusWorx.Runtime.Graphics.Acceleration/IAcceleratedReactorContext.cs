using System.Collections.Generic;
using JetBrains.Annotations;
using XyrusWorx.Runtime.Graphics.Computation;
using XyrusWorx.Runtime.Graphics.Imaging;

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