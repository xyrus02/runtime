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
	public struct Float4x4 : IMatrix, IMatrix<float>, IEquatable<Float4x4>, IComparable<Float4x4>, IComparable, IMatrixCellWriter
	{
		public float _m00, _m01, _m02, _m03, _m10, _m11, _m12, _m13, _m20, _m21, _m22, _m23, _m30, _m31, _m32, _m33;

		public Float4x4(
			Float4 col0 = default(Float4), 
			Float4 col1 = default(Float4), 
			Float4 col2 = default(Float4),
			Float4 col3 = default(Float4)) : this(
			col0.x, col0.y, col0.z, 
			col1.x, col1.y, col1.z, 
			col2.x, col2.y, col2.z) { }
		
		public Float4x4(
			float m00 = 0, float m01 = 0, float m02 = 0, float m03 = 0,
			float m10 = 0, float m11 = 0, float m12 = 0, float m13 = 0,
			float m20 = 0, float m21 = 0, float m22 = 0, float m23 = 0,
			float m30 = 0, float m31 = 0, float m32 = 0, float m33 = 0)
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

		public static Float4x4 operator +(Float4x4 a, Float4x4 b) => new Float4x4(
			a._m00 + b._m00, a._m01 + b._m01, a._m02 + b._m02, a._m03 + b._m03,
			a._m10 + b._m10, a._m11 + b._m11, a._m12 + b._m12, a._m13 + b._m13,
			a._m20 + b._m20, a._m21 + b._m21, a._m22 + b._m22, a._m23 + b._m23,
			a._m30 + b._m30, a._m31 + b._m31, a._m32 + b._m32, a._m33 + b._m33);

		public static Float4x4 operator -(Float4x4 a, Float4x4 b) => new Float4x4(
			a._m00 - b._m00, a._m01 - b._m01, a._m02 - b._m02, a._m03 - b._m03,
			a._m10 - b._m10, a._m11 - b._m11, a._m12 - b._m12, a._m13 - b._m13,
			a._m20 - b._m20, a._m21 - b._m21, a._m22 - b._m22, a._m23 - b._m23,
			a._m30 - b._m30, a._m31 - b._m31, a._m32 - b._m32, a._m33 - b._m33);

		public static Float4x4 operator *(Float4x4 a, Float4x4 b)
		{
			var c = new Float4x4();

			c._m00 = a._m00 * b._m00 + a._m01 * b._m10 + a._m02 * b._m20 + a._m03 * b._m30;
			c._m01 = a._m00 * b._m01 + a._m01 * b._m11 + a._m02 * b._m21 + a._m03 * b._m31;
			c._m02 = a._m00 * b._m02 + a._m01 * b._m12 + a._m02 * b._m22 + a._m03 * b._m32;
			c._m03 = a._m00 * b._m03 + a._m01 * b._m13 + a._m02 * b._m23 + a._m03 * b._m33;

			c._m10 = a._m10 * b._m00 + a._m11 * b._m10 + a._m12 * b._m20 + a._m13 * b._m30;
			c._m11 = a._m10 * b._m01 + a._m11 * b._m11 + a._m12 * b._m21 + a._m13 * b._m31;
			c._m12 = a._m10 * b._m02 + a._m11 * b._m12 + a._m12 * b._m22 + a._m13 * b._m32;
			c._m13 = a._m10 * b._m03 + a._m11 * b._m13 + a._m12 * b._m23 + a._m13 * b._m33;

			c._m20 = a._m20 * b._m00 + a._m21 * b._m10 + a._m22 * b._m20 + a._m23 * b._m30;
			c._m21 = a._m20 * b._m01 + a._m21 * b._m11 + a._m22 * b._m21 + a._m23 * b._m31;
			c._m22 = a._m20 * b._m02 + a._m21 * b._m12 + a._m22 * b._m22 + a._m23 * b._m32;
			c._m23 = a._m20 * b._m03 + a._m21 * b._m13 + a._m22 * b._m23 + a._m23 * b._m33;
			
			c._m30 = a._m30 * b._m00 + a._m31 * b._m10 + a._m32 * b._m20 + a._m33 * b._m30;
			c._m31 = a._m30 * b._m01 + a._m31 * b._m11 + a._m32 * b._m21 + a._m33 * b._m31;
			c._m32 = a._m30 * b._m02 + a._m31 * b._m12 + a._m32 * b._m22 + a._m33 * b._m32;
			c._m33 = a._m30 * b._m03 + a._m31 * b._m13 + a._m32 * b._m23 + a._m33 * b._m33;

			return c;
		}
		public static Float4x4 operator *(Float4x4 a, float b) => new Float4x4(
			a._m00 * b, a._m01 * b, a._m02 * b, a._m03 * b,
			a._m10 * b, a._m11 * b, a._m12 * b, a._m13 * b,
			a._m20 * b, a._m21 * b, a._m22 * b, a._m23 * b,
			a._m30 * b, a._m31 * b, a._m32 * b, a._m33 * b);

		[NotNull]
		public IVector<float> this[int i]
		{
			get
			{
				switch (i)
				{
					case 0: return new Float4( _m00, _m01, _m02, _m03 );
					case 1: return new Float4( _m10, _m11, _m12, _m13 );
					case 2: return new Float4( _m20, _m21, _m22, _m23 );
					case 3: return new Float4( _m30, _m31, _m32, _m33 );
				}

				throw new IndexOutOfRangeException();
			}
		}

		public static Float4x4 Zero
		{
			get => new Float4x4();
		}
		public static Float4x4 Identity
		{
			get => new Float4x4(
				1, 0, 0, 0,
				0, 1, 0, 0,
				0, 0, 1, 0,
				0, 0, 0, 1);
		}

		[Pure]
		public Float4x4 Transpose() => new Float4x4(
			new Float4(_m00, _m10, _m20, _m30),
			new Float4(_m01, _m11, _m21, _m31),
			new Float4(_m02, _m12, _m22, _m32),
			new Float4(_m03, _m13, _m23, _m33));
		
		[Pure]
		public float Determinant() =>
			_m03 * _m12 * _m21 * _m30 - _m02 * _m13 * _m21 * _m30 -
			_m03 * _m11 * _m22 * _m30 + _m01 * _m13 * _m22 * _m30 +
			_m02 * _m11 * _m23 * _m30 - _m01 * _m12 * _m23 * _m30 -
			_m03 * _m12 * _m20 * _m31 + _m02 * _m13 * _m20 * _m31 +
			_m03 * _m10 * _m22 * _m31 - _m00 * _m13 * _m22 * _m31 -
			_m02 * _m10 * _m23 * _m31 + _m00 * _m12 * _m23 * _m31 +
			_m03 * _m11 * _m20 * _m32 - _m01 * _m13 * _m20 * _m32 -
			_m03 * _m10 * _m21 * _m32 + _m00 * _m13 * _m21 * _m32 +
			_m01 * _m10 * _m23 * _m32 - _m00 * _m11 * _m23 * _m32 -
			_m02 * _m11 * _m20 * _m33 + _m01 * _m12 * _m20 * _m33 +
			_m02 * _m10 * _m21 * _m33 - _m00 * _m12 * _m21 * _m33 -
			_m01 * _m10 * _m22 * _m33 + _m00 * _m11 * _m22 * _m33;
		
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
		public bool Equals(Float4x4 other) => 
			_m00 == other._m00 && _m01 == other._m01 && _m02 == other._m02 && _m03 == other._m03 &&
			_m10 == other._m10 && _m11 == other._m11 && _m12 == other._m12 && _m13 == other._m13 &&
			_m20 == other._m20 && _m21 == other._m21 && _m22 == other._m22 && _m23 == other._m23 &&
			_m30 == other._m30 && _m31 == other._m31 && _m32 == other._m32 && _m33 == other._m33;
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
			{
				return false;
			}
			
			return obj is Float4x4 && Equals((Float4x4)obj);
		}
		public override string ToString() => $"{_m00}, {_m01}, {_m02}, {_m03} | {_m10}, {_m11}, {_m12}, {_m13} | {_m20}, {_m21}, {_m22}, {_m23} | {_m30}, {_m31}, {_m32}, {_m33}";
		public int CompareTo(Float4x4 other)
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
			
			if (!(obj is Float4x4))
			{
				throw new ArgumentException($"Object must be of type {nameof(Float4x4)}");
			}
			
			return CompareTo((Float4x4)obj);
		}
		
		public IVector[] GetRows() => new IVector[]
		{
			new Float4(_m00, _m10, _m20, _m30),
			new Float4(_m01, _m11, _m21, _m31),
			new Float4(_m02, _m12, _m22, _m32),
			new Float4(_m03, _m13, _m23, _m33)
		};
		
		public IVector[] GetColumns()=> new IVector[]
		{
			new Float4(_m00, _m01, _m02, _m03),
			new Float4(_m10, _m11, _m12, _m13),
			new Float4(_m20, _m21, _m22, _m23),
			new Float4(_m30, _m31, _m32, _m33)
		};
		
		IVector<float>[] IMatrix<float>.GetColumns() => new IVector<float>[]
		{
			new Float4(_m00, _m10, _m20, _m30),
			new Float4(_m01, _m11, _m21, _m31),
			new Float4(_m02, _m12, _m22, _m32),
			new Float4(_m03, _m13, _m23, _m33)
		};
		
		IVector<float>[] IMatrix<float>.GetRows() => new IVector<float>[]
		{
			new Float4(_m00, _m01, _m02, _m03),
			new Float4(_m10, _m11, _m12, _m13),
			new Float4(_m20, _m21, _m22, _m23),
			new Float4(_m30, _m31, _m32, _m33)
		};
		
		Type IMatrix.ComponentType
		{
			get => typeof(float);
		}
		
		IMatrix IMatrixCellWriter.Set(int column, int row, object value)
		{
			var t = (float)value;

			switch (column * 4 + row)
			{
				case 00: return new Float4x4(t, _m01, _m02, _m03, _m10, _m11, _m12, _m13, _m20, _m21, _m22, _m23, _m30, _m31, _m32, _m33);
				case 01: return new Float4x4(_m00, t, _m02, _m03, _m10, _m11, _m12, _m13, _m20, _m21, _m22, _m23, _m30, _m31, _m32, _m33);
				case 02: return new Float4x4(_m00, _m01, t, _m03, _m10, _m11, _m12, _m13, _m20, _m21, _m22, _m23, _m30, _m31, _m32, _m33);
				case 03: return new Float4x4(_m10, _m01, _m02, t, _m10, _m11, _m12, _m13, _m20, _m21, _m22, _m23, _m30, _m31, _m32, _m33);
				
				case 04: return new Float4x4(_m00, _m01, _m02, _m03, t, _m11, _m12, _m13, _m20, _m21, _m22, _m23, _m30, _m31, _m32, _m33);
				case 05: return new Float4x4(_m00, _m01, _m02, _m03, _m10, t, _m12, _m13, _m20, _m21, _m22, _m23, _m30, _m31, _m32, _m33);
				case 06: return new Float4x4(_m10, _m01, _m02, _m03, _m10, _m11, t, _m13, _m20, _m21, _m22, _m23, _m30, _m31, _m32, _m33);
				case 07: return new Float4x4(_m10, _m01, _m02, _m03, _m10, _m11, _m12, t, _m20, _m21, _m22, _m23, _m30, _m31, _m32, _m33);
				
				case 08: return new Float4x4(_m10, _m01, _m02, _m03, _m10, _m11, _m12, _m13, t, _m21, _m22, _m23, _m30, _m31, _m32, _m33);
				case 09: return new Float4x4(_m10, _m01, _m02, _m03, _m10, _m11, _m12, _m13, _m20, t, _m22, _m23, _m30, _m31, _m32, _m33);
				case 10: return new Float4x4(_m10, _m01, _m02, _m03, _m10, _m11, _m12, _m13, _m20, _m21, t, _m23, _m30, _m31, _m32, _m33);
				case 11: return new Float4x4(_m10, _m01, _m02, _m03, _m10, _m11, _m12, _m13, _m20, _m21, _m22, t, _m30, _m31, _m32, _m33);
					
				case 12: return new Float4x4(_m10, _m01, _m02, _m03, _m10, _m11, _m12, _m13, _m20, _m21, _m22, _m23, t, _m31, _m32, _m33);
				case 13: return new Float4x4(_m10, _m01, _m02, _m03, _m10, _m11, _m12, _m13, _m20, _m21, _m22, _m23, _m30, t, _m32, _m33);
				case 14: return new Float4x4(_m10, _m01, _m02, _m03, _m10, _m11, _m12, _m13, _m20, _m21, _m22, _m23, _m30, _m31, t, _m33);
				case 15: return new Float4x4(_m10, _m01, _m02, _m03, _m10, _m11, _m12, _m13, _m20, _m21, _m22, _m23, _m30, _m31, _m32, t);
			}

			return this;
		}
	}
}