using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using XyrusWorx.Runtime.IO;

namespace XyrusWorx.Runtime.Graphics 
{
	[PublicAPI]
	public interface IReactorContext 
	{
		[NotNull]
		ICache Cache { get; }
		
		void Kernel([NotNull] Action<int> kernel, [CanBeNull] ParallelOptions parallelOptions = null);

		Float4 Rgba(Int2 pixel);
		void Rgba(Int2 pixel, Float4 rgba);
		
		Int2 Map(Float2 uv);
		Float2 Map(Int2 pixel);

		Int2 GetBackBufferSize();
	}

}