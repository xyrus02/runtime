using System;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace XyrusWorx.Runtime.Graphics 
{
	[PublicAPI]
	public static class ReactorContextExtensions
	{
		public static Float3 Rgb(this IReactorContext context, int x, int y) => context.Rgba(new Int2(x, y)).xyz;
		public static void Rgb(this IReactorContext context, int x, int y, Float3 rgb) => context.Rgba(new Int2(x, y), new Float4(rgb, 1));
		
		public static Float3 Rgb(this IReactorContext context, Int2 pixel) => context.Rgba(pixel).xyz;
		public static void Rgb(this IReactorContext context, Int2 pixel, Float3 rgb) => context.Rgba(pixel, new Float4(rgb, 1));

		public static Float3 Rgb(this IReactorContext context, Float2 uv) => context.Rgba(context.Map(uv)).xyz;
		public static void Rgb(this IReactorContext context, Float2 uv, Float3 rgb) => context.Rgba(context.Map(uv), new Float4(rgb, 1));

		public static Float4 Rgba(this IReactorContext context, int x, int y) => context.Rgba(new Int2(x, y));
		public static void Rgba(this IReactorContext context, int x, int y, Float4 rgba) => context.Rgba(new Int2(x, y), rgba);
		
		public static Float4 Rgba(this IReactorContext context, Float2 uv) => context.Rgba(context.Map(uv));
		public static void Rgba(this IReactorContext context, Float2 uv, Float4 rgba) => context.Rgba(context.Map(uv), rgba);

		public static void Kernel(this IReactorContext context, Action<int, int> kernel, ParallelOptions parallelOptions = null)
		{
			var wrapper = new Action<int>(
				i =>
				{
					// ReSharper disable once UnusedVariable
					var size = context.GetBackBufferSize();
					kernel(i % size.x, i / size.x);
				});
			
			context.Kernel(wrapper, parallelOptions);
		}
		public static void Kernel(this IReactorContext context, Action<Int2> kernel, ParallelOptions parallelOptions = null)
		{
			var wrapper = new Action<int>(
				i =>
				{
					// ReSharper disable once UnusedVariable
					var size = context.GetBackBufferSize();
					kernel(new Int2(i % size.x, i / size.y));
				});
			
			context.Kernel(wrapper, parallelOptions);
		}
		public static void Kernel(this IReactorContext context, Action<Float2> kernel, ParallelOptions parallelOptions = null)
		{
			var wrapper = new Action<int>(
				i =>
				{
					var size = context.GetBackBufferSize();
					// ReSharper disable once PossibleLossOfFraction
					kernel(new Float2((float)(i % size.x) / size.x, (float)(i / size.x) / size.y));
				});
			
			context.Kernel(wrapper, parallelOptions);
		}
	}
}