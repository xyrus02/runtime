using JetBrains.Annotations;

namespace XyrusWorx.Runtime 
{
	[PublicAPI]
	public interface IVectorRowWriter
	{
		IVector Set(int row, object value);
	}
}