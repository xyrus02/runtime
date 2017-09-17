using JetBrains.Annotations;

namespace XyrusWorx.Runtime.Graphics
{
	[PublicAPI]
	public struct KernelIterationContext
	{
		public readonly Float4 AbsolutePosition;
		public readonly Float2 TextureUV;

		public KernelIterationContext(Float2 textureUV)
		{
			TextureUV = textureUV;
			AbsolutePosition = new Float4();
		}
	}
}