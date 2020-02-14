using System;
using JetBrains.Annotations;
using XyrusWorx.Runtime.Imaging;

namespace XyrusWorx.Runtime 
{
	[PublicAPI]
	public interface IReactor : IDisposable
	{
		void Update([NotNull] IRenderLoop renderLoop);
		void SetBackBuffer(IWritableMemory writableMemory, TextureFormat pixelFormat, int stride);
	}

}