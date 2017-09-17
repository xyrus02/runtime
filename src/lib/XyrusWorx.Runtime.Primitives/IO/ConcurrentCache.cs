using System.Collections.Concurrent;
using JetBrains.Annotations;
using XyrusWorx.Collections;

namespace XyrusWorx.Runtime.IO 
{
	[PublicAPI]
	public sealed class ConcurrentCache : ICache
	{
		private readonly ConcurrentDictionary<object, object> mData;

		public ConcurrentCache()
		{
			mData = new ConcurrentDictionary<object, object>();
		}

		public void Clear()
		{
			mData.Clear();
		}
		public void Put(object key, object data) => mData.AddOrUpdate(key, data);
		public object Fetch(object key) => mData.GetValueByKeyOrDefault(key);
	}
}