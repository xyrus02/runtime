using System;
using XyrusWorx.Runtime.Computation;
using XyrusWorx.Runtime.Expressions;

namespace XyrusWorx.Runtime.ComputeShaderTest
{
	class Program
	{
		public static void Main()
		{
			var device = new AccelerationDevice();

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
			
			var kernel = AcceleratedComputationKernel.FromSource(device,kernelWriter);
			
			var cb = new HardwareConstantBuffer<int>(device);
			var bIn = new HardwareInputBuffer(device, typeof(int), 10);
			var bOut = new HardwareOutputBuffer(device, typeof(int), 10);

			kernel.ThreadGroupCount = new Vector3<uint>(10);
			kernel.Constants[0] = cb;
			kernel.Resources[0] = bIn;
			kernel.Outputs[0] = bOut;
			
			cb.Structure = 2;
			
			var bInArrayView = new ArrayWriter<int>(bIn);
			var bOutArrayView = new ArrayReader<int>(bOut);
			
			bInArrayView.Write(0 /* <-- index | values --> */, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10);
			kernel.Execute();

			var outArray = bOutArrayView.Read(0, 10);
			for (var i = 0; i < 10; i++)
			{
				Console.WriteLine(outArray[i]);
			}
			
			Console.ReadKey();
		}
	}
}
