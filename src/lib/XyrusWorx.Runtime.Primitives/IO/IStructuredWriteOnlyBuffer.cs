using JetBrains.Annotations;

namespace XyrusWorx.Runtime.IO
{
	[PublicAPI]
	public interface IStructuredWriteOnlyBuffer : IStructuredBuffer
	{
		void Write<T>(T data, int index = 0) where T : struct;
	}
}