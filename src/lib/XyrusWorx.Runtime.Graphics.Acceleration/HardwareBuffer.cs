using System;
using JetBrains.Annotations;
using XyrusWorx.Runtime.Imaging;
using Buffer = SlimDX.Direct3D11.Buffer;

namespace XyrusWorx.Runtime 
{
	[PublicAPI]
	public abstract class HardwareBuffer : HardwareResource
	{
		internal HardwareBuffer([NotNull] AccelerationDevice provider, long size) : base(provider)
		{
			if (size <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(size));
			}
			
			Size = size;
		}
		
		public long Size { get; }
		
		

		internal abstract Buffer GetBuffer();
	}
}