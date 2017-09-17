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
	[DebuggerDisplay("{_m00}, {_m01}, {_m02} | {_m10}, {_m11}, {_m12} | {_m20}, {_m21}, {_m22}")]
	public struct Matrix3x3<T> : IMatrix, IMatrix<T>, IEquatable<Matrix3x3<T>>, IComparable<Matrix3x3<T>>, IComparable
		where T: struct, IEquatable<T>, IComparable<T>, IComparable
	{
		public T _m00, _m01, _m02, _m10, _m11, _m12, _m20, _m21, _m22;

		public Matrix3x3(
			Vector3<T> col0 = default(Vector3<T>), 
			Vector3<T> col1 = default(Vector3<T>), 
			Vector3<T> col2 = default(Vector3<T>)) : this(
			col0.x, col0.y, col0.z, 
			col1.x, col1.y, col1.z, 
			col2.x, col2.y, col2.z) { }
		
		public Matrix3x3(
			T m00 = default(T), T m01 = default(T), T m02 = default(T),
			T m10 = default(T), T m11 = default(T), T m12 = default(T),
			T m20 = default(T), T m21 = default(T), T m22 = default(T))
		{
			_m00 = m00;
			_m01 = m01;
			_m02 = m02;
			_m10 = m10;
			_m11 = m11;
			_m12 = m12;
			_m20 = m20;
			_m21 = m21;
			_m22 = m22;
		}

		[NotNull]
		public IVector<T> this[int i]
		{
			get
			{
				switch (i)
				{
					case 0: return new Vector3<T>( _m00, _m01, _m02 );
					case 1: return new Vector3<T>( _m10, _m11, _m12 );
					case 2: return new Vector3<T>( _m20, _m21, _m22 );
				}

				throw new IndexOutOfRangeException();
			}
		}

		public static Matrix3x3<T> Empty
		{
			get => new Matrix3x3<T>();
		}

		[Pure]
		public Matrix3x3<T> Transpose() => new Matrix3x3<T>(
			new Vector3<T>(_m00, _m10, _m20),
			new Vector3<T>(_m01, _m11, _m21),
			new Vector3<T>(_m02, _m12, _m22));

		[SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = _m00.GetHashCode();
				hashCode = (hashCode * 397) ^ _m01.GetHashCode();
				hashCode = (hashCode * 397) ^ _m02.GetHashCode();
				hashCode = (hashCode * 397) ^ _m10.GetHashCode();
				hashCode = (hashCode * 397) ^ _m11.GetHashCode();
				hashCode = (hashCode * 397) ^ _m12.GetHashCode();
				hashCode = (hashCode * 397) ^ _m20.GetHashCode();
				hashCode = (hashCode * 397) ^ _m21.GetHashCode();
				hashCode = (hashCode * 397) ^ _m22.GetHashCode();
				return hashCode;
			}
		}
		public bool Equals(Matrix3x3<T> other) => 
			_m00.Equals(other._m00) && _m01.Equals(other._m01) && _m02.Equals(other._m02) &&
			_m10.Equals(other._m10) && _m11.Equals(other._m11) && _m12.Equals(other._m12) &&
			_m20.Equals(other._m20) && _m21.Equals(other._m21) && _m22.Equals(other._m22);
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
			{
				return false;
			}
			
			return obj is Matrix3x3<T> && Equals((Matrix3x3<T>)obj);
		}
		public override string ToString() => $"{_m00}, {_m01}, {_m02} | {_m10}, {_m11}, {_m12} | {_m20}, {_m21}, {_m22}";
		public int CompareTo(Matrix3x3<T> other)
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
			
			var m02Comparison = _m02.CompareTo(other._m02);
			if (m02Comparison != 0)
			{
				return m02Comparison;
			}
			
			var m10Comparison = _m10.CompareTo(other._m10);
			if (m10Comparison != 0)
			{
				return m10Comparison;
			}
			
			var m11Comparison = _m11.CompareTo(other._m11);
			if (m11Comparison != 0)
			{
				return m11Comparison;
			}
			
			var m12Comparison = _m12.CompareTo(other._m12);
			if (m12Comparison != 0)
			{
				return m12Comparison;
			}
			
			var m20Comparison = _m20.CompareTo(other._m20);
			if (m20Comparison != 0)
			{
				return m20Comparison;
			}
			
			var m21Comparison = _m21.CompareTo(other._m21);
			if (m21Comparison != 0)
			{
				return m21Comparison;
			}
			
			return _m22.CompareTo(other._m22);
		}
		public int CompareTo(object obj)
		{
			if (ReferenceEquals(null, obj))
			{
				return 1;
			}
			
			if (!(obj is Matrix3x3<T>))
			{
				throw new ArgumentException($"Object must be of type {nameof(Matrix3x3<T>)}");
			}
			
			return CompareTo((Matrix3x3<T>)obj);
		}
		
		public IVector[] GetRows() => new IVector[]
		{
			new Vector3<T>(_m00, _m10, _m20),
			new Vector3<T>(_m01, _m11, _m21),
			new Vector3<T>(_m02, _m12, _m22)
		};
		
		public IVector[] GetColumns()=> new IVector[]
		{
			new Vector3<T>(_m00, _m01, _m02),
			new Vector3<T>(_m10, _m11, _m12),
			new Vector3<T>(_m20, _m21, _m22)
		};
		
		IVector<T>[] IMatrix<T>.GetColumns() => new IVector<T>[]
		{
			new Vector3<T>(_m00, _m10, _m20),
			new Vector3<T>(_m01, _m11, _m21),
			new Vector3<T>(_m02, _m12, _m22)
		};
		
		IVector<T>[] IMatrix<T>.GetRows() => new IVector<T>[]
		{
			new Vector3<T>(_m00, _m01, _m02),
			new Vector3<T>(_m10, _m11, _m12),
			new Vector3<T>(_m20, _m21, _m22)
		};
		
		Type IMatrix.ComponentType
		{
			get => typeof(T);
		}
	}
}