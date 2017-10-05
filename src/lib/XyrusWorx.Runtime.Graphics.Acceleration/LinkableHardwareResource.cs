using JetBrains.Annotations;
using SlimDX.Direct3D11;
using XyrusWorx.Runtime.Imaging;

namespace XyrusWorx.Runtime 
{
	[PublicAPI]
	public abstract class LinkableHardwareResource : HardwareResource
	{
		protected LinkableHardwareResource([NotNull] AccelerationDevice device) : base(device) { }
		
		[NotNull]
		internal abstract ShaderResourceView GetShaderResourceView();
	}
}