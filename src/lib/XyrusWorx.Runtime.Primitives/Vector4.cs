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
	public struct Vector4<T> : IVector, IVector<T>, IEquatable<Vector4<T>>, IComparable<Vector4<T>>, IComparable
		where T: struct, IEquatable<T>, IComparable<T>, IComparable
	{
		public T x, y, z, w;
		
		object[] IVector.GetComponents() => new object[] {x, y, z, w};
		T[] IVector<T>.GetComponents() => new [] {x, y, z, w};
		Type IVector.ComponentType
		{
			get => typeof(T);
		}

		public Vector4(T x = default(T), T y = default(T), T z = default(T), T w = default(T))
		{
			this.x = x;
			this.y = y;
			this.z = z;
			this.w = w;
		}
		public Vector4(Vector2<T> xy, T z = default(T), T w = default(T)) : this(xy.x, xy.y, z, w)
		{
		}
		public Vector4(T x, Vector2<T> yz, T w = default(T)) : this(x, yz.x, yz.y, w)
		{
		}
		public Vector4(T x, T y, Vector2<T> zw) : this(x, y, zw.x, zw.y)
		{
		}
		public Vector4(Vector3<T> xyz, T w = default(T)) : this(xyz.x, xyz.y, xyz.z, w)
		{
		}
		public Vector4(Vector2<T> xy, Vector2<T> zw) : this(xy.x, xy.y, zw.x, zw.y)
		{
		}
		public Vector4(T x, Vector3<T> yzw) : this(x, yzw.x, yzw.y, yzw.z)
		{
		}
		public Vector4(Vector4<T> xyzw) : this(xyzw.x, xyzw.y, xyzw.z, xyzw.w)
		{
		}

		public static bool operator ==(Vector4<T> left, Vector4<T> right) => left.Equals(right);
		public static bool operator !=(Vector4<T> left, Vector4<T> right) => !left.Equals(right);
		public static bool operator <(Vector4<T> left, Vector4<T> right) => left.CompareTo(right) < 0;
		public static bool operator >(Vector4<T> left, Vector4<T> right) => left.CompareTo(right) > 0;
		public static bool operator <=(Vector4<T> left, Vector4<T> right) => left.CompareTo(right) <= 0;
		public static bool operator >=(Vector4<T> left, Vector4<T> right) => left.CompareTo(right) >= 0;

		public T this[int i]
		{
			get
			{
				switch (i)
				{
					case 0: return x;
					case 1: return y;
					case 2: return z;
					case 3: return w;
				}
				
				throw new IndexOutOfRangeException();
			}
		}
		
		public Vector2<T> xy
		{
			get => new Vector2<T>(x, y);
		}
		public Vector2<T> xz
		{
			get => new Vector2<T>(x, z);
		}
		public Vector2<T> xw
		{
			get => new Vector2<T>(x, w);
		}
		public Vector2<T> yx
		{
			get => new Vector2<T>(y, x);
		}
		public Vector2<T> yz
		{
			get => new Vector2<T>(y, z);
		}
		public Vector2<T> yw
		{
			get => new Vector2<T>(y, w);
		}
		public Vector2<T> zx
		{
			get => new Vector2<T>(z, x);
		}
		public Vector2<T> zy
		{
			get => new Vector2<T>(z, y);
		}
		public Vector2<T> zw
		{
			get => new Vector2<T>(z, w);
		}
		public Vector2<T> wx
		{
			get => new Vector2<T>(w, x);
		}
		public Vector2<T> wy
		{
			get => new Vector2<T>(w, y);
		}
		public Vector2<T> wz
		{
			get => new Vector2<T>(w, z);
		}
		public Vector3<T> xyz
		{
			get => new Vector3<T>(x, y, z);
		}
		public Vector3<T> xyw
		{
			get => new Vector3<T>(x, y, w);
		}
		public Vector3<T> xzy
		{
			get => new Vector3<T>(x, z, y);
		}
		public Vector3<T> xzw
		{
			get => new Vector3<T>(x, z, w);
		}
		public Vector3<T> xwy
		{
			get => new Vector3<T>(x, w, y);
		}
		public Vector3<T> xwz
		{
			get => new Vector3<T>(x, w, z);
		}
		public Vector3<T> yxz
		{
			get => new Vector3<T>(y, x, z);
		}
		public Vector3<T> yxw
		{
			get => new Vector3<T>(y, x, w);
		}
		public Vector3<T> yzx
		{
			get => new Vector3<T>(y, z, x);
		}
		public Vector3<T> yzw
		{
			get => new Vector3<T>(y, z, w);
		}
		public Vector3<T> ywx
		{
			get => new Vector3<T>(y, w, x);
		}
		public Vector3<T> ywz
		{
			get => new Vector3<T>(y, w, z);
		}
		public Vector3<T> zxy
		{
			get => new Vector3<T>(z, x, y);
		}
		public Vector3<T> zxw
		{
			get => new Vector3<T>(z, x, w);
		}
		public Vector3<T> zyx
		{
			get => new Vector3<T>(z, y, x);
		}
		public Vector3<T> zyw
		{
			get => new Vector3<T>(z, y, w);
		}
		public Vector3<T> zwx
		{
			get => new Vector3<T>(z, w, x);
		}
		public Vector3<T> zwy
		{
			get => new Vector3<T>(z, w, y);
		}
		public Vector3<T> wxy
		{
			get => new Vector3<T>(w, x, y);
		}
		public Vector3<T> wxz
		{
			get => new Vector3<T>(w, x, z);
		}
		public Vector3<T> wyx
		{
			get => new Vector3<T>(w, y, x);
		}
		public Vector3<T> wyz
		{
			get => new Vector3<T>(w, y, z);
		}
		public Vector3<T> wzx
		{
			get => new Vector3<T>(w, z, x);
		}
		public Vector3<T> wzy
		{
			get => new Vector3<T>(w, z, y);
		}
		public Vector4<T> xywz 
		{
			get => new Vector4<T>(x, y, w, z);
		}
		public Vector4<T> xzyw 
		{
			get => new Vector4<T>(x, z, y, w);
		}
		public Vector4<T> xzwy 
		{
			get => new Vector4<T>(x, z, w, y);
		}
		public Vector4<T> xwyz 
		{
			get => new Vector4<T>(x, w, y, z);
		}
		public Vector4<T> xwzy 
		{
			get => new Vector4<T>(x, w, z, y);
		}
		public Vector4<T> yxzw 
		{
			get => new Vector4<T>(y, x, z, w);
		}
		public Vector4<T> yxwz 
		{
			get => new Vector4<T>(y, x, w, z);
		}
		public Vector4<T> yzxw 
		{
			get => new Vector4<T>(y, z, x, w);
		}
		public Vector4<T> yzwx 
		{
			get => new Vector4<T>(y, z, w, x);
		}
		public Vector4<T> ywxz 
		{
			get => new Vector4<T>(y, w, x, z);
		}
		public Vector4<T> ywzx 
		{
			get => new Vector4<T>(y, w, z, x);
		}
		public Vector4<T> zxyw 
		{
			get => new Vector4<T>(z, x, y, w);
		}
		public Vector4<T> zxwy 
		{
			get => new Vector4<T>(z, x, w, y);
		}
		public Vector4<T> zyxw 
		{
			get => new Vector4<T>(z, y, x, w);
		}
		public Vector4<T> zywx 
		{
			get => new Vector4<T>(z, y, w, x);
		}
		public Vector4<T> zwxy 
		{
			get => new Vector4<T>(z, w, x, y);
		}
		public Vector4<T> zwyx 
		{
			get => new Vector4<T>(z, w, y, x);
		}
		public Vector4<T> wxyz 
		{
			get => new Vector4<T>(w, x, y, z);
		}
		public Vector4<T> wxzy 
		{
			get => new Vector4<T>(w, x, z, y);
		}
		public Vector4<T> wyxz 
		{
			get => new Vector4<T>(w, y, x, z);
		}
		public Vector4<T> wyzx 
		{
			get => new Vector4<T>(w, y, z, x);
		}
		public Vector4<T> wzxy 
		{
			get => new Vector4<T>(w, z, x, y);
		}
		public Vector4<T> wzyx 
		{
			get => new Vector4<T>(w, z, y, x);
		}

		public static Vector4<T> Empty
		{
			get => new Vector4<T>();
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
		public bool Equals(Vector4<T> other) => x.Equals(other.x) && y.Equals(other.y) && z.Equals(other.z) && w.Equals(other.w);
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
			{
				return false;
			}
			
			return obj is Vector4<T> && Equals((Vector4<T>)obj);
		}
		public override string ToString() => $"{x}, {y}, {z}, {w}";
		public int CompareTo(Vector4<T> other)
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
			
			if (!(obj is Vector4<T>))
			{
				throw new ArgumentException($"Object must be of type {nameof(Vector4<T>)}");
			}
			
			return CompareTo((Vector4<T>)obj);
		}
	}
}