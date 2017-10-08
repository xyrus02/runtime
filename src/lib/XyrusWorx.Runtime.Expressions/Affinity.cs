using System;
using System.Diagnostics;
using JetBrains.Annotations;

namespace XyrusWorx.Runtime.Expressions 
{
	[PublicAPI]
	[DebuggerDisplay("{Size}")]
	public sealed class Affinity
	{
		private readonly int mRows, mColumns;
		private const int mMaxAffinity = 4;
		
		private Affinity(int rows, int columns)
		{
			if (rows <= 0 || rows > mMaxAffinity)
			{
				throw new ArgumentOutOfRangeException(nameof(rows), $"The maximum row count of a vector or matrix is {mMaxAffinity}");
			}
			
			if (columns <= 0 || columns > mMaxAffinity)
			{
				throw new ArgumentOutOfRangeException(nameof(columns), $"The maximum column count of a matrix is {mMaxAffinity}");
			}

			if (columns > 1 && rows != columns)
			{
				throw new ArgumentException("Only square matrices are supported.", nameof(columns));
			}

			mRows = rows;
			mColumns = columns;
		}

		[NotNull]
		public static Affinity Single() => new Affinity(1, 1);
		
		[NotNull]
		public static Affinity Vector(int size)
		{
			if (size <= 1)
			{
				throw new ArgumentOutOfRangeException(nameof(size));
			}
			
			return new Affinity(size, 1);
		}
		
		[NotNull]
		public static Affinity Matrix(int size)
		{
			if (size <= 1)
			{
				throw new ArgumentOutOfRangeException(nameof(size));
			}
			
			return new Affinity(size, size);
		}
		
		public Int2 Size => new Int2(mRows, mColumns);
		public int Count => mRows * mColumns;
		
		[PublicAPI]
		public Type ExpandType([NotNull] Type elementType)
		{
			if (elementType == null)
			{
				throw new ArgumentNullException(nameof(elementType));
			}
			
			if (mRows == 1 && mColumns == 1)
			{
				return elementType;
			}

			if (mColumns == 1)
			{
				if (elementType == typeof(float))
				{
					var vectors = new[]
					{
						typeof(Float2),
						typeof(Float3),
						typeof(Float4)
					};

					return vectors[mRows - 2];
				}
				
				if (elementType == typeof(int))
				{
					var vectors = new[]
					{
						typeof(Int2),
						typeof(Int3),
						typeof(Int4)
					};

					return vectors[mRows - 2];
				}
				else
				{
					var vectors = new[]
					{
						typeof(Vector2<>),
						typeof(Vector3<>),
						typeof(Vector4<>)
					};

					return vectors[mRows - 2].MakeGenericType(elementType);
				}
			}

			if (elementType == typeof(float))
			{
				var matrices = new[]
				{
					typeof(Float2x2),
					typeof(Float3x3),
					typeof(Float4x4)
				};

				return matrices[mColumns - 2];
			}
			else
			{
				var matrices = new[]
				{
					typeof(Matrix2x2<>),
					typeof(Matrix3x3<>),
					typeof(Matrix4x4<>)
				};

				return matrices[mColumns - 2].MakeGenericType(elementType);
			}
		}
	}
}