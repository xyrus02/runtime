using System.Windows.Media;
using JetBrains.Annotations;

namespace XyrusWorx.Runtime
{
	[PublicAPI]
	public interface IWpfPresenter : IPresenter
	{
		FontFamily MeasuresFontFamily { get; set; }
		double MeasuresFontSize { get; set; }
		Brush MeasuresForeground { get; set; }
		Brush MeasuresBackground { get; set; }
	}
}