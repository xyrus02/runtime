using System;
using JetBrains.Annotations;

namespace XyrusWorx.Runtime
{
	[PublicAPI]
	public interface IVector
	{
		object[] GetComponents();
		Type ComponentType { get; }
	}

	[PublicAPI]
	public interface IVector<out T>
	{
		T[] GetComponents();
		T this[int i] { get; }
	}
}