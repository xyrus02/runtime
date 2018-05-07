using System;
using System.Runtime.Serialization;
using JetBrains.Annotations;

namespace XyrusWorx.Runtime.Expressions
{
	[PublicAPI, Serializable]
	public class KernelCompileException : Exception
	{
		public KernelCompileException() { }
		public KernelCompileException(string message) : base(message) { }
		public KernelCompileException(string message, Exception innerException) : base(message, innerException) { }

		protected KernelCompileException(SerializationInfo info, StreamingContext context) : base(info, context) { }
	}
}