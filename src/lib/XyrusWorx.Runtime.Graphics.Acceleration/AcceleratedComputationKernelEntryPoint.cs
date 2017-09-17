using System;
using System.Text;
using JetBrains.Annotations;
using XyrusWorx.Runtime.Expressions;

namespace XyrusWorx.Runtime.Graphics
{
	[PublicAPI]
	public sealed class AcceleratedComputationKernelEntryPoint : Function
	{
		public AcceleratedComputationKernelEntryPoint([NotNull] VoidCustomFunctionBody body) : base(AcceleratedKernelSourceWriter.EntryPointName, body)
		{
			Threads = new Vector3<uint>(32, 32, 1);

			body.Parameters.Clear();
			body.Parameters.Add(new Symbol("id", typeof(Vector3<uint>)));
		}

		public Vector3<uint> Threads { get; set; }

		public override void Write(StringBuilder builder, IKernelWriterContext context)
		{
			builder.AppendFormat("[numthreads({0}, {1}, {2})]" + Environment.NewLine, Threads.x, Threads.y, Threads.z);
			base.Write(builder, context);
		}
	}
}