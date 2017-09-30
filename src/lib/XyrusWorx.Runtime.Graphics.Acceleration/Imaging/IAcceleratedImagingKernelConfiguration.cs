using JetBrains.Annotations;

namespace XyrusWorx.Runtime.Imaging 
{
	[PublicAPI]
	public interface IAcceleratedImagingKernelConfiguration
	{
		[NotNull]
		AcceleratedImagingKernel TextureSize(Int2 size);
		
		[NotNull]
		AcceleratedImagingKernel TextureSize(int width, int height);
	}
}