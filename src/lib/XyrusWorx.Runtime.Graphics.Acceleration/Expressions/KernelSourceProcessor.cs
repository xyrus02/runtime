using System;
using System.Text;
using JetBrains.Annotations;

namespace XyrusWorx.Runtime.Expressions
{
	class KernelSourceProcessor
	{
		private readonly KernelSourceWriterContext mContext;
		private readonly StringBuilder mOutput;
		
		public KernelSourceProcessor([NotNull] KernelSourceWriterContext context)
		{
			if (context == null)
			{
				throw new ArgumentNullException(nameof(context));
			}

			mContext = context;
			mOutput = new StringBuilder();
		}

		public void WriteRaw(string source)
		{
			mOutput.AppendLine(source);
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