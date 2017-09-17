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
	[DebuggerDisplay("{_m00}, {_m10}, | {_m01}, {_m11}, | {_m20} {_m21}")]
	public struct Float3x2 : IMatrix, IMatrix<float>, IEquatable<Float3x2>, IComparable<Float3x2>, IComparable
	{
		public float _m00, _m01, _m10, _m11, _m20, _m21;

		public Float3x2(
			Float2 col0 = default(Float2), 
			Float2 col1 = default(Float2), 
			Float2 col2 = default(Float2)) : this(
			col0.x, col0.y, 
			col1.x, col1.y, 
			col2.x, col2.y) { }
		
		public Float3x2(
			Float3 row0 = default(Float3), 
			Float3 row1 = default(Float3)) : this(
			row0.x, row1.x, 
			row0.y, row1.y, 
			row0.z, row1.z) { }
		
		public Float3x2(
			float m00 = 0, float m01 = 0, 
			float m10 = 0, float m11 = 0,
			float m20 = 0, float m21 = 0)
		{
			_m00 = m00;
			_m01 = m01;
			_m10 = m10;
			_m11 = m11;
			_m20 = m20;
			_m21 = m21;
		}

		public static Float3x3 operator +(Float3x2 a, Float3x2 b) => new Float3x3(
			a._m00 + b._m00, a._m01 + b._m01,
			a._m10 + b._m10, a._m11 + b._m11,
			a._m20 + b._m20, a._m21 + b._m21);
		
		public static Float3x3 operator -(Float3x2 a, Float3x2 b) => new Float3x3(
			a._m00 - b._m00, a._m01 - b._m01,
			a._m10 - b._m10, a._m11 - b._m11,
			a._m20 - b._m20, a._m21 - b._m21);
		
		public static Float3x3 operator *(Float3x2 a, float b) => new Float3x3(
			a._m00 + b, a._m01 + b,
			a._m10 + b, a._m11 + b,
			a._m20 + b, a._m21 + b);
		
		public static Float2 operator *(Float3x2 a, Float2 b) => new Float2(
			a._m00 * b.x + a._m10 * b.y + a._m20,
			a._m01 * b.x + a._m11 * b.y + a._m21);

		[NotNull]
		public IVector<float> this[int i]
		{
			get
			{
				switch (i)
				{
					case 0: return new Float2( _m00, _m01 );
					case 1: return new Float2( _m10, _m11 );
					case 2: return new Float2( _m20, _m21 );
				}

				throw new IndexOutOfRangeException();
			}
		}

		public static Float3x2 Zero
		{
			get => new Float3x2();
		}
		public static Float3x2 Identity
		{
			get => new Float3x2(1, 0, 0, 1, 0, 0);
		}
		
		[SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = _m00.GetHashCode();
				hashCode = (hashCode * 397) ^ _m01.GetHashCode();
				hashCode = (hashCode * 397) ^ _m10.GetHashCode();
				hashCode = (hashCode * 397) ^ _m11.GetHashCode();
				hashCode = (hashCode * 397) ^ _m20.GetHashCode();
				hashCode = (hashCode * 397) ^ _m21.GetHashCode();
				return hashCode;
			}
		}
		public bool Equals(Float3x2 other) => 
			_m00 == other._m00 && _m01 == other._m01 && 
			_m10 == other._m10 && _m11 == other._m11 &&
			_m20 == other._m20 && _m21 == other._m21;
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
			{
				return false;
			}
			
			return obj is Float3x2 && Equals((Float3x2)obj);
		}
		public override string ToString() => $"{_m00}, {_m10}, | {_m01}, {_m11}, | {_m20} {_m21}";
		public int CompareTo(Float3x2 other)
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
			
			var m11Comparison = _m11.CompareTo(other._m11);
			if (m11Comparison != 0)
			{
				return m11Comparison;
			}
			
			var m20Comparison = _m20.CompareTo(other._m20);
			if (m20Comparison != 0)
			{
				return m20Comparison;
			}
			
			return _m21.CompareTo(other._m21);
		}
		public int CompareTo(object obj)
		{
			if (ReferenceEquals(null, obj))
			{
				return 1;
			}
			
			if (!(obj is Float3x2))
			{
				throw new ArgumentException($"Object must be of type {nameof(Float3x2)}");
			}
			
			return CompareTo((Float3x2)obj);
		}
		
		public IVector[] GetRows() => new IVector[]
		{
			new Float3(_m00, _m10, _m20),
			new Float3(_m01, _m11, _m21)
		};
		
		public IVector[] GetColumns()=> new IVector[]
		{
			new Float2(_m00, _m01),
			new Float2(_m10, _m11),
			new Float2(_m20, _m21)
		};
		
		IVector<float>[] IMatrix<float>.GetColumns() => new IVector<float>[]
		{
			new Float3(_m00, _m10, _m20),
			new Float3(_m01, _m11, _m21)
		};
		
		IVector<float>[] IMatrix<float>.GetRows() => new IVector<float>[]
		{
			new Float2(_m00, _m01),
			new Float2(_m10, _m11),
			new Float2(_m20, _m21)
		};
		
		Type IMatrix.ComponentType
		{
			get => typeof(float);
		}
	}
}