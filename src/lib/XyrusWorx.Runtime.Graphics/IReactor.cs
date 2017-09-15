using System;
using JetBrains.Annotations;

namespace XyrusWorx.Runtime.Graphics 
{
	[PublicAPI]
	public interface IReactor
	{
		void Update([NotNull] IRenderLoop renderLoop);
		
		IntPtr BackBuffer { get; }
		
		int BackBufferStride { get; }
		int BackBufferHeight { get; }
	}
}