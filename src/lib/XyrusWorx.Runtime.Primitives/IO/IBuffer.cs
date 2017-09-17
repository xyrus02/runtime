using System;
using JetBrains.Annotations;

namespace XyrusWorx.Runtime.IO
{
	[PublicAPI]
	public interface IBuffer<out T> : IDisposable
	{
		T[] AsArray();
	}
}