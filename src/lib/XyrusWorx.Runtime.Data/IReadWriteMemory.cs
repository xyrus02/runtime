using JetBrains.Annotations;

namespace XyrusWorx.Runtime 
{
	[PublicAPI]
	public interface IReadWriteMemory : IReadableMemory, IWritableMemory
	{
	}
}