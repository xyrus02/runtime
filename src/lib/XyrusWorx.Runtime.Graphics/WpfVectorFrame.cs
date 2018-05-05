using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Media;
using JetBrains.Annotations;

namespace XyrusWorx.Runtime 
{
	class WpfVectorFrame : IVectorFrame, IDisposable
	{
		private readonly DrawingContext mContext;
		private readonly Dictionary<Float4_1, Pen> mPenCache;
		private readonly Dictionary<Float4, Brush> mBrushCache;

		public WpfVectorFrame([NotNull] DrawingContext context)
		{
			mContext = context ?? throw new ArgumentNullException(nameof(context));
			mPenCache = new Dictionary<Float4_1, Pen>();
			mBrushCache = new Dictionary<Float4, Brush>();
		}

		public void DrawLine(Float4 color, float thickness, Float2 a, Float2 b) 
			=> mContext.DrawLine(Pen(color, thickness), Point(a), Point(b));
		
		public void DrawRectangle(Float4 fillColor, Float4 borderColor, float borderThickness, Float2 topLeft, Float2 bottomRight) 
			=> mContext.DrawRectangle(Brush(fillColor), Pen(borderColor, borderThickness), Rect(topLeft, bottomRight));

		public void DrawRoundedRectangle(Float4 fillColor, Float4 borderColor, float borderThickness, Float2 topLeft, Float2 bottomRight, float radiusX, float radiusY)
			=> mContext.DrawRoundedRectangle(Brush(fillColor), Pen(borderColor, borderThickness), Rect(topLeft, bottomRight), radiusX, radiusY);

		public void DrawEllipse(Float4 fillColor, Float4 borderColor, float borderThickness, Float2 center, float radiusX, float radiusY) =>
			mContext.DrawEllipse(Brush(fillColor), Pen(borderColor, borderThickness), Point(center), radiusX, radiusY);

		private Pen Pen(Float4 c, float t)
		{
			var f41 = new Float4_1{F4=c, F1=t};
			
			if (mPenCache.ContainsKey(f41))
			{
				return mPenCache[f41];
			}
			
			var pen = new Pen(Brush(f41.F4), f41.F1).GetAsFrozen().CastTo<Pen>();
			mPenCache.Add(f41, pen);
			return pen;
		}
		private Brush Brush(Float4 c)
		{
			if (mBrushCache.ContainsKey(c))
			{
				return mBrushCache[c];
			}
			
			var brush = new SolidColorBrush(Color.FromArgb(
				(byte)(int)Math.Max(0, Math.Min(c.w * 255, 255)),
				(byte)(int)Math.Max(0, Math.Min(c.x * 255, 255)),
				(byte)(int)Math.Max(0, Math.Min(c.y * 255, 255)),
				(byte)(int)Math.Max(0, Math.Min(c.z * 255, 255)))).GetAsFrozen().CastTo<Brush>();
			mBrushCache.Add(c, brush);
			return brush;
		}
		private Point Point(Float2 p) => new Point(p.x, p.y);
		private Rect Rect(Float2 tl, Float2 br) => new Rect(Point(tl), Point(br));
		
		[SuppressMessage("ReSharper", "InconsistentNaming")]
		[SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
		struct Float4_1 : IEquatable<Float4_1>
		{
			public bool Equals(Float4_1 other) => F4.Equals(other.F4) && F1.Equals(other.F1);

			public override bool Equals(object obj)
			{
				if (ReferenceEquals(null, obj))
				{
					return false;
				}

				return obj is Float4_1 && Equals((Float4_1)obj);
			}
			public override int GetHashCode()
			{
				unchecked
				{
					return (F4.GetHashCode() * 397) ^ F1.GetHashCode();
				}
			}

			public static bool operator ==(Float4_1 left, Float4_1 right) => left.Equals(right);
			public static bool operator !=(Float4_1 left, Float4_1 right) => !left.Equals(right);

			public Float4 F4 { get; set; }
			public float F1 { get; set; }
		}

		public void Dispose()
		{
			((IDisposable)mContext)?.Dispose();
		}
	}
}