using System;
using System.Windows.Media;
using JetBrains.Annotations;

namespace XyrusWorx.Runtime 
{
	[PublicAPI]
	public interface IVectorBuffer
	{
		[NotNull]
		IDisposable BeginFrame();
		
		[NotNull]
		IVectorFrame GetCurrentFrame();
		
		[CanBeNull]
		DrawingGroup ToDrawingGroup();
	}
}