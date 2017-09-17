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
	public struct Vector3<T> : IVectorType, IVectorType<T>, IEquatable<Vector3<T>>, IComparable<Vector3<T>>, IComparable
		where T: struct, IEquatable<T>, IComparable<T>, IComparable
	{
		public T x, y, z;
		
		object[] IVectorType.GetComponents() => new object[] {x, y, z};
		T[] IVectorType<T>.GetComponents() => new [] {x, y, z};
		Type IVectorType.ComponentType
		{
			get => typeof(T);
		}

		public Vector3(T x = default(T), T y = default(T), T z = default(T))
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}
		public Vector3(Vector2<T> xy, T z = default(T)) : this(xy.x, xy.y, z)
		{
		}
		public Vector3(T x, Vector2<T> yz) : this(x, yz.x, yz.y)
		{
		}
		public Vector3(Vector3<T> xyz) : this(xyz.x, xyz.y, xyz.z)
		{
		}

		public static bool operator ==(Vector3<T> left, Vector3<T> right) => left.Equals(right);
		public static bool operator !=(Vector3<T> left, Vector3<T> right) => !left.Equals(right);
		public static bool operator <(Vector3<T> left, Vector3<T> right) => left.CompareTo(right) < 0;
		public static bool operator >(Vector3<T> left, Vector3<T> right) => left.CompareTo(right) > 0;
		public static bool operator <=(Vector3<T> left, Vector3<T> right) => left.CompareTo(right) <= 0;
		public static bool operator >=(Vector3<T> left, Vector3<T> right) => left.CompareTo(right) >= 0;

		public Vector2<T> xy
		{
			get => new Vector2<T>(x, y);
		}
		public Vector2<T> yx
		{
			get => new Vector2<T>(y, x);
		}
		public Vector2<T> yz
		{
			get => new Vector2<T>(y, z);
		}
		public Vector2<T> zy
		{
			get => new Vector2<T>(z, y);
		}
		public Vector2<T> xz
		{
			get => new Vector2<T>(x, z);
		}
		public Vector2<T> zx
		{
			get => new Vector2<T>(z, x);
		}
		public Vector3<T> xzy
		{
			get => new Vector3<T>(x, z, y);
		}
		public Vector3<T> yxz
		{
			get => new Vector3<T>(y, x, z);
		}
		public Vector3<T> yzx
		{
			get => new Vector3<T>(y, z, x);
		}
		public Vector3<T> zxy
		{
			get => new Vector3<T>(z, x, y);
		}
		public Vector3<T> zyx
		{
			get => new Vector3<T>(z, y, x);
		}
		
		public static Vector3<T> Empty
		{
			get => new Vector3<T>();
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
		public bool Equals(Vector3<T> other) => x.Equals(other.x) && y.Equals(other.y) && z.Equals(other.z);
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
			{
				return false;
			}
			
			return obj is Vector3<T> && Equals((Vector3<T>)obj);
		}
		public override string ToString() => $"{x}, {y}, {z}";
		public int CompareTo(Vector3<T> other)
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
			
			if (!(obj is Vector3<T>))
			{
				throw new ArgumentException($"Object must be of type {nameof(Vector3<T>)}");
			}
			
			return CompareTo((Vector3<T>)obj);
		}
	}
}