using System;
using System.Threading;
using XyrusWorx.Diagnostics;
using XyrusWorx.Runtime.Computation;
using XyrusWorx.Runtime.Expressions;
using XyrusWorx.Runtime.Imaging;

namespace XyrusWorx.Runtime.ComputeShaderTest
{
	class Program : ConsoleApplication
	{
		public static void Main() => new Program().Run();
		
		protected override IResult Execute(CancellationToken cancellationToken)
		{
			Log.Verbosity = LogVerbosity.Debug;
			
			using (var device = new AccelerationDevice(Log))
			{
				Execute(device, cancellationToken);
			}

			Console.ReadKey();
			
			return Result.Success;
		}

		private static AcceleratedComputationKernel GetKernel(AccelerationDevice device)
		{
			var kernelWriter = new KernelSourceWriter();
			kernelWriter.Write(@"

				cbuffer cb : register(b0) {
					int x;
				}

				StructuredBuffer<int> b_in : register(t0);
				RWStructuredBuffer<int> b_out : register(u0);

				[numthreads(10, 1, 1)]
				void main(uint3 id : SV_DispatchThreadId) {
					b_out[id.x] = x * b_in[id.x];
				}

			");
			
			return AcceleratedComputationKernel.FromSource(device,kernelWriter);
		}
		private static void Execute(AccelerationDevice device, CancellationToken cancellationToken)
		{
			using (var kernel = GetKernel(device))
			{
				using(var cb = new HardwareConstantBuffer<int>(device))
				using(var bIn = new HardwareInputBuffer(device, typeof(int), 10))
				using(var bOut = new HardwareOutputBuffer(device, typeof(int), 10))
				{
					kernel.ThreadGroupCount = new Vector3<uint>(10, 1, 1);
					kernel.Constants[0] = cb;
					kernel.Resources[0] = bIn;
					kernel.Outputs[0] = bOut;
					cb.Structure = 2;

					var inData = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
					var outData = new int[inData.Length];

					var bInArrayView = new ArrayWriter<int>(bIn);
					var bOutArrayView = new ArrayReader<int>(bOut);

					bInArrayView.Write(inData);
					kernel.Execute();
					bOutArrayView.Read(outData);

					using (WithColor(ConsoleColor.White, ConsoleColor.DarkBlue))
					{
						Console.WriteLine($"[ {string.Join(", ", inData)} ] => [ {string.Join(", ", outData)} ]");
					}
				}
			}
		}
	}
}
