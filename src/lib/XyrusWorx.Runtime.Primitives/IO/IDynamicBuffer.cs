using System;
using JetBrains.Annotations;

namespace XyrusWorx.Runtime.IO 
{
	[PublicAPI]
	public interface IDynamicBuffer : IDisposable
	{
		[NotNull]
		IDynamicBuffer SetValue([NotNull] string fieldName, [CanBeNull] object value);

		[NotNull]
		IStructuredReadWriteBuffer GetUnmanagedBuffer();
	}
}