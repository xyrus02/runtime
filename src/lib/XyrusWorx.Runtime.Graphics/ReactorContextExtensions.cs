using System;
using System.Numerics;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace XyrusWorx.Runtime.Graphics 
{
	[PublicAPI]
	public static class ReactorContextExtensions
	{
		public static Vector3 Rgb(this IReactorContext context, int x, int y) => context.Rgba(x, y).xyz();
		public static void Rgb(this IReactorContext context, int x, int y, Vector3 rgb) => context.Rgba(x, y, rgb.Expand(1));

		public static Vector3 Rgb(this IReactorContext context, Vector2 uv)
		{
			context.Map(uv, out var x, out var y);
			return context.Rgba(x, y).xyz();
		}
		public static void Rgb(this IReactorContext context, Vector2 uv, Vector3 rgb)
		{
			context.Map(uv, out var x, out var y);
			context.Rgba(x, y, rgb.Expand(1));
		}
		
		public static Vector4 Rgba(this IReactorContext context, Vector2 uv)
		{
			context.Map(uv, out var x, out var y);
			return context.Rgba(x, y);
		}
		public static void Rgba(this IReactorContext context, Vector2 uv, Vector4 rgba)
		{
			context.Map(uv, out var x, out var y);
			context.Rgba(x, y, rgba);
		}
		
		public static void Kernel(this IReactorContext context, Action<Vector2> kernel, ParallelOptions parallelOptions = null)
		{
			var wrapper = new Action<int>(
				i =>
				{
					context.GetBackBufferSize(out var w, out var h);
					// ReSharper disable once PossibleLossOfFraction
					kernel(new Vector2((float)(i % w) / w, (float)(i / w) / h));
				});
			
			context.Kernel(wrapper, parallelOptions);
		}
		public static void Kernel(this IReactorContext context, Action<int, int> kernel, ParallelOptions parallelOptions = null)
		{
			var wrapper = new Action<int>(
				i =>
				{
					// ReSharper disable once UnusedVariable
					context.GetBackBufferSize(out var w, out var h);
					kernel(i % w, i / w);
				});
			
			context.Kernel(wrapper, parallelOptions);
		}
	}
}