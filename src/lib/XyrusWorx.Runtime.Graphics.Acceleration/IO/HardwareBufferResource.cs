using JetBrains.Annotations;

namespace XyrusWorx.Runtime.Graphics.IO
{
	[PublicAPI]
	public abstract class HardwareBufferResource : Resource
	{
		public abstract int BufferSize { get; }
		public abstract int ElementCount { get; }

		protected virtual void OnCleanup(){}
		protected sealed override void DisposeOverride()
		{
			OnCleanup();
		}
	}
}