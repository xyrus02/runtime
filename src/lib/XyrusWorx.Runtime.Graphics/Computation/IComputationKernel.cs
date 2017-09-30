using JetBrains.Annotations;

namespace XyrusWorx.Runtime.Computation
{
	[PublicAPI]
	public interface IComputationKernel
	{
		Vector3<uint> ThreadGroupCount { get; set; }
		
		[NotNull]
		IResourcePool<IWritable> Constants { get; }
		
		[NotNull]
		IResourcePool<IWritable> Resources { get; }
		
		[NotNull]
		IResourcePool<IReadable> Outputs { get; }

		void Execute();
	}

}