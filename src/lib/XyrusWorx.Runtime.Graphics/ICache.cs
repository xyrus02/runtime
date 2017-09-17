using JetBrains.Annotations;

namespace XyrusWorx.Runtime.Graphics 
{
	[PublicAPI]
	public interface ICache
	{
		void Clear();
		void Put(object key, object data);
		object Fetch(object key);
	}
}