using JetBrains.Annotations;

namespace XyrusWorx.Runtime.Imaging 
{
	[PublicAPI]
	public interface IImagingKernel
	{
		[NotNull]
		IResourcePool<IWritableMemory> Constants { get; }
		
		[NotNull]
		IResourcePool<IWritableMemory> Textures { get; }
		
		[NotNull]
		IReadableTexture Output { get; }

		void Execute();
	}
}