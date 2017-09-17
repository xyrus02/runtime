using System;
using System.Runtime.Serialization;
using JetBrains.Annotations;

namespace XyrusWorx.Runtime.Graphics
{
	[PublicAPI, Serializable]
	public class AcceleratedKernelWriterException : Exception
	{
		public AcceleratedKernelWriterException() { }
		public AcceleratedKernelWriterException(string message) : base(message) { }
		public AcceleratedKernelWriterException(string message, Exception innerException) : base(message, innerException) { }

		protected AcceleratedKernelWriterException(SerializationInfo info, StreamingContext context) : base(info, context) { }
	}
}