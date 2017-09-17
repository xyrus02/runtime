using System;
using JetBrains.Annotations;

namespace XyrusWorx.Runtime
{
	[PublicAPI]
	public interface IMatrix
	{
		IVector[] GetRows();
		IVector[] GetColumns();
		Type ComponentType { get; }
	}

	[PublicAPI]
	public interface IMatrix<out T>
	{
		IVector<T>[] GetRows();
		IVector<T>[] GetColumns();
		IVector<T> this[int i] { get; }
	}
}