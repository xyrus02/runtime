using JetBrains.Annotations;

namespace XyrusWorx.Runtime 
{
	[PublicAPI]
	public interface IResourcePool<in T>
	{
		[CanBeNull]
		T this[int slot] { set; }
	}
}