using System;
using JetBrains.Annotations;

namespace XyrusWorx.Runtime.IO 
{
	[PublicAPI]
	public interface IDynamicBufferAppender
	{
		[NotNull]
		IDynamicBufferBuilder Field([NotNull] string fieldName, [NotNull] Type fieldType);
	}
}