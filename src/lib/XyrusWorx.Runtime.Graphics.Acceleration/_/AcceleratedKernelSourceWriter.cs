using System;
using System.Linq.Expressions;
using JetBrains.Annotations;
using XyrusWorx.Runtime.Expressions;

namespace XyrusWorx.Runtime.Graphics
{
	[PublicAPI]
	public class AcceleratedKernelSourceWriter
	{
		private const string mEntryPoint = "__main";
		private AcceleratedComputationKernelProfile mProfile;

		private AcceleratedKernelWriterContext mContext;
		private AcceleratedKernelSourceCodeProcessor mProcessor;

		public AcceleratedKernelSourceWriter()
		{
			mContext = new AcceleratedKernelWriterContext();
			mProfile = AcceleratedComputationKernelProfile.DirectCompute5;
			mProcessor = new AcceleratedKernelSourceCodeProcessor(mContext);
		}

		public void Add([NotNull] Define define)
		{
			if (define == null)
			{
				throw new ArgumentNullException(nameof(define));
			}

			mContext.Defines[define.Name] = Expression.Constant(define.GetValue());
		}
		public void Add([NotNull] ConstantBufferDefinition definition)
		{
			if (definition == null)
			{
				throw new ArgumentNullException(nameof(definition));
			}

			mContext.ConstantBuffers[definition.Name] = definition;
		}
		public void Add([NotNull] Function function)
		{
			if (function == null) throw new ArgumentNullException(nameof(function));

			mProcessor.WriteFunction(function);
		}

		[NotNull]
		public AcceleratedKernelWriterContext Context => mContext;

		internal string SourceCode => mProcessor.GetSource();
		internal static string EntryPointName = mEntryPoint;
	}
}