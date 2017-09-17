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
	public struct Byte4 : IEquatable<Byte4>, IComparable<Byte4>, IComparable
	{
		public byte x, y, z, w;

		public Byte4(byte x = 0, byte y = 0, byte z = 0, byte w = 0)
		{
			this.x = x;
			this.y = y;
			this.z = z;
			this.w = w;
		}
		public Byte4(Byte2 xy, byte z = 0, byte w = 0) : this(xy.x, xy.y, z, w)
		{
		}
		public Byte4(byte x, Byte2 yz, byte w = 0) : this(x, yz.x, yz.y, w)
		{
		}
		public Byte4(byte x, byte y, Byte2 zw) : this(x, y, zw.x, zw.y)
		{
		}
		public Byte4(Byte3 xyz, byte w = 0) : this(xyz.x, xyz.y, xyz.z, w)
		{
		}
		public Byte4(Byte2 xy, Byte2 zw) : this(xy.x, xy.y, zw.x, zw.y)
		{
		}
		public Byte4(byte x, Byte3 yzw) : this(x, yzw.x, yzw.y, yzw.z)
		{
		}
		public Byte4(Byte4 xyzw) : this(xyzw.x, xyzw.y, xyzw.z, xyzw.w)
		{
		}

		public static Byte4 operator +(byte a, Byte4 b) => new Byte4((byte)(a + b.x), (byte)(a + b.y), (byte)(a + b.z), (byte)(a + b.w));
		public static Byte4 operator -(byte a, Byte4 b) => new Byte4((byte)(a - b.x), (byte)(a - b.y), (byte)(a - b.z), (byte)(a - b.w));
		public static Byte4 operator *(byte a, Byte4 b) => new Byte4((byte)(a * b.x), (byte)(a * b.y), (byte)(a * b.z), (byte)(a * b.w));
		public static Byte4 operator /(byte a, Byte4 b) => new Byte4((byte)(a / b.x), (byte)(a / b.y), (byte)(a / b.z), (byte)(a / b.w));
		public static Byte4 operator +(Byte4 a, byte b) => new Byte4((byte)(a.x + b), (byte)(a.y + b), (byte)(a.z + b), (byte)(a.w + b));
		public static Byte4 operator -(Byte4 a, byte b) => new Byte4((byte)(a.x - b), (byte)(a.y - b), (byte)(a.z - b), (byte)(a.w + b));
		public static Byte4 operator *(Byte4 a, byte b) => new Byte4((byte)(a.x * b), (byte)(a.y * b), (byte)(a.z * b), (byte)(a.w + b));
		public static Byte4 operator /(Byte4 a, byte b) => new Byte4((byte)(a.x / b), (byte)(a.y / b), (byte)(a.z / b), (byte)(a.w + b));
		public static Byte4 operator +(Byte4 a, Byte4 b) => new Byte4((byte)(a.x + b.x), (byte)(a.y + b.y), (byte)(a.z + b.z), (byte)(a.w + b.w));
		public static Byte4 operator -(Byte4 a, Byte4 b) => new Byte4((byte)(a.x - b.x), (byte)(a.y - b.y), (byte)(a.z - b.z), (byte)(a.w - b.w));
		public static Byte4 operator *(Byte4 a, Byte4 b) => new Byte4((byte)(a.x * b.x), (byte)(a.y * b.y), (byte)(a.z * b.z), (byte)(a.w * b.w));
		public static Byte4 operator /(Byte4 a, Byte4 b) => new Byte4((byte)(a.x / b.x), (byte)(a.y / b.y), (byte)(a.z / b.z), (byte)(a.w / b.w));
		public static bool operator ==(Byte4 left, Byte4 right) => left.Equals(right);
		public static bool operator !=(Byte4 left, Byte4 right) => !left.Equals(right);
		public static bool operator <(Byte4 left, Byte4 right) => left.CompareTo(right) < 0;
		public static bool operator >(Byte4 left, Byte4 right) => left.CompareTo(right) > 0;
		public static bool operator <=(Byte4 left, Byte4 right) => left.CompareTo(right) <= 0;
		public static bool operator >=(Byte4 left, Byte4 right) => left.CompareTo(right) >= 0;

		public Byte2 xy
		{
			get => new Byte2(x, y);
		}
		public Byte2 xz
		{
			get => new Byte2(x, z);
		}
		public Byte2 xw
		{
			get => new Byte2(x, w);
		}
		public Byte2 yx
		{
			get => new Byte2(y, x);
		}
		public Byte2 yz
		{
			get => new Byte2(y, z);
		}
		public Byte2 yw
		{
			get => new Byte2(y, w);
		}
		public Byte2 zx
		{
			get => new Byte2(z, x);
		}
		public Byte2 zy
		{
			get => new Byte2(z, y);
		}
		public Byte2 zw
		{
			get => new Byte2(z, w);
		}
		public Byte2 wx
		{
			get => new Byte2(w, x);
		}
		public Byte2 wy
		{
			get => new Byte2(w, y);
		}
		public Byte2 wz
		{
			get => new Byte2(w, z);
		}
		public Byte3 xyz
		{
			get => new Byte3(x, y, z);
		}
		public Byte3 xyw
		{
			get => new Byte3(x, y, w);
		}
		public Byte3 xzy
		{
			get => new Byte3(x, z, y);
		}
		public Byte3 xzw
		{
			get => new Byte3(x, z, w);
		}
		public Byte3 xwy
		{
			get => new Byte3(x, w, y);
		}
		public Byte3 xwz
		{
			get => new Byte3(x, w, z);
		}
		public Byte3 yxz
		{
			get => new Byte3(y, x, z);
		}
		public Byte3 yxw
		{
			get => new Byte3(y, x, w);
		}
		public Byte3 yzx
		{
			get => new Byte3(y, z, x);
		}
		public Byte3 yzw
		{
			get => new Byte3(y, z, w);
		}
		public Byte3 ywx
		{
			get => new Byte3(y, w, x);
		}
		public Byte3 ywz
		{
			get => new Byte3(y, w, z);
		}
		public Byte3 zxy
		{
			get => new Byte3(z, x, y);
		}
		public Byte3 zxw
		{
			get => new Byte3(z, x, w);
		}
		public Byte3 zyx
		{
			get => new Byte3(z, y, x);
		}
		public Byte3 zyw
		{
			get => new Byte3(z, y, w);
		}
		public Byte3 zwx
		{
			get => new Byte3(z, w, x);
		}
		public Byte3 zwy
		{
			get => new Byte3(z, w, y);
		}
		public Byte3 wxy
		{
			get => new Byte3(w, x, y);
		}
		public Byte3 wxz
		{
			get => new Byte3(w, x, z);
		}
		public Byte3 wyx
		{
			get => new Byte3(w, y, x);
		}
		public Byte3 wyz
		{
			get => new Byte3(w, y, z);
		}
		public Byte3 wzx
		{
			get => new Byte3(w, z, x);
		}
		public Byte3 wzy
		{
			get => new Byte3(w, z, y);
		}
		public Byte4 xywz 
		{
			get => new Byte4(x, y, w, z);
		}
		public Byte4 xzyw 
		{
			get => new Byte4(x, z, y, w);
		}
		public Byte4 xzwy 
		{
			get => new Byte4(x, z, w, y);
		}
		public Byte4 xwyz 
		{
			get => new Byte4(x, w, y, z);
		}
		public Byte4 xwzy 
		{
			get => new Byte4(x, w, z, y);
		}
		public Byte4 yxzw 
		{
			get => new Byte4(y, x, z, w);
		}
		public Byte4 yxwz 
		{
			get => new Byte4(y, x, w, z);
		}
		public Byte4 yzxw 
		{
			get => new Byte4(y, z, x, w);
		}
		public Byte4 yzwx 
		{
			get => new Byte4(y, z, w, x);
		}
		public Byte4 ywxz 
		{
			get => new Byte4(y, w, x, z);
		}
		public Byte4 ywzx 
		{
			get => new Byte4(y, w, z, x);
		}
		public Byte4 zxyw 
		{
			get => new Byte4(z, x, y, w);
		}
		public Byte4 zxwy 
		{
			get => new Byte4(z, x, w, y);
		}
		public Byte4 zyxw 
		{
			get => new Byte4(z, y, x, w);
		}
		public Byte4 zywx 
		{
			get => new Byte4(z, y, w, x);
		}
		public Byte4 zwxy 
		{
			get => new Byte4(z, w, x, y);
		}
		public Byte4 zwyx 
		{
			get => new Byte4(z, w, y, x);
		}
		public Byte4 wxyz 
		{
			get => new Byte4(w, x, y, z);
		}
		public Byte4 wxzy 
		{
			get => new Byte4(w, x, z, y);
		}
		public Byte4 wyxz 
		{
			get => new Byte4(w, y, x, z);
		}
		public Byte4 wyzx 
		{
			get => new Byte4(w, y, z, x);
		}
		public Byte4 wzxy 
		{
			get => new Byte4(w, z, x, y);
		}
		public Byte4 wzyx 
		{
			get => new Byte4(w, z, y, x);
		}

		public static Byte4 Zero
		{
			get => new Byte4(0);
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
		public bool Equals(Byte4 other) => x == other.x && y == other.y && z == other.z && w == other.w;
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
			{
				return false;
			}
			
			return obj is Byte4 && Equals((Byte4)obj);
		}
		public override string ToString() => $"{x}, {y}, {z}, {w}";
		public int CompareTo(Byte4 other)
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
			
			if (!(obj is Byte4))
			{
				throw new ArgumentException($"Object must be of type {nameof(Byte4)}");
			}
			
			return CompareTo((Byte4)obj);
		}
	}
}