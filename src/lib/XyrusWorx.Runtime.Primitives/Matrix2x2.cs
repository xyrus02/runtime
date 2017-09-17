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
	[DebuggerDisplay("{_m00}, {_m01} | {_m10}, {_m11}")]
	public struct Matrix2x2<T> : IMatrix, IMatrix<T>, IEquatable<Matrix2x2<T>>, IComparable<Matrix2x2<T>>, IComparable
		where T: struct, IEquatable<T>, IComparable<T>, IComparable
	{
		public T _m00, _m01, _m10, _m11;

		public Matrix2x2(
			Vector2<T> col0 = default(Vector2<T>), 
			Vector2<T> col1 = default(Vector2<T>)) : this(
			col0.x, col0.y, 
			col1.x, col1.y) { }
		
		public Matrix2x2(
			T m00 = default(T), T m01 = default(T),
			T m10 = default(T), T m11 = default(T))
		{
			_m00 = m00;
			_m01 = m01;
			_m10 = m10;
			_m11 = m11;
		}

		[NotNull]
		public IVector<T> this[int i]
		{
			get
			{
				switch (i)
				{
					case 0: return new Vector2<T>(_m00, _m01);
					case 1: return new Vector2<T>(_m10, _m11);
				}

				throw new IndexOutOfRangeException();
			}
		}

		public static Matrix2x2<T> Empty
		{
			get => new Matrix2x2<T>();
		}

		[Pure]
		public Matrix2x2<T> Transpose() => new Matrix2x2<T>(
			new Vector2<T>(_m00, _m10),
			new Vector2<T>(_m01, _m11));
		
		[SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = _m00.GetHashCode();
				hashCode = (hashCode * 397) ^ _m01.GetHashCode();
				hashCode = (hashCode * 397) ^ _m10.GetHashCode();
				hashCode = (hashCode * 397) ^ _m11.GetHashCode();
				return hashCode;
			}
		}
		public bool Equals(Matrix2x2<T> other) => 
			_m00.Equals(other._m00) && _m01.Equals(other._m01) &&
			_m10.Equals(other._m10) && _m11.Equals(other._m11);
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
			{
				return false;
			}
			
			return obj is Matrix2x2<T> && Equals((Matrix2x2<T>)obj);
		}
		public override string ToString() => $"{_m00}, {_m01} | {_m10}, {_m11}";
		public int CompareTo(Matrix2x2<T> other)
		{
			var m00Comparison = _m00.CompareTo(other._m00);
			if (m00Comparison != 0)
			{
				return m00Comparison;
			}
			
			var m01Comparison = _m01.CompareTo(other._m01);
			if (m01Comparison != 0)
			{
				return m01Comparison;
			}
			
			var m10Comparison = _m10.CompareTo(other._m10);
			if (m10Comparison != 0)
			{
				return m10Comparison;
			}
			
			return _m11.CompareTo(other._m11);
		}
		public int CompareTo(object obj)
		{
			if (ReferenceEquals(null, obj))
			{
				return 1;
			}
			
			if (!(obj is Matrix2x2<T>))
			{
				throw new ArgumentException($"Object must be of type {nameof(Matrix2x2<T>)}");
			}
			
			return CompareTo((Matrix2x2<T>)obj);
		}
		
		public IVector[] GetRows() => new IVector[]
		{
			new Vector2<T>(_m00, _m10),
			new Vector2<T>(_m01, _m11)
		};
		
		public IVector[] GetColumns()=> new IVector[]
		{
			new Vector2<T>(_m00, _m01),
			new Vector2<T>(_m10, _m11)
		};
		
		IVector<T>[] IMatrix<T>.GetColumns() => new IVector<T>[]
		{
			new Vector2<T>(_m00, _m10),
			new Vector2<T>(_m01, _m11)
		};
		
		IVector<T>[] IMatrix<T>.GetRows() => new IVector<T>[]
		{
			new Vector2<T>(_m00, _m01),
			new Vector2<T>(_m10, _m11)
		};
		
		Type IMatrix.ComponentType
		{
			get => typeof(T);
		}
	}
}