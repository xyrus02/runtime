using System;
using JetBrains.Annotations;
using XyrusWorx.Runtime.Graphics.Imaging;

namespace XyrusWorx.Runtime.Graphics 
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