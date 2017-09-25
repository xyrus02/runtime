using System;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace XyrusWorx.Runtime.Graphics.IO
{
	[PublicAPI]
	public abstract class HardwareBufferResource : Resource
	{
		[DllImport("kernel32.dll")]
		internal static extern void CopyMemory(IntPtr destination, IntPtr source, int length);
		
		public abstract int BufferSize { get; }
		public abstract int ElementCount { get; }

		protected virtual void OnCleanup(){}
		protected sealed override void DisposeOverride()
		{
			OnCleanup();
		}
	}
}