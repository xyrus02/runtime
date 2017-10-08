using JetBrains.Annotations;

namespace XyrusWorx.Runtime.Expressions
{
	[PublicAPI]
	public class KernelSourceWriter
	{
		private const string mEntryPoint = "main";

		private KernelSourceWriterContext mContext;
		private KernelSourceProcessor mProcessor;

		public KernelSourceWriter()
		{
			mContext = new KernelSourceWriterContext();
			mProcessor = new KernelSourceProcessor(mContext);
		}

		public void Write(string source)
		{
			mProcessor.WriteRaw(source);
		}
		
		[NotNull]
		public KernelSourceWriterContext Context => mContext;

		internal string SourceCode => mProcessor.GetSource();
		internal static string EntryPointName = mEntryPoint;
	}
}