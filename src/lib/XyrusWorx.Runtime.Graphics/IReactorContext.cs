using System;
using System.Numerics;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace XyrusWorx.Runtime.Graphics 
{
	[PublicAPI]
	public interface IReactorContext 
	{
		void Kernel([NotNull] Action<int> kernel, [CanBeNull] ParallelOptions parallelOptions = null);

		Vector4 Rgba(int x, int y);
		void Rgba(int x, int y, Vector4 rgba);
		
		void Map(Vector2 uv, out int x, out int y);
		void Map(int x, int y, out Vector2 uv);

		void GetBackBufferSize(out int width, out int height);
	}
}