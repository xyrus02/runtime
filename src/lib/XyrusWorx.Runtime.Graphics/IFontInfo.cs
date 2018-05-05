using System.Windows;
using System.Windows.Media;
using JetBrains.Annotations;

namespace XyrusWorx.Runtime 
{
	[PublicAPI]
	public interface IFontInfo
	{
		FontFamily FontFamily { get; }
		FontWeight FontWeight { get; }
		FontStyle FontStyle { get; }
		FontStretch FontStretch { get; }
		double FontSize { get; }
	}
}