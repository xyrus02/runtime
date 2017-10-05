using System;
using JetBrains.Annotations;
using Buffer = SlimDX.Direct3D11.Buffer;

namespace XyrusWorx.Runtime 
{
	[PublicAPI]
	public abstract class HardwareBuffer : LinkableHardwareResource
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