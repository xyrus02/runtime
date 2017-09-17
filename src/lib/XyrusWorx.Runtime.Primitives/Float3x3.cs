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
	public struct Float3x3 : IMatrixType, IMatrixType<float>, IEquatable<Float3x3>, IComparable<Float3x3>, IComparable
	{
		public float _m00, _m01, _m02, _m10, _m11, _m12, _m20, _m21, _m22;

		public Float3x3(
			Float3 col0 = default(Float3), 
			Float3 col1 = default(Float3), 
			Float3 col2 = default(Float3)) : this(
			col0.x, col0.y, col0.z, 
			col1.x, col1.y, col1.z, 
			col2.x, col2.y, col2.z) { }
		
		public Float3x3(
			float m00 = 0, float m01 = 0, float m02 = 0,
			float m10 = 0, float m11 = 0, float m12 = 0,
			float m20 = 0, float m21 = 0, float m22 = 0)
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

		public static Float3x3 operator +(Float3x3 a, Float3x3 b) => new Float3x3(
			a._m00 + b._m00, a._m01 + b._m01, a._m02 + b._m02,
			a._m10 + b._m10, a._m11 + b._m11, a._m12 + b._m12,
			a._m20 + b._m20, a._m21 + b._m21, a._m22 + b._m22);

		public static Float3x3 operator -(Float3x3 a, Float3x3 b) => new Float3x3(
			a._m00 - b._m00, a._m01 - b._m01, a._m02 - b._m02,
			a._m10 - b._m10, a._m11 - b._m11, a._m12 - b._m12,
			a._m20 - b._m20, a._m21 - b._m21, a._m22 - b._m22);

		public static Float3x3 operator *(Float3x3 a, Float3x3 b)
		{
			var c = new Float3x3();

			c._m00 = a._m00 * b._m00 + a._m01 * b._m10 + a._m02 * b._m20;
			c._m01 = a._m00 * b._m01 + a._m01 * b._m11 + a._m02 * b._m21;
			c._m02 = a._m00 * b._m02 + a._m01 * b._m12 + a._m02 * b._m22;

			c._m10 = a._m10 * b._m00 + a._m11 * b._m10 + a._m12 * b._m20;
			c._m11 = a._m10 * b._m01 + a._m11 * b._m11 + a._m12 * b._m21;
			c._m12 = a._m10 * b._m02 + a._m11 * b._m12 + a._m12 * b._m22;

			c._m20 = a._m20 * b._m00 + a._m21 * b._m10 + a._m22 * b._m20;
			c._m21 = a._m20 * b._m01 + a._m21 * b._m11 + a._m22 * b._m21;
			c._m22 = a._m20 * b._m02 + a._m21 * b._m12 + a._m22 * b._m22;

			return c;
		}
		public static Float3x3 operator *(Float3x3 a, float b) => new Float3x3(
			a._m00 * b, a._m01 * b, a._m02 * b,
			a._m10 * b, a._m11 * b, a._m12 * b,
			a._m20 * b, a._m21 * b, a._m22 * b);

		[NotNull]
		public float[] this[int i]
		{
			get
			{
				switch (i)
				{
					case 0:return new [] { _m00, _m01, _m02 };
					case 1:return new [] { _m10, _m11, _m12 };
					case 2:return new [] { _m20, _m21, _m22 };
				}

				throw new IndexOutOfRangeException();
			}
		}

		public static Float3x3 Zero
		{
			get => new Float3x3();
		}
		public static Float3x3 Identity
		{
			get => new Float3x3(
				1, 0, 0,
				0, 1, 0,
				0, 0, 1);
		}

		[Pure]
		public Float3x3 Transpose() => new Float3x3(
			new Float3(_m00, _m10, _m20),
			new Float3(_m01, _m11, _m21),
			new Float3(_m02, _m12, _m22));

		[Pure]
		public float Determinant() =>
			_m00 * _m11 * _m22 +
			_m01 * _m12 * _m20 +
			_m02 * _m10 * _m21 -
			_m02 * _m11 * _m20 -
			_m01 * _m10 * _m22 -
			_m00 * _m12 * _m21;
		
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
		public bool Equals(Float3x3 other) => 
			_m00 == other._m00 && _m01 == other._m01 && _m02 == other._m02 &&
			_m10 == other._m10 && _m11 == other._m11 && _m12 == other._m12 &&
			_m20 == other._m20 && _m21 == other._m21 && _m22 == other._m22;
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
			{
				return false;
			}
			
			return obj is Float3x3 && Equals((Float3x3)obj);
		}
		public override string ToString() => $"{_m00}, {_m01}, {_m02} | {_m10}, {_m11}, {_m12} | {_m20}, {_m21}, {_m22}";
		public int CompareTo(Float3x3 other)
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
			
			if (!(obj is Float3x3))
			{
				throw new ArgumentException($"Object must be of type {nameof(Float3x3)}");
			}
			
			return CompareTo((Float3x3)obj);
		}
		
		public IVectorType[] GetRows() => new IVectorType[]
		{
			new Float3(_m00, _m10, _m20),
			new Float3(_m01, _m11, _m21),
			new Float3(_m02, _m12, _m22)
		};
		
		public IVectorType[] GetColumns()=> new IVectorType[]
		{
			new Float3(_m00, _m01, _m02),
			new Float3(_m10, _m11, _m12),
			new Float3(_m20, _m21, _m22)
		};
		
		IVectorType<float>[] IMatrixType<float>.GetColumns() => new IVectorType<float>[]
		{
			new Float3(_m00, _m10, _m20),
			new Float3(_m01, _m11, _m21),
			new Float3(_m02, _m12, _m22)
		};
		
		IVectorType<float>[] IMatrixType<float>.GetRows() => new IVectorType<float>[]
		{
			new Float3(_m00, _m01, _m02),
			new Float3(_m10, _m11, _m12),
			new Float3(_m20, _m21, _m22)
		};
		
		Type IMatrixType.ComponentType
		{
			get => typeof(float);
		}
	}
}