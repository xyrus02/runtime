using System;
using JetBrains.Annotations;

namespace XyrusWorx.Runtime.Expressions 
{
	[PublicAPI]
	public interface IDynamicBufferAppender
	{
		[NotNull]
		IDynamicBufferBuilder Field([NotNull] string fieldName, [NotNull] Type fieldType);
	}
}