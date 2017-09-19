using System;
using XyrusWorx.Runtime.Graphics;
using XyrusWorx.Runtime.Graphics.IO;

namespace XyrusWorx.Runtime.ComputeShaderTest
{
	class Program
	{
		public static void Main()
		{
			var provider = new AcceleratedComputationProvider();
			var source = new AcceleratedKernelSource(@"

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
			
			var kernel = new AcceleratedComputationKernel(provider, source.Compile());
			var cb = new StructuredHardwareResource<int>(provider);
			var b_in = new StructuredHardwareInputBufferResource<int>(provider, 10);
			var b_out = new StructuredHardwareOutputBufferResource<int>(provider, 10);

			kernel.Constants.Add(cb);
			kernel.Resources.Add(b_in);
			kernel.Outputs.Add(b_out);
			
			cb.Data = 2;
			b_in.Write(new[]{1,2,3,4,5,6,7,8,9,10});
			
			kernel.Compute();

			var sw = new ComputationSwapBufferResource<int>(provider, 10);
			
			sw.FetchResource(b_out);

			using (var str = sw.Read())
			{
				var out_buf = new int[10];
				str.Read(out_buf, 0, 10);

				for (var i = 0; i < 10; i++)
				{
					Console.WriteLine(out_buf[i]);
				}
			}
			
			Console.ReadKey();
		}
	}
}
