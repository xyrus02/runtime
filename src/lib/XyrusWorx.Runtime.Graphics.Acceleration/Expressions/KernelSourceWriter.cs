using JetBrains.Annotations;

namespace XyrusWorx.Runtime.Expressions
{
	[PublicAPI]
	public class KernelSourceWriter
	{
		private const string mEntryPoint = "main";

		private KernelSourceProcessor mProcessor;

		public KernelSourceWriter()
		{
			mProcessor = new KernelSourceProcessor();
		}

		public void Write(string source)
		{
			mProcessor.WriteRaw(source);
		}
		
		internal string SourceCode => mProcessor.GetSource();
		internal static string EntryPointName = mEntryPoint;
	}
}