using System.Runtime.Serialization;
using XyrusWorx.Diagnostics;

namespace XyrusWorx.Runtime.Graphics
{
	[DataContract]
	public class AcceleratedKernelCompilerMessage : Result
	{
		[DataMember]
		public LogMessageClass Type { get; set; }

		[DataMember]
		public string Message { get; set; }

		public uint Code { get; set; }

		[DataMember]
		public int LineNumber { get; set; }

		[DataMember]
		public int ColumnNumber { get; set; }
	}
}