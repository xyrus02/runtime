using System.Collections.Generic;
using System.Collections.ObjectModel;
using JetBrains.Annotations;
using XyrusWorx.Diagnostics;

namespace XyrusWorx.Runtime 
{
	[PublicAPI]
	public sealed class CompilerContext
	{
		private readonly ILogWriter mLog;
		private List<LogMessage> mMessages;

		public CompilerContext()
		{
			mMessages = new List<LogMessage>();
			mLog = new DelegateLogWriter(mMessages.Add);
			Messages = new ReadOnlyCollection<LogMessage>(mMessages);
		}
		
		[NotNull]
		public IReadOnlyList<LogMessage> Messages { get; }

		[NotNull]
		internal ILogWriter Writer => mLog;
	}
}