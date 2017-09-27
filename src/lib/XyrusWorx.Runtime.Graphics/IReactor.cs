using System;
using JetBrains.Annotations;
using XyrusWorx.Runtime.Imaging;

namespace XyrusWorx.Runtime 
{

	[PublicAPI]
	public interface IReactor : IDisposable
	{
		void InvalidateState();
		void Update([NotNull] IRenderLoop renderLoop);
		
		[NotNull]
		IReadWriteTexture BackBuffer { get; }
	}
}