using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using JetBrains.Annotations;
using XyrusWorx.Runtime.Imaging;

namespace XyrusWorx.Runtime
{

	[PublicAPI]
	public sealed class WpfPresenter : Control, IPresenter, IFontInfo
	{
		public static DependencyProperty MeasuresFontFamilyProperty = DependencyProperty.Register("MeasuresFontFamily", typeof(FontFamily), typeof(WpfPresenter), new FrameworkPropertyMetadata(new FontFamily("Courier"), FrameworkPropertyMetadataOptions.AffectsRender, OnMeasuresFontFamilyChanged));

		public static DependencyProperty MeasuresFontSizeProperty = DependencyProperty.Register("MeasuresFontSize", typeof(double), typeof(WpfPresenter), new FrameworkPropertyMetadata(14.0, FrameworkPropertyMetadataOptions.AffectsRender));
		public static DependencyProperty MeasuresForegroundProperty = DependencyProperty.Register("MeasuresForeground", typeof(Brush), typeof(WpfPresenter), new FrameworkPropertyMetadata(Brushes.White, FrameworkPropertyMetadataOptions.AffectsRender));
		public static DependencyProperty MeasuresBackgroundProperty = DependencyProperty.Register("MeasuresBackground", typeof(Brush), typeof(WpfPresenter), new FrameworkPropertyMetadata(Brushes.Transparent, FrameworkPropertyMetadataOptions.AffectsRender));
		public static DependencyProperty ShowFramesPerSecondProperty = DependencyProperty.Register("ShowFramesPerSecond", typeof(bool), typeof(WpfPresenter), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender));
		public static DependencyProperty ShowClockProperty = DependencyProperty.Register("ShowClock", typeof(bool), typeof(WpfPresenter), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender));

		private readonly Scope mPresentationScope = new Scope();
		
		private Typeface mMeasuresTypeFace;
		private WriteableBitmap mBitmap;
		private IRenderLoop mRenderLoop;

		public WpfPresenter()
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
		public Brush MeasuresBackground
		{
			get => GetValue(MeasuresBackgroundProperty) as Brush;
			set => SetValue(MeasuresBackgroundProperty, value);
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
		
		public void InvalidateBackBuffer()
		{
			var renderLoop = DataContext.CastTo<IRenderLoop>();
			var reactor = renderLoop?.CurrentReactor;

			if (reactor == null)
			{
				mBitmap = null;
				return;
			}

			var width = reactor.BackBuffer.Stride >> 2;
			mBitmap = new WriteableBitmap(width, reactor.BackBuffer.Height, 96.0, 96.0, PixelFormats.Bgra32, null);
			
			Width = width;
			Height = reactor.BackBuffer.Height;
		}
	
		unsafe void IPresenter.Present(IReactor reactor, IRenderLoop renderLoop)
		{
			if (reactor == null)
			{
				throw new ArgumentNullException(nameof(reactor));
			}
			
			if (renderLoop == null)
			{
				throw new ArgumentNullException(nameof(renderLoop));
			}

			if (mPresentationScope.IsInScope)
			{
				return;
			}

			using (mPresentationScope.Enter())
			{
				mRenderLoop = renderLoop;
				
				if (mBitmap != null)
				{
					Parallel.For(0, mBitmap.BackBufferStride * mBitmap.PixelHeight, i => 
						*((uint*)mBitmap.BackBuffer.ToPointer()) = TextureFormat.Bgra.Map(reactor.BackBuffer[i], TextureFormat.Rgba));
					
					reactor.BackBuffer.Read(mBitmap.BackBuffer, 0, mBitmap.BackBufferStride * mBitmap.PixelHeight);
				}
			
				InvalidateVisual();
			}
		}
		protected override void OnRender(DrawingContext drawingContext)
		{
			base.OnRender(drawingContext);

			var area = new Rect(0, 0, ActualWidth, ActualHeight);
			
			drawingContext.DrawRectangle(Background, null, area);
			
			if (mBitmap != null)
			{
				drawingContext.DrawImage(mBitmap, area);
			}

			var dg = mRenderLoop.CurrentReactor?.VectorBuffer.ToDrawingGroup();
			if (dg != null)
			{
				drawingContext.DrawDrawing(dg);
			}

			double measuresOffset = 5;

			void PrintLn(string text)
			{
				if (mMeasuresTypeFace == null)
				{
					UpdateTypeFace();
				}
				
#pragma warning disable 618
				var formattedText = new FormattedText(text, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, mMeasuresTypeFace, MeasuresFontSize, MeasuresForeground);
				var point = new Point(area.Width - 5 - formattedText.Width, measuresOffset);
#pragma warning restore 618
				
				drawingContext.DrawRectangle(MeasuresBackground, null, new Rect(point, new Size(formattedText.Width, formattedText.Height)));
				drawingContext.DrawText(formattedText, point);
				measuresOffset += formattedText.Height + 2;
			}
			
			if (ShowFramesPerSecond)
			{
				PrintLn($"{mRenderLoop?.FramesPerSecond:###,###,###,##0.00} fps");
			}
				
			if (ShowClock)
			{
				PrintLn($"{mRenderLoop?.Clock:###,###,###,##0.00}s");
			}
		}

		private void UpdateTypeFace()
		{
			mMeasuresTypeFace = new Typeface(MeasuresFontFamily ?? FontFamily, FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
		}
		private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			InvalidateBackBuffer();
		}
		private static void OnMeasuresFontFamilyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			d.CastTo<WpfPresenter>()?.UpdateTypeFace();
		}
	}

}
