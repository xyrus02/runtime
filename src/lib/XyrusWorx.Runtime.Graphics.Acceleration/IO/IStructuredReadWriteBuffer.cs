using JetBrains.Annotations;

namespace XyrusWorx.Runtime.IO
{
	[PublicAPI]
	public interface IStructuredReadWriteBuffer : IStructuredReadOnlyBuffer, IStructuredWriteOnlyBuffer
	{
	}
}