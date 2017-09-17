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
	[DebuggerDisplay("{x}, {y}")]
	public struct Int2 : IVectorType, IVectorType<int>, IEquatable<Int2>, IComparable<Int2>, IComparable
	{
		public int x, y;

		object[] IVectorType.GetComponents() => new object[] { x, y };
		int[] IVectorType<int>.GetComponents() => new[] { x, y };
		Type IVectorType.ComponentType
		{
			get => typeof(int);
		}

		public Int2(int x = 0, int y = 0)
		{
			this.x = x;
			this.y = y;
		}
		public Int2(Byte2 xy) : this(xy.x, xy.y)
		{
		}
		public Int2(Int2 xy) : this(xy.x, xy.y)
		{
		}

		public static Int2 operator +(int a, Int2 b) => new Int2(a + b.x, a + b.y);
		public static Int2 operator -(int a, Int2 b) => new Int2(a - b.x, a - b.y);
		public static Int2 operator *(int a, Int2 b) => new Int2(a * b.x, a * b.y);
		public static Int2 operator /(int a, Int2 b) => new Int2(a / b.x, a / b.y);
		public static Int2 operator +(Int2 a, int b) => new Int2(a.x + b, a.y + b);
		public static Int2 operator -(Int2 a, int b) => new Int2(a.x - b, a.y - b);
		public static Int2 operator *(Int2 a, int b) => new Int2(a.x * b, a.y * b);
		public static Int2 operator /(Int2 a, int b) => new Int2(a.x / b, a.y / b);
		public static Int2 operator +(Int2 a, Int2 b) => new Int2(a.x + b.x, a.y + b.y);
		public static Int2 operator -(Int2 a, Int2 b) => new Int2(a.x - b.x, a.y - b.y);
		public static Int2 operator *(Int2 a, Int2 b) => new Int2(a.x * b.x, a.y * b.y);
		public static Int2 operator /(Int2 a, Int2 b) => new Int2(a.x / b.x, a.y / b.y);
		public static bool operator ==(Int2 left, Int2 right) => left.Equals(right);
		public static bool operator !=(Int2 left, Int2 right) => !left.Equals(right);
		public static bool operator <(Int2 left, Int2 right) => left.CompareTo(right) < 0;
		public static bool operator >(Int2 left, Int2 right) => left.CompareTo(right) > 0;
		public static bool operator <=(Int2 left, Int2 right) => left.CompareTo(right) <= 0;
		public static bool operator >=(Int2 left, Int2 right) => left.CompareTo(right) >= 0;
		public static implicit operator Vector2<int>(Int2 source) => new Vector2<int>(source.x, source.y); 
		public static implicit operator Int2(Vector2<int> source) => new Int2(source.x, source.y); 

		public Byte2 Byte() => new Byte2((byte)x, (byte)y);
		
		public Int2 yx
		{
			get => new Int2(y, x);
		}
		
		public static Int2 Zero
		{
			get => new Int2(0);
		}

		[SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
		public override int GetHashCode()
		{
			unchecked
			{
				return (x.GetHashCode() * 397) ^ y.GetHashCode();
			}
		}
		public bool Equals(Int2 other) => x == other.x && y == other.y;
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
			{
				return false;
			}
			return obj is Int2 && Equals((Int2)obj);
		}
		public override string ToString() => $"{x}, {y}";
		public int CompareTo(Int2 other)
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
			
			if (!(obj is Int2))
			{
				throw new ArgumentException($"Object must be of type {nameof(Int2)}");
			}
			
			return CompareTo((Int2)obj);
		}
	}
}