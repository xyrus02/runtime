using System;
using System.Runtime.Serialization;
using JetBrains.Annotations;

namespace XyrusWorx.Runtime.Expressions 
{
	[PublicAPI, Serializable]
	public class CompilerException : Exception
	{
		public CompilerException() { }
		public CompilerException(string message) : base(message) { }
		public CompilerException(string message, Exception innerException) : base(message, innerException) { }
		
		protected CompilerException(SerializationInfo info, StreamingContext context) : base(info, context) { }
	}
}