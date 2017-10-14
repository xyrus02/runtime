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
	public struct Float2x2 : IMatrix, IMatrix<float>, IEquatable<Float2x2>, IComparable<Float2x2>, IComparable, IMatrixCellWriter
	{
		public float _m00, _m01, _m10, _m11;

		public Float2x2(
			Float2 col0 = default(Float2), 
			Float2 col1 = default(Float2)) : this(
			col0.x, col0.y, 
			col1.x, col1.y) { }
		
		public Float2x2(
			float m00 = 0, float m01 = 0,
			float m10 = 0, float m11 = 0)
		{
			_m00 = m00;
			_m01 = m01;
			_m10 = m10;
			_m11 = m11;
		}

		public static Float2x2 operator +(Float2x2 a, Float2x2 b) => new Float2x2(
			a._m00 + b._m00, a._m01 + b._m01,
			a._m10 + b._m10, a._m11 + b._m11);

		public static Float2x2 operator -(Float2x2 a, Float2x2 b) => new Float2x2(
			a._m00 - b._m00, a._m01 - b._m01,
			a._m10 - b._m10, a._m11 - b._m11);

		public static Float2x2 operator *(Float2x2 a, Float2x2 b)
		{
			var c = new Float2x2();

			c._m00 = a._m00 * b._m00 + a._m01 * b._m10;
			c._m01 = a._m00 * b._m01 + a._m01 * b._m11;

			c._m10 = a._m10 * b._m00 + a._m11 * b._m10;
			c._m11 = a._m10 * b._m01 + a._m11 * b._m11;

			return c;
		}
		public static Float2x2 operator *(Float2x2 a, float b) => new Float2x2(
			a._m00 * b, a._m01 * b,
			a._m10 * b, a._m11 * b);

		[NotNull]
		public IVector<float> this[int i]
		{
			get
			{
				switch (i)
				{
					case 0: return new Float2(_m00, _m01);
					case 1: return new Float2(_m10, _m11);
				}

				throw new IndexOutOfRangeException();
			}
		}

		public static Float2x2 Zero
		{
			get => new Float2x2();
		}
		public static Float2x2 Identity
		{
			get => new Float2x2(
				1, 0,
				0, 1);
		}

		[Pure]
		public Float2x2 Transpose() => new Float2x2(
			new Float2(_m00, _m10),
			new Float2(_m01, _m11));

		[Pure]
		public float Determinant() =>
			_m00 * _m11 -
			_m01 * _m10;
		
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
		public bool Equals(Float2x2 other) => 
			_m00 == other._m00 && _m01 == other._m01 &&
			_m10 == other._m10 && _m11 == other._m11;
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
			{
				return false;
			}
			
			return obj is Float2x2 && Equals((Float2x2)obj);
		}
		public override string ToString() => $"{_m00}, {_m01} | {_m10}, {_m11}";
		public int CompareTo(Float2x2 other)
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
			
			if (!(obj is Float2x2))
			{
				throw new ArgumentException($"Object must be of type {nameof(Float2x2)}");
			}
			
			return CompareTo((Float2x2)obj);
		}
		
		public IVector[] GetRows() => new IVector[]
		{
			new Float2(_m00, _m10),
			new Float2(_m01, _m11)
		};
		
		public IVector[] GetColumns()=> new IVector[]
		{
			new Float2(_m00, _m01),
			new Float2(_m10, _m11)
		};
		
		IVector<float>[] IMatrix<float>.GetColumns() => new IVector<float>[]
		{
			new Float2(_m00, _m10),
			new Float2(_m01, _m11)
		};
		
		IVector<float>[] IMatrix<float>.GetRows() => new IVector<float>[]
		{
			new Float2(_m00, _m01),
			new Float2(_m10, _m11)
		};
		
		Type IMatrix.ComponentType
		{
			get => typeof(float);
		}
		
		IMatrix IMatrixCellWriter.Set(int column, int row, object value)
		{
			var t = (float)value;

			switch (column * 2 + row)
			{
				case 0: return new Float2x2(t, _m01, _m10, _m11);
				case 1: return new Float2x2(_m00, t, _m10, _m11);
				case 2: return new Float2x2(_m10, _m01, t, _m11);
				case 3: return new Float2x2(_m10, _m01, _m10, t);
			}

			return this;
		}
	}
}