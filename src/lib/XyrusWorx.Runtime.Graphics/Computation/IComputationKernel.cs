using JetBrains.Annotations;

namespace XyrusWorx.Runtime.Computation
{

	[PublicAPI]
	public interface IComputationKernel
	{
		Vector3<uint> ThreadGroupCount { get; set; }
		
		[NotNull]
		IResourcePool<IWritableMemory> Constants { get; }
		
		[NotNull]
		IResourcePool<IWritableMemory> Textures { get; }
		
		[NotNull]
		IResourcePool<IReadableMemory> Outputs { get; }

		void Execute();
	}

}