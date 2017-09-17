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
	public struct Vector2<T> : IVector, IVector<T>, IEquatable<Vector2<T>>, IComparable<Vector2<T>>, IComparable
		where T: struct, IEquatable<T>, IComparable<T>, IComparable
	{
		public T x, y;
		
		object[] IVector.GetComponents() => new object[] {x, y};
		T[] IVector<T>.GetComponents() => new [] {x, y};
		Type IVector.ComponentType
		{
			get => typeof(T);
		}
		
		public Vector2(T x = default(T), T y = default(T))
		{
			this.x = x;
			this.y = y;
		}
		public Vector2(Vector2<T> xy) : this(xy.x, xy.y)
		{
		}
		
		public static bool operator ==(Vector2<T> left, Vector2<T> right) => left.Equals(right);
		public static bool operator !=(Vector2<T> left, Vector2<T> right) => !left.Equals(right);
		public static bool operator <(Vector2<T> left, Vector2<T> right) => left.CompareTo(right) < 0;
		public static bool operator >(Vector2<T> left, Vector2<T> right) => left.CompareTo(right) > 0;
		public static bool operator <=(Vector2<T> left, Vector2<T> right) => left.CompareTo(right) <= 0;
		public static bool operator >=(Vector2<T> left, Vector2<T> right) => left.CompareTo(right) >= 0;
		
		public T this[int i]
		{
			get
			{
				switch (i)
				{
					case 0: return x;
					case 1: return y;
				}
				
				throw new IndexOutOfRangeException();
			}
		}
		
		public Vector2<T> yx
		{
			get => new Vector2<T>(y, x);
		}
		
		public static Vector2<T> Empty
		{
			get => new Vector2<T>();
		}
		
		[SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
		public override int GetHashCode()
		{
			unchecked
			{
				return (x.GetHashCode() * 397) ^ y.GetHashCode();
			}
		}
		public bool Equals(Vector2<T> other) => x.Equals(other.x) && y.Equals(other.y);
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
			{
				return false;
			}
			return obj is Vector2<T> && Equals((Vector2<T>)obj);
		}
		public override string ToString() => $"{x}, {y}";
		public int CompareTo(Vector2<T> other)
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
			
			if (!(obj is Vector2<T>))
			{
				throw new ArgumentException($"Object must be of type {nameof(Vector2<T>)}");
			}
			
			return CompareTo((Vector2<T>)obj);
		}
	}
}
