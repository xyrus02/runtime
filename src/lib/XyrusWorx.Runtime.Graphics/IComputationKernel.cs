using System.Collections.Generic;
using JetBrains.Annotations;
using XyrusWorx.Runtime.IO;

namespace XyrusWorx.Runtime.Graphics
{
	[PublicAPI]
	public interface IComputationKernel
	{
		IList<IStructuredBuffer> Resources { get; }
		IList<IDeviceBuffer> Outputs { get; }
		IList<IStructuredReadWriteBuffer> Constants { get; }

		void Compute();

		int ThreadGroupCountX { get; set; }
		int ThreadGroupCountY { get; set; }
		int ThreadGroupCountZ { get; set; }
	}
}