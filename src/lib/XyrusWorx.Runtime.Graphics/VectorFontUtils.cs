using System;
using System.Globalization;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using JetBrains.Annotations;

namespace XyrusWorx.Runtime 
{
	[PublicAPI]
	public static class VectorFontUtils
	{
		[NotNull]
		public static IFormattedTextBuilder Format(this IFontInfo fontInfo, string text) 
			=> new FormattedTextBuilder(fontInfo, text);

		[Pure]
		[ContractAnnotation("null => null")]
		public static Typeface ToTypeFace(this IFontInfo fontInfo) 
			=> fontInfo == null ? null : new Typeface(fontInfo.FontFamily, fontInfo.FontStyle, fontInfo.FontWeight, fontInfo.FontStretch);

		class FormattedTextBuilder : IFormattedTextBuilder
		{
			private readonly IFontInfo mFontInfo;
			private readonly string mText;
			private CultureInfo mCulture;
			private FlowDirection mFlowDirection;
			private Brush mForeground;
			private NumberSubstitution mNumberSubstitution;
			private TextFormattingMode mTextFormattingMode;
			private double mDpi;

			public FormattedTextBuilder([NotNull] IFontInfo fontInfo, string text)
			{
				mFontInfo = fontInfo ?? throw new ArgumentNullException(nameof(fontInfo));
				mText = text ?? string.Empty;
				
				mCulture = CultureInfo.CurrentCulture;
				mFlowDirection = System.Windows.FlowDirection.LeftToRight;
				mForeground = Brushes.Black;
				mNumberSubstitution = null;
				mTextFormattingMode = System.Windows.Media.TextFormattingMode.Display;
				
				mDpi = typeof(SystemParameters).GetProperty("Dpi", BindingFlags.NonPublic | BindingFlags.Static)?.GetValue(null, null) as double? ?? 96.0;
			}

			[Pure]
			public FormattedText Create()
			{
				#if(NET472)
					return new FormattedText(mText, mCulture, mFlowDirection, mFontInfo.ToTypeFace() ?? throw new ArgumentNullException(), mFontInfo.FontSize, mForeground, mNumberSubstitution, mTextFormattingMode, mDpi);
				#else
					return new FormattedText(mText, mCulture, mFlowDirection, mFontInfo.ToTypeFace() ?? throw new ArgumentNullException(), mFontInfo.FontSize, mForeground, mNumberSubstitution, mTextFormattingMode);
				#endif
			}

			public IFormattedTextBuilder Culture(CultureInfo cultureInfo)
			{
				if (cultureInfo == null)
				{
					throw new ArgumentNullException(nameof(cultureInfo));
				}

				mCulture = cultureInfo;
				return this;
			}
			public IFormattedTextBuilder Foreground(Brush foreground)
			{
				if (foreground == null)
				{
					throw new ArgumentNullException(nameof(foreground));
				}

				mForeground = foreground;
				return this;
			}
			public IFormattedTextBuilder NumberSubstitution(NumberSubstitution numberSubstitution)
			{
				mNumberSubstitution = numberSubstitution;
				return this;
			}
			public IFormattedTextBuilder FlowDirection(FlowDirection flowDirection)
			{
				mFlowDirection = flowDirection;
				return this;
			}
			public IFormattedTextBuilder TextFormattingMode(TextFormattingMode textFormattingMode)
			{
				mTextFormattingMode = textFormattingMode;
				return this;
			}

			public IFormattedTextBuilder Dpi(double dpi)
			{
				if (dpi <= 0 || double.IsNaN(dpi) || double.IsInfinity(dpi))
				{
					throw new ArgumentOutOfRangeException(nameof(dpi));
				}

				mDpi = dpi;
				return this;
			}
		}
	}
}