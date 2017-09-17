using JetBrains.Annotations;

namespace XyrusWorx.Runtime.Graphics
{
	[PublicAPI]
	public enum AcceleratedComputationKernelProfile
	{
		DirectCompute4,
		DirectCompute5,

		PixelShader40,
		PixelShader50
	}
}