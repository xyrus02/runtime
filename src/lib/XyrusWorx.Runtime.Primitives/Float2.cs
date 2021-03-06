using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace XyrusWorx.Runtime
{
	[PublicAPI][StructLayout(LayoutKind.Sequential)]
	[SuppressMessage("ReSharper", "InconsistentNaming")]
	[DebuggerDisplay("{x}, {y}")]
	public struct Float2 : IVector, IVector<float>, IEquatable<Float2>, IComparable<Float2>, IComparable, IVectorRowWriter
	{
		public float x, y;

		object[] IVector.GetComponents() => new object[] {x, y};
		float[] IVector<float>.GetComponents() => new [] {x, y};
		Type IVector.ComponentType
		{
			get => typeof(float);
		}

		public Float2(float x = 0, float y = 0)
		{
			this.x = x;
			this.y = y;
		}
		public Float2(Float2 xy) : this(xy.x, xy.y)
		{
		}

		public static Float2 operator +(float a, Float2 b) => new Float2(a + b.x, a + b.y);
		public static Float2 operator -(float a, Float2 b) => new Float2(a - b.x, a - b.y);
		public static Float2 operator *(float a, Float2 b) => new Float2(a * b.x, a * b.y);
		public static Float2 operator /(float a, Float2 b) => new Float2(a / b.x, a / b.y);
		public static Float2 operator +(Float2 a, float b) => new Float2(a.x + b, a.y + b);
		public static Float2 operator -(Float2 a, float b) => new Float2(a.x - b, a.y - b);
		public static Float2 operator *(Float2 a, float b) => new Float2(a.x * b, a.y * b);
		public static Float2 operator /(Float2 a, float b) => new Float2(a.x / b, a.y / b);
		public static Float2 operator +(Float2 a, Float2 b) => new Float2(a.x + b.x, a.y + b.y);
		public static Float2 operator -(Float2 a, Float2 b) => new Float2(a.x - b.x, a.y - b.y);
		public static Float2 operator *(Float2 a, Float2 b) => new Float2(a.x * b.x, a.y * b.y);
		public static Float2 operator /(Float2 a, Float2 b) => new Float2(a.x / b.x, a.y / b.y);
		public static bool operator ==(Float2 left, Float2 right) => left.Equals(right);
		public static bool operator !=(Float2 left, Float2 right) => !left.Equals(right);
		public static bool operator <(Float2 left, Float2 right) => left.CompareTo(right) < 0;
		public static bool operator >(Float2 left, Float2 right) => left.CompareTo(right) > 0;
		public static bool operator <=(Float2 left, Float2 right) => left.CompareTo(right) <= 0;
		public static bool operator >=(Float2 left, Float2 right) => left.CompareTo(right) >= 0;
		public static implicit operator Vector2<float>(Float2 source) => new Vector2<float>(source.x, source.y); 
		public static implicit operator Float2(Vector2<float> source) => new Float2(source.x, source.y);  
		public static explicit operator Int2(Float2 source) => source.Int(); 

		public float RadiusSquared() => x * x + y * y;
		public float Radius() => (float)Math.Sqrt(x * x + y * y);

		public float this[int i]
		{
			get
			{
				switch (i)
				{
					case 0: return x;
					case 1: return y;
				}
				
				throw new IndexOutOfRangeException();
			}
		}
		public Int2 Int() => new Int2((int)x, (int)y);

		public Float2 yx
		{
			get => new Float2(y, x);
		}
		
		public static Float2 Zero
		{
			get => new Float2(0);
		}

		[SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
		public override int GetHashCode()
		{
			unchecked
			{
				return (x.GetHashCode() * 397) ^ y.GetHashCode();
			}
		}
		public bool Equals(Float2 other) => x == other.x && y == other.y;
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
			{
				return false;
			}
			return obj is Float2 && Equals((Float2)obj);
		}
		public override string ToString() => $"{x}, {y}";
		public int CompareTo(Float2 other)
		{
			var xComparison = x.CompareTo(other.x);
			return xComparison != 0 ? xComparison : y.CompareTo(other.y);
		}
		public int CompareTo(object obj)
		{
			if (ReferenceEquals(null, obj))
			{
				return 1;
			}
			
			if (!(obj is Float2))
			{
				throw new ArgumentException($"Object must be of type {nameof(Float2)}");
			}
			
			return CompareTo((Float2)obj);
		}
		
		IVector IVectorRowWriter.Set(int row, object value)
		{
			var t = (float)value;

			switch (row)
			{
				case 0: return new Float2(t, y);
				case 1: return new Float2(x, t);
			}

			return this;
		}
	}
}