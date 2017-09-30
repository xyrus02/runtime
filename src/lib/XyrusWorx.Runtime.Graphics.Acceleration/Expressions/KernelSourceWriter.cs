using System;
using System.Linq.Expressions;
using JetBrains.Annotations;

namespace XyrusWorx.Runtime.Expressions
{
	[PublicAPI]
	public class KernelSourceWriter
	{
		private const string mEntryPoint = "__main";

		private KernelSourceWriterContext mContext;
		private KernelSourceProcessor mProcessor;

		public KernelSourceWriter()
		{
			mContext = new KernelSourceWriterContext();
			mProcessor = new KernelSourceProcessor(mContext);
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
		public KernelSourceWriterContext Context => mContext;

		internal string SourceCode => mProcessor.GetSource();
		internal static string EntryPointName = mEntryPoint;
	}
}