using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace XyrusWorx.Runtime
{
	[PublicAPI]
	[StructLayout(LayoutKind.Sequential)]
	[SuppressMessage("ReSharper", "InconsistentNaming")]
	[DebuggerDisplay("{x}, {y}, {z}")]
	public struct Float3 : IVector, IVector<float>, IEquatable<Float3>, IComparable<Float3>, IComparable
	{
		public float x, y, z;

		object[] IVector.GetComponents() => new object[] { x, y, z };
		float[] IVector<float>.GetComponents() => new[] { x, y, z };
		Type IVector.ComponentType
		{
			get => typeof(float);
		}

		public Float3(float x = 0, float y = 0, float z = 0)
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}
		public Float3(Float2 xy, float z = 0) : this(xy.x, xy.y, z)
		{
		}
		public Float3(float x, Float2 yz) : this(x, yz.x, yz.y)
		{
		}
		public Float3(Float3 xyz) : this(xyz.x, xyz.y, xyz.z)
		{
		}

		public static Float3 operator +(float a, Float3 b) => new Float3(a + b.x, a + b.y, a + b.z);
		public static Float3 operator -(float a, Float3 b) => new Float3(a - b.x, a - b.y, a - b.z);
		public static Float3 operator *(float a, Float3 b) => new Float3(a * b.x, a * b.y, a * b.z);
		public static Float3 operator /(float a, Float3 b) => new Float3(a / b.x, a / b.y, a / b.z);
		public static Float3 operator +(Float3 a, float b) => new Float3(a.x + b, a.y + b, a.z + b);
		public static Float3 operator -(Float3 a, float b) => new Float3(a.x - b, a.y - b, a.z - b);
		public static Float3 operator *(Float3 a, float b) => new Float3(a.x * b, a.y * b, a.z * b);
		public static Float3 operator /(Float3 a, float b) => new Float3(a.x / b, a.y / b, a.z / b);
		public static Float3 operator +(Float3 a, Float3 b) => new Float3(a.x + b.x, a.y + b.y, a.z + b.z);
		public static Float3 operator -(Float3 a, Float3 b) => new Float3(a.x - b.x, a.y - b.y, a.z - b.z);
		public static Float3 operator *(Float3 a, Float3 b) => new Float3(a.x * b.x, a.y * b.y, a.z * b.z);
		public static Float3 operator /(Float3 a, Float3 b) => new Float3(a.x / b.x, a.y / b.y, a.z / b.z);
		public static bool operator ==(Float3 left, Float3 right) => left.Equals(right);
		public static bool operator !=(Float3 left, Float3 right) => !left.Equals(right);
		public static bool operator <(Float3 left, Float3 right) => left.CompareTo(right) < 0;
		public static bool operator >(Float3 left, Float3 right) => left.CompareTo(right) > 0;
		public static bool operator <=(Float3 left, Float3 right) => left.CompareTo(right) <= 0;
		public static bool operator >=(Float3 left, Float3 right) => left.CompareTo(right) >= 0;
		public static implicit operator Vector3<float>(Float3 source) => new Vector3<float>(source.x, source.y, source.z); 
		public static implicit operator Float3(Vector3<float> source) => new Float3(source.x, source.y, source.z); 
		public static explicit operator Int3(Float3 source) => source.Int(); 

		public float RadiusSquared() => x * x + y * y + z * z;
		public float Radius() => (float)Math.Sqrt(x * x + y * y + z * z);

		public float this[int i]
		{
			get
			{
				switch (i)
				{
					case 0: return x;
					case 1: return y;
					case 2: return z;
				}
				
				throw new IndexOutOfRangeException();
			}
		}
		public Int3 Int() => new Int3((int)x, (int)y, (int)z);
		
		public Float2 xy
		{
			get => new Float2(x, y);
		}
		public Float2 yx
		{
			get => new Float2(y, x);
		}
		public Float2 yz
		{
			get => new Float2(y, z);
		}
		public Float2 zy
		{
			get => new Float2(z, y);
		}
		public Float2 xz
		{
			get => new Float2(x, z);
		}
		public Float2 zx
		{
			get => new Float2(z, x);
		}
		public Float3 xzy
		{
			get => new Float3(x, z, y);
		}
		public Float3 yxz
		{
			get => new Float3(y, x, z);
		}
		public Float3 yzx
		{
			get => new Float3(y, z, x);
		}
		public Float3 zxy
		{
			get => new Float3(z, x, y);
		}
		public Float3 zyx
		{
			get => new Float3(z, y, x);
		}

		public static Float3 Zero
		{
			get => new Float3(0);
		}

		[SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = x.GetHashCode();
				hashCode = (hashCode * 397) ^ y.GetHashCode();
				hashCode = (hashCode * 397) ^ z.GetHashCode();
				return hashCode;
			}
		}
		public bool Equals(Float3 other) => x == other.x && y == other.y && z == other.z;
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
			{
				return false;
			}
			
			return obj is Float3 && Equals((Float3)obj);
		}
		public override string ToString() => $"{x}, {y}, {z}";
		public int CompareTo(Float3 other)
		{
			var xComparison = x.CompareTo(other.x);
			if (xComparison != 0)
			{
				return xComparison;
			}
			
			var yComparison = y.CompareTo(other.y);
			if (yComparison != 0)
			{
				return yComparison;
			}
			
			return z.CompareTo(other.z);
		}
		public int CompareTo(object obj)
		{
			if (ReferenceEquals(null, obj))
			{
				return 1;
			}
			
			if (!(obj is Float3))
			{
				throw new ArgumentException($"Object must be of type {nameof(Float3)}");
			}
			
			return CompareTo((Float3)obj);
		}
	}
}