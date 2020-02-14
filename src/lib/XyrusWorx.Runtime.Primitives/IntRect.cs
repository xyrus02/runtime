using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using JetBrains.Annotations;

namespace XyrusWorx.Runtime
{
	[PublicAPI]
	[Serializable]
	public struct IntRect : IFormattable
	{
		private int mX;
		private int mY;
		private int mWidth;
		private int mHeight;

		public IntRect(Int2 size)
		{
			mX = mY = 0;
			mWidth = size.x;
			mHeight = size.y;
		}
		public IntRect(Int2 point1, Int2 point2)
		{
			if (point1.x < point2.x)
			{
				mX = point1.x;
				mWidth = point2.x - point1.x;
			}
			else
			{
				mX = point2.x;
				mWidth = point1.x - point2.x;
			}

			if (point1.y < point2.y)
			{
				mY = point1.y;
				mHeight = point2.y - point1.y;
			}
			else
			{
				mY = point2.y;
				mHeight = point1.y - point2.y;
			}
		}
		public IntRect(int x, int y, int width, int height)
		{
			if (width < 0 || height < 0)
				throw new ArgumentException("width and height must be non-negative.");
			mX = x;
			mY = y;
			mWidth = width;
			mHeight = height;
		}

		[SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = mX.GetHashCode();
				hashCode = (hashCode * 397) ^ mY.GetHashCode();
				hashCode = (hashCode * 397) ^ mWidth.GetHashCode();
				hashCode = (hashCode * 397) ^ mHeight.GetHashCode();
				return hashCode;
			}
		}

		public bool Equals(IntRect value)
		{
			return (mX == value.X &&
				mY == value.Y &&
				mWidth == value.Width &&
				mHeight == value.Height);
		}
		public override bool Equals(object o)
		{
			if (!(o is IntRect))
				return false;

			return Equals((IntRect)o);
		}
		public static bool Equals(IntRect rect1, IntRect rect2)
		{
			return rect1.Equals(rect2);
		}

		public static bool operator !=(IntRect rect1, IntRect rect2)
		{
			return !(rect1.Location == rect2.Location && rect1.Size == rect2.Size);
		}
		public static bool operator ==(IntRect rect1, IntRect rect2)
		{
			return rect1.Location == rect2.Location && rect1.Size == rect2.Size;
		}

		public bool Contains(IntRect rect)
		{
			if (rect.Left < Left ||
				rect.Right > Right)
				return false;

			if (rect.Top < Top ||
				rect.Bottom > Bottom)
				return false;

			return true;
		}
		public bool Contains(int x, int y)
		{
			if (x < Left || x > Right)
				return false;
			if (y < Top || y > Bottom)
				return false;

			return true;
		}
		public bool Contains(Int2 point)
		{
			return Contains(point.x, point.y);
		}

		public static IntRect Inflate(IntRect rect, int width, int height)
		{
			if (width < rect.Width * -2)
				return Empty;
			if (height < rect.Height * -2)
				return Empty;

			var result = rect;
			result.Inflate(width, height);
			return result;
		}
		public static IntRect Inflate(IntRect rect, Int2 size)
		{
			return Inflate(rect, size.x, size.y);
		}
		public void Inflate(int width, int height)
		{
			mX -= width;
			mY -= height;

			mWidth += 2 * width;
			mHeight += 2 * height;
		}
		public void Inflate(Int2 size)
		{
			Inflate(size.x, size.y);
		}

		public bool IntersectsWith(IntRect rect)
		{
			return !((Left >= rect.Right) || (Right <= rect.Left) ||
				(Top >= rect.Bottom) || (Bottom <= rect.Top));
		}
		public void Intersect(IntRect rect)
		{
			var x = Math.Max(mX, rect.mX);
			var y = Math.Max(mY, rect.mY);
			var w = Math.Min(Right, rect.Right) - x;
			var h = Math.Min(Bottom, rect.Bottom) - y;

			if (w < 0 || h < 0)
			{
				mX = mY = int.MaxValue;
				mWidth = mHeight = int.MinValue;
			}
			else
			{
				mX = x;
				mY = y;
				mWidth = w;
				mHeight = h;
			}
		}
		public static IntRect Intersect(IntRect rect1, IntRect rect2)
		{
			var result = rect1;
			result.Intersect(rect2);
			return result;
		}

