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
	[DebuggerDisplay("{_m00}, {_m01}, {_m02}, {_m03} | {_m10}, {_m11}, {_m12}, {_m13} | {_m20}, {_m21}, {_m22}, {_m23} | {_m30}, {_m31}, {_m32}, {_m33}")]
	public struct Matrix4x4<T> : IMatrix, IMatrix<T>, IEquatable<Matrix4x4<T>>, IComparable<Matrix4x4<T>>, IComparable
		where T: struct, IEquatable<T>, IComparable<T>, IComparable
	{
		public T _m00, _m01, _m02, _m03, _m10, _m11, _m12, _m13, _m20, _m21, _m22, _m23, _m30, _m31, _m32, _m33;

		public Matrix4x4(
			Vector4<T> col0 = default(Vector4<T>), 
			Vector4<T> col1 = default(Vector4<T>), 
			Vector4<T> col2 = default(Vector4<T>),
			Vector4<T> col3 = default(Vector4<T>)) : this(
			col0.x, col0.y, col0.z, 
			col1.x, col1.y, col1.z, 
			col2.x, col2.y, col2.z) { }
		
		public Matrix4x4(
			T m00 = default(T), T m01 = default(T), T m02 = default(T), T m03 = default(T),
			T m10 = default(T), T m11 = default(T), T m12 = default(T), T m13 = default(T),
			T m20 = default(T), T m21 = default(T), T m22 = default(T), T m23 = default(T),
			T m30 = default(T), T m31 = default(T), T m32 = default(T), T m33 = default(T))
		{
			_m00 = m00;
			_m01 = m01;
			_m02 = m02;
			_m03 = m03;
			_m10 = m10;
			_m11 = m11;
			_m12 = m12;
			_m13 = m13;
			_m20 = m20;
			_m21 = m21;
			_m22 = m22;
			_m23 = m23;
			_m30 = m30;
			_m31 = m31;
			_m32 = m32;
			_m33 = m33;
		}

		[NotNull]
		public IVector<T> this[int i]
		{
			get
			{
				switch (i)
				{
					case 0: return new Vector4<T>( _m00, _m01, _m02, _m03 );
					case 1: return new Vector4<T>( _m10, _m11, _m12, _m13 );
					case 2: return new Vector4<T>( _m20, _m21, _m22, _m23 );
					case 3: return new Vector4<T>( _m30, _m31, _m32, _m33 );
				}

				throw new IndexOutOfRangeException();
			}
		}

		public static Matrix4x4<T> Empty
		{
			get => new Matrix4x4<T>();
		}

		[Pure]
		public Matrix4x4<T> Transpose() => new Matrix4x4<T>(
			new Vector4<T>(_m00, _m10, _m20, _m30),
			new Vector4<T>(_m01, _m11, _m21, _m31),
			new Vector4<T>(_m02, _m12, _m22, _m32),
			new Vector4<T>(_m03, _m13, _m23, _m33));
		
		[SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = _m00.GetHashCode();
				hashCode = (hashCode * 397) ^ _m01.GetHashCode();
				hashCode = (hashCode * 397) ^ _m02.GetHashCode();
				hashCode = (hashCode * 397) ^ _m03.GetHashCode();
				hashCode = (hashCode * 397) ^ _m10.GetHashCode();
				hashCode = (hashCode * 397) ^ _m11.GetHashCode();
				hashCode = (hashCode * 397) ^ _m12.GetHashCode();
				hashCode = (hashCode * 397) ^ _m13.GetHashCode();
				hashCode = (hashCode * 397) ^ _m20.GetHashCode();
				hashCode = (hashCode * 397) ^ _m21.GetHashCode();
				hashCode = (hashCode * 397) ^ _m22.GetHashCode();
				hashCode = (hashCode * 397) ^ _m23.GetHashCode();
				hashCode = (hashCode * 397) ^ _m30.GetHashCode();
				hashCode = (hashCode * 397) ^ _m31.GetHashCode();
				hashCode = (hashCode * 397) ^ _m32.GetHashCode();
				hashCode = (hashCode * 397) ^ _m33.GetHashCode();
				return hashCode;
			}
		}
		public bool Equals(Matrix4x4<T> other) => 
			_m00.Equals(other._m00) && _m01.Equals(other._m01) && _m02.Equals(other._m02) && _m03.Equals(other._m03) &&
			_m10.Equals(other._m10) && _m11.Equals(other._m11) && _m12.Equals(other._m12) && _m13.Equals(other._m13) &&
			_m20.Equals(other._m20) && _m21.Equals(other._m21) && _m22.Equals(other._m22) && _m23.Equals(other._m23) &&
			_m30.Equals(other._m30) && _m31.Equals(other._m31) && _m32.Equals(other._m32) && _m33.Equals(other._m33);
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
			{
				return false;
			}
			
			return obj is Matrix4x4<T> && Equals((Matrix4x4<T>)obj);
		}
		public override string ToString() => $"{_m00}, {_m01}, {_m02}, {_m03} | {_m10}, {_m11}, {_m12}, {_m13} | {_m20}, {_m21}, {_m22}, {_m23} | {_m30}, {_m31}, {_m32}, {_m33}";
		public int CompareTo(Matrix4x4<T> other)
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
			
			var m03Comparison = _m03.CompareTo(other._m03);
			if (m03Comparison != 0)
			{
				return m03Comparison;
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
			
			var m13Comparison = _m13.CompareTo(other._m13);
			if (m13Comparison != 0)
			{
				return m13Comparison;
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
			
			var m22Comparison = _m22.CompareTo(other._m22);
			if (m22Comparison != 0)
			{
				return m22Comparison;
			}
			
			var m23Comparison = _m23.CompareTo(other._m23);
			if (m23Comparison != 0)
			{
				return m23Comparison;
			}
			
			var m30Comparison = _m30.CompareTo(other._m30);
			if (m30Comparison != 0)
			{
				return m30Comparison;
			}
			
			var m31Comparison = _m31.CompareTo(other._m31);
			if (m31Comparison != 0)
			{
				return m31Comparison;
			}
			
			var m32Comparison = _m32.CompareTo(other._m32);
			if (m32Comparison != 0)
			{
				return m32Comparison;
			}
			
			return _m33.CompareTo(other._m33);
		}
		public int CompareTo(object obj)
		{
			if (ReferenceEquals(null, obj))
			{
				return 1;
			}
			
			if (!(obj is Matrix4x4<T>))
			{
				throw new ArgumentException($"Object must be of type {nameof(Matrix4x4<T>)}");
			}
			
			return CompareTo((Matrix4x4<T>)obj);
		}
		
		public IVector[] GetRows() => new IVector[]
		{
			new Vector4<T>(_m00, _m10, _m20, _m30),
			new Vector4<T>(_m01, _m11, _m21, _m31),
			new Vector4<T>(_m02, _m12, _m22, _m32),
			new Vector4<T>(_m03, _m13, _m23, _m33)
		};
		
		public IVector[] GetColumns()=> new IVector[]
		{
			new Vector4<T>(_m00, _m01, _m02, _m03),
			new Vector4<T>(_m10, _m11, _m12, _m13),
			new Vector4<T>(_m20, _m21, _m22, _m23),
			new Vector4<T>(_m30, _m31, _m32, _m33)
		};
		
		IVector<T>[] IMatrix<T>.GetColumns() => new IVector<T>[]
		{
			new Vector4<T>(_m00, _m10, _m20, _m30),
			new Vector4<T>(_m01, _m11, _m21, _m31),
			new Vector4<T>(_m02, _m12, _m22, _m32),
			new Vector4<T>(_m03, _m13, _m23, _m33)
		};
		
		IVector<T>[] IMatrix<T>.GetRows() => new IVector<T>[]
		{
			new Vector4<T>(_m00, _m01, _m02, _m03),
			new Vector4<T>(_m10, _m11, _m12, _m13),
			new Vector4<T>(_m20, _m21, _m22, _m23),
			new Vector4<T>(_m30, _m31, _m32, _m33)
		};
		
		Type IMatrix.ComponentType
		{
			get => typeof(T);
		}
	}
}