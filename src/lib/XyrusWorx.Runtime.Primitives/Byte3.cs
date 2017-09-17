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
	public struct Byte3 : IEquatable<Byte3>, IComparable<Byte3>, IComparable
	{
		public byte x, y, z;

		public Byte3(byte x = 0, byte y = 0, byte z = 0)
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}
		public Byte3(Byte2 xy, byte z = 0) : this(xy.x, xy.y, z)
		{
		}
		public Byte3(byte x, Byte2 yz) : this(x, yz.x, yz.y)
		{
		}
		public Byte3(Byte3 xyz) : this(xyz.x, xyz.y, xyz.z)
		{
		}

		public static Byte3 operator +(byte a, Byte3 b) => new Byte3((byte)(a + b.x), (byte)(a + b.y), (byte)(a + b.z));
		public static Byte3 operator -(byte a, Byte3 b) => new Byte3((byte)(a - b.x), (byte)(a - b.y), (byte)(a - b.z));
		public static Byte3 operator *(byte a, Byte3 b) => new Byte3((byte)(a * b.x), (byte)(a * b.y), (byte)(a * b.z));
		public static Byte3 operator /(byte a, Byte3 b) => new Byte3((byte)(a / b.x), (byte)(a / b.y), (byte)(a / b.z));
		public static Byte3 operator +(Byte3 a, byte b) => new Byte3((byte)(a.x + b), (byte)(a.y + b), (byte)(a.z + b));
		public static Byte3 operator -(Byte3 a, byte b) => new Byte3((byte)(a.x - b), (byte)(a.y - b), (byte)(a.z - b));
		public static Byte3 operator *(Byte3 a, byte b) => new Byte3((byte)(a.x * b), (byte)(a.y * b), (byte)(a.z * b));
		public static Byte3 operator /(Byte3 a, byte b) => new Byte3((byte)(a.x / b), (byte)(a.y / b), (byte)(a.z / b));
		public static Byte3 operator +(Byte3 a, Byte3 b) => new Byte3((byte)(a.x + b.x), (byte)(a.y + b.y), (byte)(a.z + b.z));
		public static Byte3 operator -(Byte3 a, Byte3 b) => new Byte3((byte)(a.x - b.x), (byte)(a.y - b.y), (byte)(a.z - b.z));
		public static Byte3 operator *(Byte3 a, Byte3 b) => new Byte3((byte)(a.x * b.x), (byte)(a.y * b.y), (byte)(a.z * b.z));
		public static Byte3 operator /(Byte3 a, Byte3 b) => new Byte3((byte)(a.x / b.x), (byte)(a.y / b.y), (byte)(a.z / b.z));
		public static bool operator ==(Byte3 left, Byte3 right) => left.Equals(right);
		public static bool operator !=(Byte3 left, Byte3 right) => !left.Equals(right);
		public static bool operator <(Byte3 left, Byte3 right) => left.CompareTo(right) < 0;
		public static bool operator >(Byte3 left, Byte3 right) => left.CompareTo(right) > 0;
		public static bool operator <=(Byte3 left, Byte3 right) => left.CompareTo(right) <= 0;
		public static bool operator >=(Byte3 left, Byte3 right) => left.CompareTo(right) >= 0;

		public Byte2 xy
		{
			get => new Byte2(x, y);
		}
		public Byte2 yx
		{
			get => new Byte2(y, x);
		}
		public Byte2 yz
		{
			get => new Byte2(y, z);
		}
		public Byte2 zy
		{
			get => new Byte2(z, y);
		}
		public Byte2 xz
		{
			get => new Byte2(x, z);
		}
		public Byte2 zx
		{
			get => new Byte2(z, x);
		}
		public Byte3 xzy
		{
			get => new Byte3(x, z, y);
		}
		public Byte3 yxz
		{
			get => new Byte3(y, x, z);
		}
		public Byte3 yzx
		{
			get => new Byte3(y, z, x);
		}
		public Byte3 zxy
		{
			get => new Byte3(z, x, y);
		}
		public Byte3 zyx
		{
			get => new Byte3(z, y, x);
		}
		
		public static Byte3 Zero
		{
			get => new Byte3(0);
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
		public bool Equals(Byte3 other) => x == other.x && y == other.y && z == other.z;
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
			{
				return false;
			}
			
			return obj is Byte3 && Equals((Byte3)obj);
		}
		public override string ToString() => $"{x}, {y}, {z}";
		public int CompareTo(Byte3 other)
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
			
			if (!(obj is Byte3))
			{
				throw new ArgumentException($"Object must be of type {nameof(Byte3)}");
			}
			
			return CompareTo((Byte3)obj);
		}
	}
}