using JetBrains.Annotations;

namespace XyrusWorx.Runtime.IO
{
	[PublicAPI]
	public interface IStructuredReadOnlyBuffer : IStructuredBuffer
	{
		T Read<T>(int index = 0) where T : struct;
	}
}