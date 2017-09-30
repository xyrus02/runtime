using System;
using System.Runtime.Serialization;
using JetBrains.Annotations;

namespace XyrusWorx.Runtime.Expressions
{
	[PublicAPI, Serializable]
	public class KernelSourceException : Exception
	{
		public KernelSourceException() { }
		public KernelSourceException(string message) : base(message) { }
		public KernelSourceException(string message, Exception innerException) : base(message, innerException) { }

		protected KernelSourceException(SerializationInfo info, StreamingContext context) : base(info, context) { }
	}
}