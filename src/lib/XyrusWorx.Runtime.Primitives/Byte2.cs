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
	public struct Byte2 : IVectorType, IVectorType<byte>, IEquatable<Byte2>, IComparable<Byte2>, IComparable
	{
		public byte x, y;
		
		object[] IVectorType.GetComponents() => new object[] {x, y};
		byte[] IVectorType<byte>.GetComponents() => new [] {x, y};
		Type IVectorType.ComponentType
		{
			get => typeof(byte);
		}

		public Byte2(byte x = 0, byte y = 0)
		{
			this.x = x;
			this.y = y;
		}
		public Byte2(Byte2 xy) : this(xy.x, xy.y)
		{
		}

		public static Byte2 operator +(byte a, Byte2 b) => new Byte2((byte)(a + b.x), (byte)(a + b.y));
		public static Byte2 operator -(byte a, Byte2 b) => new Byte2((byte)(a - b.x), (byte)(a - b.y));
		public static Byte2 operator *(byte a, Byte2 b) => new Byte2((byte)(a * b.x), (byte)(a * b.y));
		public static Byte2 operator /(byte a, Byte2 b) => new Byte2((byte)(a / b.x), (byte)(a / b.y));
		public static Byte2 operator +(Byte2 a, byte b) => new Byte2((byte)(a.x + b), (byte)(a.y + b));
		public static Byte2 operator -(Byte2 a, byte b) => new Byte2((byte)(a.x - b), (byte)(a.y - b));
		public static Byte2 operator *(Byte2 a, byte b) => new Byte2((byte)(a.x * b), (byte)(a.y * b));
		public static Byte2 operator /(Byte2 a, byte b) => new Byte2((byte)(a.x / b), (byte)(a.y / b));
		public static Byte2 operator +(Byte2 a, Byte2 b) => new Byte2((byte)(a.x + b.x), (byte)(a.y + b.y));
		public static Byte2 operator -(Byte2 a, Byte2 b) => new Byte2((byte)(a.x - b.x), (byte)(a.y - b.y));
		public static Byte2 operator *(Byte2 a, Byte2 b) => new Byte2((byte)(a.x * b.x), (byte)(a.y * b.y));
		public static Byte2 operator /(Byte2 a, Byte2 b) => new Byte2((byte)(a.x / b.x), (byte)(a.y / b.y));
		public static bool operator ==(Byte2 left, Byte2 right) => left.Equals(right);
		public static bool operator !=(Byte2 left, Byte2 right) => !left.Equals(right);
		public static bool operator <(Byte2 left, Byte2 right) => left.CompareTo(right) < 0;
		public static bool operator >(Byte2 left, Byte2 right) => left.CompareTo(right) > 0;
		public static bool operator <=(Byte2 left, Byte2 right) => left.CompareTo(right) <= 0;
		public static bool operator >=(Byte2 left, Byte2 right) => left.CompareTo(right) >= 0;
		public static implicit operator Vector2<byte>(Byte2 source) => new Vector2<byte>(source.x, source.y); 
		public static implicit operator Byte2(Vector2<byte> source) => new Byte2(source.x, source.y); 

		public Byte2 yx
		{
			get => new Byte2(y, x);
		}
		
		public static Byte2 Zero
		{
			get => new Byte2(0);
		}
		
		[SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
		public override int GetHashCode()
		{
			unchecked
			{
				return (x.GetHashCode() * 397) ^ y.GetHashCode();
			}
		}
		public bool Equals(Byte2 other) => x == other.x && y == other.y;
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
			{
				return false;
			}
			return obj is Byte2 && Equals((Byte2)obj);
		}
		public override string ToString() => $"{x}, {y}";
		public int CompareTo(Byte2 other)
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
			
			if (!(obj is Byte2))
			{
				throw new ArgumentException($"Object must be of type {nameof(Byte2)}");
			}
			
			return CompareTo((Byte2)obj);
		}
	}
}