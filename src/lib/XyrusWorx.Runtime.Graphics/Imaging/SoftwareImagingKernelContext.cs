using JetBrains.Annotations;

namespace XyrusWorx.Runtime.Graphics.Imaging
{
	[PublicAPI]
	public struct SoftwareImagingKernelContext
	{
		public readonly Float4 AbsolutePosition;
		public readonly Float2 TextureUV;

		public SoftwareImagingKernelContext(Float2 textureUV)
		{
			TextureUV = textureUV;
			AbsolutePosition = new Float4();
		}
	}
}