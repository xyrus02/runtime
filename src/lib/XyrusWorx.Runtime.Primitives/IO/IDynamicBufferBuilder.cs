using System;
using JetBrains.Annotations;

namespace XyrusWorx.Runtime.IO 
{
	[PublicAPI]
	public interface IDynamicBufferBuilder : IDynamicBufferAppender, IDisposable
	{
		[NotNull]
		IDynamicBuffer Commit();
	}
}