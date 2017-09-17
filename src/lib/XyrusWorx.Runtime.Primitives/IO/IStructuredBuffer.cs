using System;
using JetBrains.Annotations;

namespace XyrusWorx.Runtime.IO
{
	[PublicAPI]
	public interface IStructuredBuffer : IDisposable
	{
		int ElementCount { get; }
		int BufferSize { get; }
	}
}