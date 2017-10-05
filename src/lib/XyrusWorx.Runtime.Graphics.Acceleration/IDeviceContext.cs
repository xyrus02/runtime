using JetBrains.Annotations;
using XyrusWorx.Diagnostics;

namespace XyrusWorx.Runtime 
{
	[PublicAPI]
	public interface IDeviceContext 
	{
		[NotNull]
		ILogWriter DiagnosticsWriter { get; }
		
		bool IsDebugModeEnabled { get; }
	}
}