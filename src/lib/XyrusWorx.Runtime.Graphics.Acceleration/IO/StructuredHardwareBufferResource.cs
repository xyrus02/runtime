using JetBrains.Annotations;
using SlimDX.Direct3D11;
using D3DBuffer = SlimDX.Direct3D11.Buffer;

namespace XyrusWorx.Runtime.Graphics.IO
{
	[PublicAPI]
	public abstract class StructuredHardwareBufferResource : HardwareBufferResource
	{
		internal abstract D3DBuffer HardwareBuffer { get; }
		internal abstract ShaderResourceView View { get; }

		protected override void OnCleanup()
		{
			HardwareBuffer?.Dispose();
		}
	}
}