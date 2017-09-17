using JetBrains.Annotations;

namespace XyrusWorx.Runtime.IO 
{
	[PublicAPI]
	public interface ICache
	{
		void Clear();
		void Put(object key, object data);
		object Fetch(object key);
	}
}