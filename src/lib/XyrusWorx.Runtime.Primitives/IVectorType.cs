using System;
using JetBrains.Annotations;

namespace XyrusWorx.Runtime
{
	[PublicAPI]
	public interface IVectorType
	{
		object[] GetComponents();
		Type ComponentType { get; }
	}

	[PublicAPI]
	public interface IVectorType<out T>
	{
		T[] GetComponents();
	}
}