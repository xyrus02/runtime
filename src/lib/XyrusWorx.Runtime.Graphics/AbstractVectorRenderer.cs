using System;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using JetBrains.Annotations;

namespace XyrusWorx.Runtime 
{
	[PublicAPI]
	public abstract class AbstractVectorRenderer
	{
		private DrawingGroup mDrawingGroup;
		private CancellationTokenSource mCancellationTokenSource;
		private Size mOutputSize;
		private Vector mOutputDpi;
		private IFontInfo mFontInfo;

		protected AbstractVectorRenderer()
		{
			mOutputSize = new Size(300, 300);
			mOutputDpi = new Vector(96, 96);
			mFontInfo = new DefaultFontInfo();
		}

		[NotNull]
		public IFontInfo FontInfo
		{
			get => mFontInfo;
			set => mFontInfo = value ?? throw new ArgumentNullException(nameof(value));
		}

		public void Draw([NotNull] DrawingContext drawingContext)
		{
			if (drawingContext == null)
			{
				throw new ArgumentNullException(nameof(drawingContext));
			}

			if (mDrawingGroup != null)
			{
				drawingContext.DrawDrawing(mDrawingGroup);
			}
		}
		
		public void UpdateOutputSize(Size newSize, Vector newDpi)
		{
			if (newSize.IsEmpty 
				|| double.IsNaN(newSize.Width) 
				|| double.IsInfinity(newSize.Width) 
				|| double.IsNaN(newSize.Height) 
				|| double.IsInfinity(newSize.Height) 
				|| newSize.Width <= 0 
				|| newSize.Height <= 0)
			{
				return;
			}
			
			mCancellationTokenSource?.Cancel();
			mOutputSize = newSize;
			mOutputDpi = newDpi;
		}
		public void Render()
		{
			DrawingContext subContext = null;

			mCancellationTokenSource?.Cancel();
			mDrawingGroup = new DrawingGroup();

			void OnEnter()
			{
				subContext = mDrawingGroup.Open();
				mCancellationTokenSource = new CancellationTokenSource();
			}
			void OnLeave()
			{
				mCancellationTokenSource = null;
				subContext?.Close();
			}

			using (new Scope(OnEnter, OnLeave).Enter())
			{
				UpdateBackBuffer(subContext, mCancellationTokenSource.Token);
			}
		}

		protected Size OutputSize => mOutputSize;
		protected Vector OutputDpi => mOutputDpi;
		
		protected abstract void UpdateBackBuffer(DrawingContext drawingContext, CancellationToken cancellationToken);

		sealed class DefaultFontInfo : IFontInfo
		{
			public FontFamily FontFamily { get; } = new FontFamily("Segoe UI");
			public FontWeight FontWeight => FontWeights.Normal;
			public FontStyle FontStyle => FontStyles.Normal;
			public FontStretch FontStretch => FontStretches.Normal;
			public double FontSize => 12;
		}
	}

}