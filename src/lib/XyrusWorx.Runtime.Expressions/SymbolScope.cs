using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using XyrusWorx.Collections;

namespace XyrusWorx.Runtime.Expressions
{
	[PublicAPI]
	public class SymbolScope<T> : Scope, IEnumerable<T>, IReadOnlyDictionary<string, T>
	{
		private readonly Dictionary<string, T> mData;

		public SymbolScope()
		{
			mData = new Dictionary<string, T>();
		}

		bool IReadOnlyDictionary<string, T>.ContainsKey(string key) => mData.ContainsKey(key);
		bool IReadOnlyDictionary<string, T>.TryGetValue(string key, out T value) => mData.TryGetValue(key, out value);

		public T this[string name]
		{
			get => mData.GetValueByKeyOrDefault(name);
			set => mData.AddOrUpdate(name, value);
		}
		public IEnumerable<string> Keys
		{
			get => mData.Keys;
		}
		public IEnumerable<T> Values
		{
			get => mData.Values;
		}

		IEnumerator<KeyValuePair<string, T>> IEnumerable<KeyValuePair<string, T>>.GetEnumerator() => mData.GetEnumerator();
		IEnumerator<T> IEnumerable<T>.GetEnumerator() => mData.Values.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => mData.Values.GetEnumerator();

		public int Count { get; }
	}
}