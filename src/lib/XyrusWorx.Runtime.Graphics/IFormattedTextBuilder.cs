using System.Globalization;
using System.Windows;
using System.Windows.Media;
using JetBrains.Annotations;

namespace XyrusWorx.Runtime 
{
	[PublicAPI]
	public interface IFormattedTextBuilder
	{
		[NotNull]
		[Pure]
		FormattedText Create();
		
		[NotNull]
		IFormattedTextBuilder Culture([NotNull] CultureInfo cultureInfo);
		
		[NotNull]
		IFormattedTextBuilder Foreground([NotNull] Brush foreground);
		
		[NotNull]
		IFormattedTextBuilder NumberSubstitution([NotNull] NumberSubstitution numberSubstitution);
		
		[NotNull]
		IFormattedTextBuilder FlowDirection(FlowDirection flowDirection);
		
		[NotNull]
		IFormattedTextBuilder TextFormattingMode(TextFormattingMode textFormattingMode);
	}
}