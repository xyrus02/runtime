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
	public struct Int3 : IVectorType, IVectorType<int>, IEquatable<Int3>, IComparable<Int3>, IComparable
	{
		public int x, y, z;

		object[] IVectorType.GetComponents() => new object[] { x, y, z };
		int[] IVectorType<int>.GetComponents() => new[] { x, y, z };
		Type IVectorType.ComponentType
		{
			get => typeof(int);
		}

		public Int3(int x = 0, int y = 0, int z = 0)
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}
		public Int3(Int2 xy, int z = 0) : this(xy.x, xy.y, z)
		{
		}
		public Int3(int x, Int2 yz) : this(x, yz.x, yz.y)
		{
		}
		public Int3(Int3 xyz) : this(xyz.x, xyz.y, xyz.z)
		{
		}
		
		public static Int3 operator +(int a, Int3 b) => new Int3(a + b.x, a + b.y, a + b.z);
		public static Int3 operator -(int a, Int3 b) => new Int3(a - b.x, a - b.y, a - b.z);
		public static Int3 operator *(int a, Int3 b) => new Int3(a * b.x, a * b.y, a * b.z);
		public static Int3 operator /(int a, Int3 b) => new Int3(a / b.x, a / b.y, a / b.z);
		public static Int3 operator +(Int3 a, int b) => new Int3(a.x + b, a.y + b, a.z + b);
		public static Int3 operator -(Int3 a, int b) => new Int3(a.x - b, a.y - b, a.z - b);
		public static Int3 operator *(Int3 a, int b) => new Int3(a.x * b, a.y * b, a.z * b);
		public static Int3 operator /(Int3 a, int b) => new Int3(a.x / b, a.y / b, a.z / b);
		public static Int3 operator +(Int3 a, Int3 b) => new Int3(a.x + b.x, a.y + b.y, a.z + b.z);
		public static Int3 operator -(Int3 a, Int3 b) => new Int3(a.x - b.x, a.y - b.y, a.z - b.z);
		public static Int3 operator *(Int3 a, Int3 b) => new Int3(a.x * b.x, a.y * b.y, a.z * b.z);
		public static Int3 operator /(Int3 a, Int3 b) => new Int3(a.x / b.x, a.y / b.y, a.z / b.z);
		public static bool operator ==(Int3 left, Int3 right) => left.Equals(right);
		public static bool operator !=(Int3 left, Int3 right) => !left.Equals(right);
		public static bool operator <(Int3 left, Int3 right) => left.CompareTo(right) < 0;
		public static bool operator >(Int3 left, Int3 right) => left.CompareTo(right) > 0;
		public static bool operator <=(Int3 left, Int3 right) => left.CompareTo(right) <= 0;
		public static bool operator >=(Int3 left, Int3 right) => left.CompareTo(right) >= 0;
		public static implicit operator Vector3<int>(Int3 source) => new Vector3<int>(source.x, source.y, source.z); 
		public static implicit operator Int3(Vector3<int> source) => new Int3(source.x, source.y, source.z); 
		public static implicit operator Float3(Int3 source) => source.Float(); 

		public Float3 Float() => new Float3(x, y, z);
		
		public Int2 xy
		{
			get => new Int2(x, y);
		}
		public Int2 yx
		{
			get => new Int2(y, x);
		}
		public Int2 yz
		{
			get => new Int2(y, z);
		}
		public Int2 zy
		{
			get => new Int2(z, y);
		}
		public Int2 xz
		{
			get => new Int2(x, z);
		}
		public Int2 zx
		{
			get => new Int2(z, x);
		}
		public Int3 xzy
		{
			get => new Int3(x, z, y);
		}
		public Int3 yxz
		{
			get => new Int3(y, x, z);
		}
		public Int3 yzx
		{
			get => new Int3(y, z, x);
		}
		public Int3 zxy
		{
			get => new Int3(z, x, y);
		}
		public Int3 zyx
		{
			get => new Int3(z, y, x);
		}

		public static Int3 Zero
		{
			get => new Int3(0);
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
		public bool Equals(Int3 other) => x == other.x && y == other.y && z == other.z;
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
			{
				return false;
			}
			
			return obj is Int3 && Equals((Int3)obj);
		}
		public override string ToString() => $"{x}, {y}, {z}";
		public int CompareTo(Int3 other)
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
			
			if (!(obj is Int3))
			{
				throw new ArgumentException($"Object must be of type {nameof(Int3)}");
			}
			
			return CompareTo((Int3)obj);
		}
	}
}