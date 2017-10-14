using JetBrains.Annotations;

namespace XyrusWorx.Runtime 
{
	[PublicAPI]
	public interface IMatrixCellWriter
	{
		IMatrix Set(int column, int row, object value);
	}

}