		public void Offset(int offsetX, int offsetY)
		{
			mX += offsetX;
			mY += offsetY;
		}
		public static IntRect Offset(IntRect rect, int offsetX, int offsetY)
		{
			var result = rect;
			result.Offset(offsetX, offsetY);
			return result;
		}
		public void Offset(Int2 offsetVector)
		{
			mX += offsetVector.x;
			mY += offsetVector.y;
		}
		public static IntRect Offset(IntRect rect, Int2 offsetVector)
		{
			var result = rect;
			result.Offset(offsetVector);
			return result;
		}

		public static IntRect Union(IntRect rect1, IntRect rect2)
		{
			var result = rect1;
			result.Union(rect2);
			return result;
		}
		public static IntRect Union(IntRect rect, Int2 point)
		{
			var result = rect;
			result.Union(point);
			return result;
		}
		public void Union(IntRect rect)
		{
			var left = Math.Min(Left, rect.Left);
			var top = Math.Min(Top, rect.Top);
			var right = Math.Max(Right, rect.Right);
			var bottom = Math.Max(Bottom, rect.Bottom);

			mX = left;
			mY = top;
			mWidth = right - left;
			mHeight = bottom - top;
		}
		public void Union(Int2 point)
		{
			Union(new IntRect(point, point));
		}

		public override string ToString()
		{
			return ToString(null);
		}
		public string ToString(IFormatProvider provider)
		{
			return ToString(null, provider);
		}
		
		string IFormattable.ToString(string format, IFormatProvider provider)
		{
			return ToString(format, provider);
		}
		private string ToString(string format, IFormatProvider provider)
		{
			if (IsEmpty)
				return "Empty";

			if (provider == null)
				provider = CultureInfo.CurrentCulture;

			if (format == null)
				format = string.Empty;

			var rectFormat = string.Format("{{0:{0}}}{1}{{1:{0}}}{1}{{2:{0}}}{1}{{3:{0}}}", format, ", ");
			return string.Format(provider, rectFormat, mX, mY, mWidth, mHeight);
		}

		public static IntRect Empty
		{
			get
			{
				var r = new IntRect();
				r.mX = r.mY = 0;
				r.mWidth = r.mHeight = 0;
				return r;
			}
		}

		public bool IsEmpty => (mX == 0 && mY == 0 && mWidth == 0 && mHeight == 0);

		public Int2 Location
		{
			get => new Int2(mX, mY);
			set
			{
				if (IsEmpty)
				{
					throw new InvalidOperationException("Cannot modify this property on the Empty Rect.");
				}

				mX = value.x;
				mY = value.y;
			}
		}
		public Int2 Size
		{
			get => new Int2(mWidth, mHeight);
			set
			{
				if (IsEmpty)
				{
					throw new InvalidOperationException("Cannot modify this property on the Empty Rect.");
				}

				mWidth = value.x;
				mHeight = value.y;
			}
		}

		public int X
		{
			get => mX;
			set
			{
				if (IsEmpty)
				{
					throw new InvalidOperationException("Cannot modify this property on the Empty Rect.");
				}

				mX = value;
			}
		}
		public int Y
		{
			get => mY;
			set
			{
				if (IsEmpty)
				{
					throw new InvalidOperationException("Cannot modify this property on the Empty Rect.");
				}

				mY = value;
			}
		}

		public int Width
		{
			get => mWidth;
			set
			{
				if (IsEmpty)
				{
					throw new InvalidOperationException("Cannot modify this property on the Empty Rect.");
				}

				if (value < 0)
				{
					throw new ArgumentException("width must be non-negative.");
				}

				mWidth = value;
			}
		}
		public int Height
		{
			get => mHeight;
			set
			{
				if (IsEmpty)
				{
					throw new InvalidOperationException("Cannot modify this property on the Empty Rect.");
				}

				if (value < 0)
				{
					throw new ArgumentException("height must be non-negative.");
				}

				mHeight = value;
			}
		}

		public int Left => mX;
		public int Top => mY;
		public int Right => mX + mWidth;
		public int Bottom => mY + mHeight;

		public Int2 TopLeft => new Int2(Left, Top);
		public Int2 TopRight => new Int2(Right, Top);
		public Int2 BottomLeft => new Int2(Left, Bottom);
		public Int2 BottomRight => new Int2(Right, Bottom);
	}
}