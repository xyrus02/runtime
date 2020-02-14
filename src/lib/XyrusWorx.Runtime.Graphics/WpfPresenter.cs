using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using JetBrains.Annotations;
using XyrusWorx.Runtime.Imaging;

namespace XyrusWorx.Runtime
{
	[PublicAPI]
	public sealed class WpfPresenter : ContentControl, IWpfPresenter, IFontInfo
	{
		public static DependencyProperty MeasuresFontFamilyProperty = DependencyProperty.Register("MeasuresFontFamily", typeof(FontFamily), typeof(WpfPresenter), new FrameworkPropertyMetadata(new FontFamily("Courier"), FrameworkPropertyMetadataOptions.AffectsRender));
		public static DependencyProperty MeasuresFontSizeProperty = DependencyProperty.Register("MeasuresFontSize", typeof(double), typeof(WpfPresenter), new FrameworkPropertyMetadata(14.0, FrameworkPropertyMetadataOptions.AffectsRender));
		public static DependencyProperty MeasuresForegroundProperty = DependencyProperty.Register("MeasuresForeground", typeof(Brush), typeof(WpfPresenter), new FrameworkPropertyMetadata(Brushes.White, FrameworkPropertyMetadataOptions.AffectsRender));
		public static DependencyProperty MeasuresBackgroundProperty = DependencyProperty.Register("MeasuresBackground", typeof(Brush), typeof(WpfPresenter), new FrameworkPropertyMetadata(Brushes.Transparent, FrameworkPropertyMetadataOptions.AffectsRender));
		public static DependencyProperty ShowFramesPerSecondProperty = DependencyProperty.Register("ShowFramesPerSecond", typeof(bool), typeof(WpfPresenter), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender));
		public static DependencyProperty ShowClockProperty = DependencyProperty.Register("ShowClock", typeof(bool), typeof(WpfPresenter), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender));

		private WriteableBitmap mBitmap;

		public void SetSize(Int2 size)
		{
			Width = size.x;
			Height = size.y;
		}
		public void Run()
		{
			var renderLoop = DataContext.CastTo<IRenderLoop>();
			var reactor = renderLoop?.CurrentReactor;

			if (reactor == null)
			{
				mBitmap = null;
				return;
			}

			var width = (int)ActualWidth;
			var height = (int)ActualHeight;

			mBitmap = new WriteableBitmap(width, height, 96.0, 96.0, PixelFormats.Bgra32, null);
			reactor.SetBackBuffer(new MemoryWindow(mBitmap.BackBuffer, mBitmap.BackBufferStride * mBitmap.PixelHeight), TextureFormat.Bgra, mBitmap.BackBufferStride);

			var border = new Grid {Background = Background};
			var image = new Image();

			RenderOptions.SetBitmapScalingMode(image, BitmapScalingMode.NearestNeighbor);
			RenderOptions.SetEdgeMode(image, EdgeMode.Aliased);

			image.Source = mBitmap;
			image.Stretch = Stretch.None;
			image.HorizontalAlignment = HorizontalAlignment.Center;
			image.VerticalAlignment = VerticalAlignment.Center;

			border.Children.Add(image);
			Content = border;

			renderLoop.Reset();

			var dt = new DispatcherTimer(
				TimeSpan.FromMilliseconds(1000 / 60.0), 
				DispatcherPriority.Background,
				// ReSharper disable once AssignNullToNotNullAttribute
				dispatcher: Dispatcher, 
				callback:
				(o, e) =>
				{
					mBitmap.Lock();
					renderLoop.NextFrame();
					mBitmap.AddDirtyRect(new Int32Rect(0, 0, mBitmap.PixelWidth, mBitmap.PixelHeight));
					mBitmap.Unlock();
				});

			dt.Start();

			var infoBlock = new StackPanel();

			infoBlock.SetBinding(TextBlock.FontFamilyProperty, new Binding(nameof(MeasuresFontFamily)) {Source = this, Mode = BindingMode.OneWay});
			infoBlock.SetBinding(TextBlock.FontSizeProperty, new Binding(nameof(MeasuresFontSize)) {Source = this, Mode = BindingMode.OneWay});
			infoBlock.SetBinding(TextBlock.ForegroundProperty, new Binding(nameof(MeasuresForeground)) {Source = this, Mode = BindingMode.OneWay});
			infoBlock.SetBinding(TextBlock.BackgroundProperty, new Binding(nameof(MeasuresBackground)) {Source = this, Mode = BindingMode.OneWay});
			infoBlock.Margin = new Thickness(5,5,0,0);
			infoBlock.HorizontalAlignment = HorizontalAlignment.Left;
			infoBlock.VerticalAlignment = VerticalAlignment.Top;
			border.Children.Add(infoBlock);

			var fps = new TextBlock();

			fps.SetBinding(VisibilityProperty, new Binding(nameof(ShowFramesPerSecond)) {Source = this, Converter = new BooleanToVisibilityConverter(), Mode = BindingMode.OneWay});
			fps.SetBinding(TextBlock.TextProperty, new Binding(nameof(IRenderLoop.FramesPerSecond)) { StringFormat = "{0:###,###,###,##0.00} fps", Mode = BindingMode.OneWay});
			infoBlock.Children.Add(fps);

			var clock = new TextBlock();

			clock.SetBinding(VisibilityProperty, new Binding(nameof(ShowClock)) { Source = this, Converter = new BooleanToVisibilityConverter(), Mode = BindingMode.OneWay });
			clock.SetBinding(TextBlock.TextProperty, new Binding(nameof(IRenderLoop.Clock)) { StringFormat = "{0:###,###,###,##0.00} s", Mode = BindingMode.OneWay });
			infoBlock.Children.Add(clock);

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
	}

}
