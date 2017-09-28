using System;
using System.Text;
using JetBrains.Annotations;
using XyrusWorx.Runtime.Expressions;

namespace XyrusWorx.Runtime.Graphics
{
	class AcceleratedKernelSourceCodeProcessor
	{
		private readonly AcceleratedKernelWriterContext mContext;
		private readonly StringBuilder mOutput;
		
		public AcceleratedKernelSourceCodeProcessor([NotNull] AcceleratedKernelWriterContext context)
		{
			if (context == null)
			{
				throw new ArgumentNullException(nameof(context));
			}

			mContext = context;
			mOutput = new StringBuilder();
		}

		public void WriteFunction([NotNull] Function function)
		{
			if (function == null)
			{
				throw new ArgumentNullException(nameof(function));
			}

			function.Write(mOutput, mContext);
		}

		[NotNull]
		public string GetSource()
		{
			var result = new StringBuilder();

			result.AppendLine(mContext.GetProgramHeader());
			result.AppendLine(mOutput.ToString());

			return result.ToString();
		}
	}
}