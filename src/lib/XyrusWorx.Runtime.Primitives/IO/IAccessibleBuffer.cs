using System;
using JetBrains.Annotations;

namespace XyrusWorx.Runtime.IO
{
	[PublicAPI]
	public interface IAccessibleBuffer<TData, in TAddress> : IBuffer<TData>
	{
		TData Get(TAddress address);
		void Set(TAddress address, TData data);
		void Load(IntPtr dataPtr, int dataLength);
		void Flush();
	}
}