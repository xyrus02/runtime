using System;
using System.Windows;
using System.Windows.Media;
using XyrusWorx.Runtime.Expressions;
using XyrusWorx.Runtime.Imaging;

namespace XyrusWorx.Runtime.SimpleGraphicsAppTest
{
	class Program : WpfGraphicsApplication<TestReactorAccelerated>
	{
		[STAThread]
		static void Main() => Bootstrap<Program>();

		protected override void OnConfigureWindowOverride(Window window)
		{
			window.Title = "Test";
		}

		protected override void OnInitialize(IWpfPresenter view, TestReactorAccelerated reactor)
		{
			view.SetSize(new Int2(1024, 1024));
			view.ShowFramesPerSecond = true;
			view.ShowClock = true;
			view.MeasuresFontFamily = new FontFamily("Consolas");
			view.MeasuresFontSize = 14;
			view.MeasuresBackground = Brushes.Black;
			view.MeasuresForeground = Brushes.Yellow;
		}
	}

	sealed class TestReactorAccelerated : AcceleratedReactor
	{
		private static readonly AccelerationDevice mDevice = new AccelerationDevice();

		public TestReactorAccelerated() : base(mDevice)
		{
		}

		protected override AcceleratedImagingKernel CreateOutputKernel()
		{
			var kernelWriter = new KernelSourceWriter();
			kernelWriter.Write(@"

				float4 main(float2 uv  : POSITION) : SV_Target {

					return float4(
						uv.x < 0.5 && uv.y < 0.5f ? 1 : 0,
						uv.x >= 0.5f && uv.y < 0.5f ? 1 : 0,
						uv.x < 0.5f && uv.y >= 0.5f ? 1 : 0,
						uv.x >= 0.5f && uv.y >= 0.5f ? 0 : 1);
				}

			");

			return AcceleratedImagingKernel.FromSource(ComputationProvider, kernelWriter).TextureSize(512,512);
		}
	}

	sealed class TestReactor : Reactor
	{
		private Kernel mKernel;

		protected override void InitializeOverride()
		{
			mKernel = new Kernel(BackBufferWidth, BackBufferHeight);
			//mKernel.ParallelOptions = new ParallelOptions {MaxDegreeOfParallelism = 1};
		}
		protected override void UpdateOverride(IRenderLoop renderLoop, IRenderContext context)
		{
			mKernel.Phase += 0.01f;
			mKernel.Execute();

			context.Blit(mKernel.OutputMemory);
		}

		class Kernel : SoftwareImagingKernel
		{
			public Kernel(int width, int height)
			{
				TextureSize = new Int2(width, height);
			}

			public float Phase { get; set; }
			
			public override Int2 TextureSize { get; }

			protected override Float4 ExecuteOverride(SoftwareImagingKernelContext context)
			{
				return new Float4(
					context.TextureUV.x < 0.5f && context.TextureUV.y < 0.5f ? 1 : 0,
					context.TextureUV.x >= 0.5f && context.TextureUV.y < 0.5f ? 1 : 0,
					context.TextureUV.x < 0.5f && context.TextureUV.y >= 0.5f ? 1 : 0,
					context.TextureUV.x >= 0.5f && context.TextureUV.y >= 0.5f ? 0 : 1);
				//return new Float4(context.TextureUV * (1 + 0.5f * new Float2((Mathf.PI * Phase).Sin(), (Mathf.PI * Phase).Cos())), new Float2(1, 1));
			}
		}
	}
}
