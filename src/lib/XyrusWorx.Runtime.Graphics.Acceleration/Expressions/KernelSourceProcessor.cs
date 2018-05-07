using System.Text;
using JetBrains.Annotations;

namespace XyrusWorx.Runtime.Expressions
{
	class KernelSourceProcessor
	{
		private readonly StringBuilder mOutput;
		
		public KernelSourceProcessor()
		{
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

			result.AppendLine(mOutput.ToString());

			return result.ToString();
		}
	}
}