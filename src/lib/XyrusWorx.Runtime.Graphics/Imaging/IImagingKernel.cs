using JetBrains.Annotations;

namespace XyrusWorx.Runtime.Imaging 
{
	[PublicAPI]
	public interface IImagingKernel
	{
		[NotNull]
		IResourcePool<IWritable> Constants { get; }
		
		[NotNull]
		IResourcePool<IWritable> Textures { get; }
		
		[NotNull]
		IReadable Output { get; }

		void Execute();
	}
}