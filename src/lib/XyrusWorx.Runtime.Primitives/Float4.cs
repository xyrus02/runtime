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
	[DebuggerDisplay("{x}, {y}, {z}, {w}")]
	public struct Float4 : IVectorType, IVectorType<float>, IEquatable<Float4>, IComparable<Float4>, IComparable
	{
		public float x, y, z, w;

		object[] IVectorType.GetComponents() => new object[] { x, y, z, w };
		float[] IVectorType<float>.GetComponents() => new[] { x, y, z, w };
		Type IVectorType.ComponentType
		{
			get => typeof(float);
		}

		public Float4(float x = 0, float y = 0, float z = 0, float w = 0)
		{
			this.x = x;
			this.y = y;
			this.z = z;
			this.w = w;
		}
		public Float4(Float2 xy, float z = 0, float w = 0) : this(xy.x, xy.y, z, w)
		{
		}
		public Float4(float x, Float2 yz, float w = 0) : this(x, yz.x, yz.y, w)
		{
		}
		public Float4(float x, float y, Float2 zw) : this(x, y, zw.x, zw.y)
		{
		}
		public Float4(Float3 xyz, float w = 0) : this(xyz.x, xyz.y, xyz.z, w)
		{
		}
		public Float4(Float2 xy, Float2 zw) : this(xy.x, xy.y, zw.x, zw.y)
		{
		}
		public Float4(float x, Float3 yzw) : this(x, yzw.x, yzw.y, yzw.z)
		{
		}
		public Float4(Byte4 xyzw) : this(xyzw.x, xyzw.y, xyzw.z, xyzw.w)
		{
		}
		public Float4(Int4 xyzw) : this(xyzw.x, xyzw.y, xyzw.z, xyzw.w)
		{
		}
		public Float4(Float4 xyzw) : this(xyzw.x, xyzw.y, xyzw.z, xyzw.w)
		{
		}

		public static Float4 operator +(float a, Float4 b) => new Float4(a + b.x, a + b.y, a + b.z, a + b.w);
		public static Float4 operator -(float a, Float4 b) => new Float4(a - b.x, a - b.y, a - b.z, a - b.w);
		public static Float4 operator *(float a, Float4 b) => new Float4(a * b.x, a * b.y, a * b.z, a * b.w);
		public static Float4 operator /(float a, Float4 b) => new Float4(a / b.x, a / b.y, a / b.z, a / b.w);
		public static Float4 operator +(Float4 a, float b) => new Float4(a.x + b, a.y + b, a.z + b, a.w + b);
		public static Float4 operator -(Float4 a, float b) => new Float4(a.x - b, a.y - b, a.z - b, a.w + b);
		public static Float4 operator *(Float4 a, float b) => new Float4(a.x * b, a.y * b, a.z * b, a.w + b);
		public static Float4 operator /(Float4 a, float b) => new Float4(a.x / b, a.y / b, a.z / b, a.w + b);
		public static Float4 operator +(Float4 a, Float4 b) => new Float4(a.x + b.x, a.y + b.y, a.z + b.z, a.w + b.w);
		public static Float4 operator -(Float4 a, Float4 b) => new Float4(a.x - b.x, a.y - b.y, a.z - b.z, a.w - b.w);
		public static Float4 operator *(Float4 a, Float4 b) => new Float4(a.x * b.x, a.y * b.y, a.z * b.z, a.w * b.w);
		public static Float4 operator /(Float4 a, Float4 b) => new Float4(a.x / b.x, a.y / b.y, a.z / b.z, a.w / b.w);
		public static bool operator ==(Float4 left, Float4 right) => left.Equals(right);
		public static bool operator !=(Float4 left, Float4 right) => !left.Equals(right);
		public static bool operator <(Float4 left, Float4 right) => left.CompareTo(right) < 0;
		public static bool operator >(Float4 left, Float4 right) => left.CompareTo(right) > 0;
		public static bool operator <=(Float4 left, Float4 right) => left.CompareTo(right) <= 0;
		public static bool operator >=(Float4 left, Float4 right) => left.CompareTo(right) >= 0;
		public static implicit operator Vector4<float>(Float4 source) => new Vector4<float>(source.x, source.y); 
		public static implicit operator Float4(Vector2<float> source) => new Float4(source.x, source.y); 
		
		public float RadiusSquared() => x * x + y * y + z * z + w * w;
		public float Radius() => (float)Math.Sqrt(x * x + y * y + z * z + w * w);
		
		public Byte4 Byte() => new Byte4((byte)x, (byte)y, (byte)z, (byte)w);
		public Int4 Int() => new Int4((int)x, (int)y, (int)z, (int)w);

		public Float2 xy
		{
			get => new Float2(x, y);
		}
		public Float2 xz
		{
			get => new Float2(x, z);
		}
		public Float2 xw
		{
			get => new Float2(x, w);
		}
		public Float2 yx
		{
			get => new Float2(y, x);
		}
		public Float2 yz
		{
			get => new Float2(y, z);
		}
		public Float2 yw
		{
			get => new Float2(y, w);
		}
		public Float2 zx
		{
			get => new Float2(z, x);
		}
		public Float2 zy
		{
			get => new Float2(z, y);
		}
		public Float2 zw
		{
			get => new Float2(z, w);
		}
		public Float2 wx
		{
			get => new Float2(w, x);
		}
		public Float2 wy
		{
			get => new Float2(w, y);
		}
		public Float2 wz
		{
			get => new Float2(w, z);
		}
		public Float3 xyz
		{
			get => new Float3(x, y, z);
		}
		public Float3 xyw
		{
			get => new Float3(x, y, w);
		}
		public Float3 xzy
		{
			get => new Float3(x, z, y);
		}
		public Float3 xzw
		{
			get => new Float3(x, z, w);
		}
		public Float3 xwy
		{
			get => new Float3(x, w, y);
		}
		public Float3 xwz
		{
			get => new Float3(x, w, z);
		}
		public Float3 yxz
		{
			get => new Float3(y, x, z);
		}
		public Float3 yxw
		{
			get => new Float3(y, x, w);
		}
		public Float3 yzx
		{
			get => new Float3(y, z, x);
		}
		public Float3 yzw
		{
			get => new Float3(y, z, w);
		}
		public Float3 ywx
		{
			get => new Float3(y, w, x);
		}
		public Float3 ywz
		{
			get => new Float3(y, w, z);
		}
		public Float3 zxy
		{
			get => new Float3(z, x, y);
		}
		public Float3 zxw
		{
			get => new Float3(z, x, w);
		}
		public Float3 zyx
		{
			get => new Float3(z, y, x);
		}
		public Float3 zyw
		{
			get => new Float3(z, y, w);
		}
		public Float3 zwx
		{
			get => new Float3(z, w, x);
		}
		public Float3 zwy
		{
			get => new Float3(z, w, y);
		}
		public Float3 wxy
		{
			get => new Float3(w, x, y);
		}
		public Float3 wxz
		{
			get => new Float3(w, x, z);
		}
		public Float3 wyx
		{
			get => new Float3(w, y, x);
		}
		public Float3 wyz
		{
			get => new Float3(w, y, z);
		}
		public Float3 wzx
		{
			get => new Float3(w, z, x);
		}
		public Float3 wzy
		{
			get => new Float3(w, z, y);
		}
		public Float4 xywz 
		{
			get => new Float4(x, y, w, z);
		}
		public Float4 xzyw 
		{
			get => new Float4(x, z, y, w);
		}
		public Float4 xzwy 
		{
			get => new Float4(x, z, w, y);
		}
		public Float4 xwyz 
		{
			get => new Float4(x, w, y, z);
		}
		public Float4 xwzy 
		{
			get => new Float4(x, w, z, y);
		}
		public Float4 yxzw 
		{
			get => new Float4(y, x, z, w);
		}
		public Float4 yxwz 
		{
			get => new Float4(y, x, w, z);
		}
		public Float4 yzxw 
		{
			get => new Float4(y, z, x, w);
		}
		public Float4 yzwx 
		{
			get => new Float4(y, z, w, x);
		}
		public Float4 ywxz 
		{
			get => new Float4(y, w, x, z);
		}
		public Float4 ywzx 
		{
			get => new Float4(y, w, z, x);
		}
		public Float4 zxyw 
		{
			get => new Float4(z, x, y, w);
		}
		public Float4 zxwy 
		{
			get => new Float4(z, x, w, y);
		}
		public Float4 zyxw 
		{
			get => new Float4(z, y, x, w);
		}
		public Float4 zywx 
		{
			get => new Float4(z, y, w, x);
		}
		public Float4 zwxy 
		{
			get => new Float4(z, w, x, y);
		}
		public Float4 zwyx 
		{
			get => new Float4(z, w, y, x);
		}
		public Float4 wxyz 
		{
			get => new Float4(w, x, y, z);
		}
		public Float4 wxzy 
		{
			get => new Float4(w, x, z, y);
		}
		public Float4 wyxz 
		{
			get => new Float4(w, y, x, z);
		}
		public Float4 wyzx 
		{
			get => new Float4(w, y, z, x);
		}
		public Float4 wzxy 
		{
			get => new Float4(w, z, x, y);
		}
		public Float4 wzyx 
		{
			get => new Float4(w, z, y, x);
		}

		public static Float4 Zero
		{
			get => new Float4(0);
		}

		[SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = x.GetHashCode();
				hashCode = (hashCode * 397) ^ y.GetHashCode();
				hashCode = (hashCode * 397) ^ z.GetHashCode();
				hashCode = (hashCode * 397) ^ w.GetHashCode();
				return hashCode;
			}
		}
		public bool Equals(Float4 other) => x == other.x && y == other.y && z == other.z && w == other.w;
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
			{
				return false;
			}
			
			return obj is Float4 && Equals((Float4)obj);
		}
		public override string ToString() => $"{x}, {y}, {z}, {w}";
		public int CompareTo(Float4 other)
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
			
			var zComparison = z.CompareTo(other.z);
			if (zComparison != 0)
			{
				return zComparison;
			}
			
			return w.CompareTo(other.w);
		}
		public int CompareTo(object obj)
		{
			if (ReferenceEquals(null, obj))
			{
				return 1;
			}
			
			if (!(obj is Float4))
			{
				throw new ArgumentException($"Object must be of type {nameof(Float4)}");
			}
			
			return CompareTo((Float4)obj);
		}
	}
}