using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using XyrusWorx.Runtime.IO;

namespace XyrusWorx.Runtime.Graphics
{
	[PublicAPI]
	public interface IImagingKernel : IDisposable
	{
		IList<IStructuredBuffer> Resources { get; }
		IList<IStructuredReadWriteBuffer> Constants { get; }
		IDataStream<Vector4<byte>> Compute(int arrayWidth, int arrayHeight);
	}
}