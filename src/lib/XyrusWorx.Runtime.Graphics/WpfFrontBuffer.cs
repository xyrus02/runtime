using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using JetBrains.Annotations;

namespace XyrusWorx.Runtime.Graphics
{
	[PublicAPI]
	public sealed class WpfFrontBuffer : Control, IPresenter
	{
		public static DependencyProperty MeasuresFontFamilyProperty = DependencyProperty.Register("MeasuresFontFamily", typeof(FontFamily), typeof(WpfFrontBuffer), new FrameworkPropertyMetadata(new FontFamily("Courier"), FrameworkPropertyMetadataOptions.AffectsRender, OnMeasuresFontFamilyChanged));

		public static DependencyProperty MeasuresFontSizeProperty = DependencyProperty.Register("MeasuresFontSize", typeof(double), typeof(WpfFrontBuffer), new FrameworkPropertyMetadata(14.0, FrameworkPropertyMetadataOptions.AffectsRender));
		public static DependencyProperty MeasuresForegroundProperty = DependencyProperty.Register("MeasuresForeground", typeof(Brush), typeof(WpfFrontBuffer), new FrameworkPropertyMetadata(Brushes.White, FrameworkPropertyMetadataOptions.AffectsRender));
		public static DependencyProperty ShowFramesPerSecondProperty = DependencyProperty.Register("ShowFramesPerSecond", typeof(bool), typeof(WpfFrontBuffer), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender));
		public static DependencyProperty ShowClockProperty = DependencyProperty.Register("ShowClock", typeof(bool), typeof(WpfFrontBuffer), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender));
		
		private Typeface mMeasuresTypeFace;
		private WriteableBitmap mFrontBuffer;
		private IRenderLoop mRenderLoop;

		public WpfFrontBuffer()
		{
			DataContextChanged += OnDataContextChanged;
		}

		public FontFamily MeasuresFontFamily
		{
			get => GetValue(MeasuresFontFamilyProperty) as FontFamily;
			set => SetValue(MeasuresFontFamilyProperty, value);
		}
		public double MeasuresFontSize
		{
			get => (double)GetValue(MeasuresFontSizeProperty);
			set => SetValue(MeasuresFontSizeProperty, value);
		}
		public Brush MeasuresForeground
		{
			get => GetValue(MeasuresForegroundProperty) as Brush;
			set => SetValue(MeasuresForegroundProperty, value);
		}
		
		public bool ShowFramesPerSecond
		{
			get => (bool)GetValue(ShowFramesPerSecondProperty);
			set => SetValue(ShowFramesPerSecondProperty, value);
		}
		public bool ShowClock
		{
			get => (bool)GetValue(ShowClockProperty);
			set => SetValue(ShowClockProperty, value);
		}
	
		void IPresenter.Present(IReactor reactor, IRenderLoop renderLoop)
		{
			if (reactor == null)
			{
				throw new ArgumentNullException(nameof(reactor));
			}
			
			if (renderLoop == null)
			{
				throw new ArgumentNullException(nameof(renderLoop));
			}

			mRenderLoop = renderLoop;

			if (mFrontBuffer != null)
			{
				var fbArea = new Int32Rect(0, 0, mFrontBuffer.PixelWidth, mFrontBuffer.PixelHeight);
				var fbLength = reactor.BackBufferStride * reactor.BackBufferHeight;
				var fbStride = mFrontBuffer.PixelWidth * 4;
				
				mFrontBuffer.WritePixels(fbArea, reactor.BackBuffer, fbLength, fbStride);
			}
			
			InvalidateVisual();
		}
		protected override void OnRender(DrawingContext drawingContext)
		{
			base.OnRender(drawingContext);

			var area = new Rect(0, 0, ActualWidth, ActualHeight);
			
			drawingContext.DrawRectangle(Background, null, area);
			
			if (mFrontBuffer != null)
			{
				drawingContext.DrawImage(mFrontBuffer, area);
			}

			double measuresOffset = 5;
			
			if (ShowFramesPerSecond)
			{
				if (mMeasuresTypeFace == null)
				{
					UpdateTypeFace();
				}
				
				var formattedText = new FormattedText($"{mRenderLoop?.FramesPerSecond:###,###,###,##0.00} fps", CultureInfo.CurrentCulture, FlowDirection.LeftToRight, mMeasuresTypeFace, MeasuresFontSize, MeasuresForeground);
				
				drawingContext.DrawText(formattedText, new Point(area.Width - 5 - formattedText.Width, measuresOffset));
				measuresOffset += formattedText.Height + 2;
			}
				
			if (ShowClock)
			{
				if (mMeasuresTypeFace == null)
				{
					UpdateTypeFace();
				}
				
				var formattedText = new FormattedText($"{mRenderLoop?.Clock:###,###,###,##0.00}s", CultureInfo.CurrentCulture, FlowDirection.LeftToRight, mMeasuresTypeFace, MeasuresFontSize, MeasuresForeground);
				
				drawingContext.DrawText(formattedText, new Point(area.Width - 5 - formattedText.Width, measuresOffset));
				// ReSharper disable once RedundantAssignment
				measuresOffset += formattedText.Height + 2;
			}
		}

		private void UpdateTypeFace()
		{
			mMeasuresTypeFace = new Typeface(MeasuresFontFamily ?? FontFamily, FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
		}

		private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			var renderLoop = e.NewValue as IRenderLoop;
			var reactor = renderLoop?.CurrentReactor;

			if (reactor == null)
			{
				mFrontBuffer = null;
				return;
			}

			var width = reactor.BackBufferStride >> 2;
			mFrontBuffer = new WriteableBitmap(width, reactor.BackBufferHeight, 96.0, 96.0, PixelFormats.Bgra32, null);
			
			Width = width;
			Height = reactor.BackBufferHeight;
		}
		private static void OnMeasuresFontFamilyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			d.CastTo<WpfFrontBuffer>()?.UpdateTypeFace();
		}
	}

}
