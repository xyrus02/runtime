using System;
using JetBrains.Annotations;

namespace XyrusWorx.Runtime.Expressions 
{
	[PublicAPI]
	public interface IDynamicBufferBuilder : IDynamicBufferAppender, IDisposable
	{
		[NotNull]
		IDynamicBuffer Commit();
	}
}