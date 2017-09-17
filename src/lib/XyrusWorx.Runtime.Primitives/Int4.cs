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
	public struct Int4 : IVectorType, IVectorType<int>, IEquatable<Int4>, IComparable<Int4>, IComparable
	{
		public int x, y, z, w;

		object[] IVectorType.GetComponents() => new object[] { x, y, z, w };
		int[] IVectorType<int>.GetComponents() => new[] { x, y, z, w };
		Type IVectorType.ComponentType
		{
			get => typeof(int);
		}

		public Int4(int x = 0, int y = 0, int z = 0, int w = 0)
		{
			this.x = x;
			this.y = y;
			this.z = z;
			this.w = w;
		}
		public Int4(Int2 xy, int z = 0, int w = 0) : this(xy.x, xy.y, z, w)
		{
		}
		public Int4(int x, Int2 yz, int w = 0) : this(x, yz.x, yz.y, w)
		{
		}
		public Int4(int x, int y, Int2 zw) : this(x, y, zw.x, zw.y)
		{
		}
		public Int4(Int3 xyz, int w = 0) : this(xyz.x, xyz.y, xyz.z, w)
		{
		}
		public Int4(Int2 xy, Int2 zw) : this(xy.x, xy.y, zw.x, zw.y)
		{
		}
		public Int4(int x, Int3 yzw) : this(x, yzw.x, yzw.y, yzw.z)
		{
		}
		public Int4(Int4 xyzw) : this(xyzw.x, xyzw.y, xyzw.z, xyzw.w)
		{
		}

		public static Int4 operator +(int a, Int4 b) => new Int4(a + b.x, a + b.y, a + b.z, a + b.w);
		public static Int4 operator -(int a, Int4 b) => new Int4(a - b.x, a - b.y, a - b.z, a - b.w);
		public static Int4 operator *(int a, Int4 b) => new Int4(a * b.x, a * b.y, a * b.z, a * b.w);
		public static Int4 operator /(int a, Int4 b) => new Int4(a / b.x, a / b.y, a / b.z, a / b.w);
		public static Int4 operator +(Int4 a, int b) => new Int4(a.x + b, a.y + b, a.z + b, a.w + b);
		public static Int4 operator -(Int4 a, int b) => new Int4(a.x - b, a.y - b, a.z - b, a.w + b);
		public static Int4 operator *(Int4 a, int b) => new Int4(a.x * b, a.y * b, a.z * b, a.w + b);
		public static Int4 operator /(Int4 a, int b) => new Int4(a.x / b, a.y / b, a.z / b, a.w + b);
		public static Int4 operator +(Int4 a, Int4 b) => new Int4(a.x + b.x, a.y + b.y, a.z + b.z, a.w + b.w);
		public static Int4 operator -(Int4 a, Int4 b) => new Int4(a.x - b.x, a.y - b.y, a.z - b.z, a.w - b.w);
		public static Int4 operator *(Int4 a, Int4 b) => new Int4(a.x * b.x, a.y * b.y, a.z * b.z, a.w * b.w);
		public static Int4 operator /(Int4 a, Int4 b) => new Int4(a.x / b.x, a.y / b.y, a.z / b.z, a.w / b.w);
		public static bool operator ==(Int4 left, Int4 right) => left.Equals(right);
		public static bool operator !=(Int4 left, Int4 right) => !left.Equals(right);
		public static bool operator <(Int4 left, Int4 right) => left.CompareTo(right) < 0;
		public static bool operator >(Int4 left, Int4 right) => left.CompareTo(right) > 0;
		public static bool operator <=(Int4 left, Int4 right) => left.CompareTo(right) <= 0;
		public static bool operator >=(Int4 left, Int4 right) => left.CompareTo(right) >= 0;
		public static implicit operator Vector4<int>(Int4 source) => new Vector4<int>(source.x, source.y, source.z, source.w); 
		public static implicit operator Int4(Vector4<int> source) => new Int4(source.x, source.y, source.z, source.w); 
		public static implicit operator Float4(Int4 source) => source.Float(); 

		public Float4 Float() => new Float4(x, y, z, w);
		
		public Int2 xy
		{
			get => new Int2(x, y);
		}
		public Int2 xz
		{
			get => new Int2(x, z);
		}
		public Int2 xw
		{
			get => new Int2(x, w);
		}
		public Int2 yx
		{
			get => new Int2(y, x);
		}
		public Int2 yz
		{
			get => new Int2(y, z);
		}
		public Int2 yw
		{
			get => new Int2(y, w);
		}
		public Int2 zx
		{
			get => new Int2(z, x);
		}
		public Int2 zy
		{
			get => new Int2(z, y);
		}
		public Int2 zw
		{
			get => new Int2(z, w);
		}
		public Int2 wx
		{
			get => new Int2(w, x);
		}
		public Int2 wy
		{
			get => new Int2(w, y);
		}
		public Int2 wz
		{
			get => new Int2(w, z);
		}
		public Int3 xyz
		{
			get => new Int3(x, y, z);
		}
		public Int3 xyw
		{
			get => new Int3(x, y, w);
		}
		public Int3 xzy
		{
			get => new Int3(x, z, y);
		}
		public Int3 xzw
		{
			get => new Int3(x, z, w);
		}
		public Int3 xwy
		{
			get => new Int3(x, w, y);
		}
		public Int3 xwz
		{
			get => new Int3(x, w, z);
		}
		public Int3 yxz
		{
			get => new Int3(y, x, z);
		}
		public Int3 yxw
		{
			get => new Int3(y, x, w);
		}
		public Int3 yzx
		{
			get => new Int3(y, z, x);
		}
		public Int3 yzw
		{
			get => new Int3(y, z, w);
		}
		public Int3 ywx
		{
			get => new Int3(y, w, x);
		}
		public Int3 ywz
		{
			get => new Int3(y, w, z);
		}
		public Int3 zxy
		{
			get => new Int3(z, x, y);
		}
		public Int3 zxw
		{
			get => new Int3(z, x, w);
		}
		public Int3 zyx
		{
			get => new Int3(z, y, x);
		}
		public Int3 zyw
		{
			get => new Int3(z, y, w);
		}
		public Int3 zwx
		{
			get => new Int3(z, w, x);
		}
		public Int3 zwy
		{
			get => new Int3(z, w, y);
		}
		public Int3 wxy
		{
			get => new Int3(w, x, y);
		}
		public Int3 wxz
		{
			get => new Int3(w, x, z);
		}
		public Int3 wyx
		{
			get => new Int3(w, y, x);
		}
		public Int3 wyz
		{
			get => new Int3(w, y, z);
		}
		public Int3 wzx
		{
			get => new Int3(w, z, x);
		}
		public Int3 wzy
		{
			get => new Int3(w, z, y);
		}
		public Int4 xywz 
		{
			get => new Int4(x, y, w, z);
		}
		public Int4 xzyw 
		{
			get => new Int4(x, z, y, w);
		}
		public Int4 xzwy 
		{
			get => new Int4(x, z, w, y);
		}
		public Int4 xwyz 
		{
			get => new Int4(x, w, y, z);
		}
		public Int4 xwzy 
		{
			get => new Int4(x, w, z, y);
		}
		public Int4 yxzw 
		{
			get => new Int4(y, x, z, w);
		}
		public Int4 yxwz 
		{
			get => new Int4(y, x, w, z);
		}
		public Int4 yzxw 
		{
			get => new Int4(y, z, x, w);
		}
		public Int4 yzwx 
		{
			get => new Int4(y, z, w, x);
		}
		public Int4 ywxz 
		{
			get => new Int4(y, w, x, z);
		}
		public Int4 ywzx 
		{
			get => new Int4(y, w, z, x);
		}
		public Int4 zxyw 
		{
			get => new Int4(z, x, y, w);
		}
		public Int4 zxwy 
		{
			get => new Int4(z, x, w, y);
		}
		public Int4 zyxw 
		{
			get => new Int4(z, y, x, w);
		}
		public Int4 zywx 
		{
			get => new Int4(z, y, w, x);
		}
		public Int4 zwxy 
		{
			get => new Int4(z, w, x, y);
		}
		public Int4 zwyx 
		{
			get => new Int4(z, w, y, x);
		}
		public Int4 wxyz 
		{
			get => new Int4(w, x, y, z);
		}
		public Int4 wxzy 
		{
			get => new Int4(w, x, z, y);
		}
		public Int4 wyxz 
		{
			get => new Int4(w, y, x, z);
		}
		public Int4 wyzx 
		{
			get => new Int4(w, y, z, x);
		}
		public Int4 wzxy 
		{
			get => new Int4(w, z, x, y);
		}
		public Int4 wzyx 
		{
			get => new Int4(w, z, y, x);
		}

		public static Int4 Zero
		{
			get => new Int4(0);
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
		public bool Equals(Int4 other) => x == other.x && y == other.y && z == other.z && w == other.w;
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
			{
				return false;
			}
			
			return obj is Int4 && Equals((Int4)obj);
		}
		public override string ToString() => $"{x}, {y}, {z}, {w}";
		public int CompareTo(Int4 other)
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
			
			if (!(obj is Int4))
			{
				throw new ArgumentException($"Object must be of type {nameof(Int4)}");
			}
			
			return CompareTo((Int4)obj);
		}
	}
}