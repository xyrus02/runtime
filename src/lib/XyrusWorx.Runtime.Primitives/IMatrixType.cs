using System;
using JetBrains.Annotations;

namespace XyrusWorx.Runtime
{
	[PublicAPI]
	public interface IMatrixType
	{
		IVectorType[] GetRows();
		IVectorType[] GetColumns();
		Type ComponentType { get; }
	}

	[PublicAPI]
	public interface IMatrixType<out T>
	{
		IVectorType<T>[] GetRows();
		IVectorType<T>[] GetColumns();
	}
}