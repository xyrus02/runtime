using System;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace XyrusWorx.Runtime.Graphics
{
	[PublicAPI]
	public static class Mathf
	{
		[StructLayout(LayoutKind.Explicit)]
		[SuppressMessage("ReSharper", "InconsistentNaming")]
		struct fiu
		{
			[FieldOffset(0)]
			public float f;

			[FieldOffset(0)]
			public int tmp;
		}

		private static float Exec(Func<double, double> f, float x) => (float)f(x);
		private static float Exec(Func<double, double, double> f, float x, float y) => (float)f(x, y);
		private static float Exec(Func<double, double, double, double> f, float x, float y, float z) => (float)f(x, y, z);

		private static Vector2 Exec(Func<double, double> f, Vector2 x) => new Vector2((float)f(x.X), (float)f(x.Y));
		private static Vector2 Exec(Func<double, double, double> f, Vector2 x, Vector2 y) => new Vector2((float)f(x.X, y.X), (float)f(x.Y, y.Y));
		private static Vector2 Exec(Func<double, double, double, double> f, Vector2 x, Vector2 y, Vector2 z) => new Vector2((float)f(x.X, y.X, z.X), (float)f(x.Y, y.Y, z.Y));

		private static Vector3 Exec(Func<double, double> f, Vector3 x) => new Vector3((float)f(x.X), (float)f(x.Y), (float)f(x.Z));
		private static Vector3 Exec(Func<double, double, double> f, Vector3 x, Vector3 y) => new Vector3((float)f(x.X, y.X), (float)f(x.Y, y.Y), (float)f(x.Z, y.Z));
		private static Vector3 Exec(Func<double, double, double, double> f, Vector3 x, Vector3 y, Vector3 z) => new Vector3((float)f(x.X, y.X, z.X), (float)f(x.Y, y.Y, z.Y), (float)f(x.Z, y.Z, z.Z));

		private static Vector4 Exec(Func<double, double> f, Vector4 x) => new Vector4((float)f(x.X), (float)f(x.Y), (float)f(x.Z), (float)f(x.W));
		private static Vector4 Exec(Func<double, double, double> f, Vector4 x, Vector4 y) => new Vector4((float)f(x.X, y.X), (float)f(x.Y, y.Y), (float)f(x.Z, y.Z), (float)f(x.W, y.W));
		private static Vector4 Exec(Func<double, double, double, double> f, Vector4 x, Vector4 y, Vector4 z) => new Vector4((float)f(x.X, y.X, z.X), (float)f(x.Y, y.Y, z.Y), (float)f(x.Z, y.Z, z.Z), (float)f(x.W, y.W, z.W));

		private static double ClampCore(double a, double b, double c) => a < b ? b : a > c ? c : a;
		private static double RoundCore(double a, double b) => Math.Round(a, (int)b);
		private static double Log2Core(double a) => Math.Log(a, 2);

		private static double SignCore(double a) => Math.Sign(a);

		public static float Acos(float x) => Exec(Math.Acos, x);
		public static float Asin(float x) => Exec(Math.Asin, x);
		public static float Atan(float x) => Exec(Math.Atan, x);

		public static Vector2 Acos(Vector2 x) => Exec(Math.Acos, x);
		public static Vector2 Asin(Vector2 x) => Exec(Math.Asin, x);
		public static Vector2 Atan(Vector2 x) => Exec(Math.Atan, x);

		public static Vector3 Acos(Vector3 x) => Exec(Math.Acos, x);
		public static Vector3 Asin(Vector3 x) => Exec(Math.Asin, x);
		public static Vector3 Atan(Vector3 x) => Exec(Math.Atan, x);

		public static Vector4 Acos(Vector4 x) => Exec(Math.Acos, x);
		public static Vector4 Asin(Vector4 x) => Exec(Math.Asin, x);
		public static Vector4 Atan(Vector4 x) => Exec(Math.Atan, x);

		public static float  Atan2(float y, float x) => Exec(Math.Atan2, y, x);
		public static Vector2 Atan2(Vector2 y, Vector2 x) => Exec(Math.Atan2, y, x);
		public static Vector3 Atan2(Vector3 y, Vector3 x) => Exec(Math.Atan2, y, x);
		public static Vector4 Atan2(Vector4 y, Vector4 x) => Exec(Math.Atan2, y, x);

		public static float Abs  (float x) => Exec(Math.Abs, x);
		public static float Ceil (float x) => Exec(Math.Ceiling, x);
		public static float Floor(float x) => Exec(Math.Floor, x);
		public static float Clamp(float x, float min, float max) => Exec(ClampCore, x, min, max);
		public static float Round(float x, float y) => Exec(RoundCore, x, y);
		public static float Trunc(float x) => (int)x;

		public static Vector2 Abs  (Vector2 x) => Exec(Math.Abs, x);
		public static Vector2 Ceil (Vector2 x) => Exec(Math.Ceiling, x);
		public static Vector2 Floor(Vector2 x) => Exec(Math.Floor, x);
		public static Vector2 Clamp(Vector2 x, Vector2 min, Vector2 max) => Exec(ClampCore, x, min, max);
		public static Vector2 Round(Vector2 x, Vector2 y) => Exec(RoundCore, x, y);
		public static Vector2 Trunc(Vector2 x) => new Vector2((int)x.X, (int)x.Y);

		public static Vector3 Abs  (Vector3 x) => Exec(Math.Abs, x);
		public static Vector3 Ceil (Vector3 x) => Exec(Math.Ceiling, x);
		public static Vector3 Floor(Vector3 x) => Exec(Math.Floor, x);
		public static Vector3 Clamp(Vector3 x, Vector3 min, Vector3 max) => Exec(ClampCore, x, min, max);
		public static Vector3 Round(Vector3 x, Vector3 y) => Exec(RoundCore, x, y);
		public static Vector3 Trunc(Vector3 x) => new Vector3((int)x.X, (int)x.Y, (int)x.Z);

		public static Vector4 Abs  (Vector4 x) => Exec(Math.Abs, x);
		public static Vector4 Ceil (Vector4 x) => Exec(Math.Ceiling, x);
		public static Vector4 Floor(Vector4 x) => Exec(Math.Floor, x);
		public static Vector4 Clamp(Vector4 x, Vector4 min, Vector4 max) => Exec(ClampCore, x, min, max);
		public static Vector4 Round(Vector4 x, Vector4 y) => Exec(RoundCore, x, y);
		public static Vector4 Trunc(Vector4 x) => new Vector4((int)x.X, (int)x.Y, (int)x.Z, (int)x.W);

		public static float Cos(float x) => Exec(Math.Cos, x);
		public static float Sin(float x) => Exec(Math.Sin, x);
		public static float Tan(float x) => Exec(Math.Tan, x);

		public static float Cosh(float x) => Exec(Math.Cosh, x);
		public static float Sinh(float x) => Exec(Math.Sinh, x);
		public static float Tanh(float x) => Exec(Math.Tanh, x);

		public static float Exp  (float x) => Exec(Math.Exp, x);
		public static float Log  (float x) => Exec(Math.Log, x);
		public static float Log2  (float x) =>Exec(Log2Core, x);
		public static float Log10(float x) => Exec(Math.Log10, x);

		public static Vector2 Cos(Vector2 x) => Exec(Math.Cos, x);
		public static Vector2 Sin(Vector2 x) => Exec(Math.Sin, x);
		public static Vector2 Tan(Vector2 x) => Exec(Math.Tan, x);

		public static Vector2 Cosh(Vector2 x) => Exec(Math.Cosh, x);
		public static Vector2 Sinh(Vector2 x) => Exec(Math.Sinh, x);
		public static Vector2 Tanh(Vector2 x) => Exec(Math.Tanh, x);

		public static Vector2 Exp  (Vector2 x) => Exec(Math.Exp, x);
		public static Vector2 Log  (Vector2 x) => Exec(Math.Log, x);
		public static Vector2 Log2 (Vector2 x) => Exec(Log2Core, x);
		public static Vector2 Log10(Vector2 x) => Exec(Math.Log10, x);

		public static Vector3 Cos(Vector3 x) => Exec(Math.Cos, x);
		public static Vector3 Sin(Vector3 x) => Exec(Math.Sin, x);
		public static Vector3 Tan(Vector3 x) => Exec(Math.Tan, x);

		public static Vector3 Cosh(Vector3 x) => Exec(Math.Cosh, x);
		public static Vector3 Sinh(Vector3 x) => Exec(Math.Sinh, x);
		public static Vector3 Tanh(Vector3 x) => Exec(Math.Tanh, x);

		public static Vector3 Exp  (Vector3 x) => Exec(Math.Exp, x);
		public static Vector3 Log  (Vector3 x) => Exec(Math.Log, x);
		public static Vector3 Log2 (Vector3 x) => Exec(Log2Core, x);
		public static Vector3 Log10(Vector3 x) => Exec(Math.Log10, x);

		public static Vector4 Cos(Vector4 x) => Exec(Math.Cos, x);
		public static Vector4 Sin(Vector4 x) => Exec(Math.Sin, x);
		public static Vector4 Tan(Vector4 x) => Exec(Math.Tan, x);

		public static Vector4 Cosh(Vector4 x) => Exec(Math.Cosh, x);
		public static Vector4 Sinh(Vector4 x) => Exec(Math.Sinh, x);
		public static Vector4 Tanh(Vector4 x) => Exec(Math.Tanh, x);

		public static Vector4 Exp  (Vector4 x) => Exec(Math.Exp, x);
		public static Vector4 Log  (Vector4 x) => Exec(Math.Log, x);
		public static Vector4 Log2 (Vector4 x) => Exec(Log2Core, x);
		public static Vector4 Log10(Vector4 x) => Exec(Math.Log10, x);

		public static float Min(float x, float y) => Exec(Math.Min, x, y);
		public static float Max(float x, float y) => Exec(Math.Max, x, y);

		public static float Pow (float x, float y) => Exec(Math.Pow, x, y);
		public static float Sign(float x) => Exec(SignCore, x);

		public static Vector2 Min(Vector2 x, Vector2 y) => Exec(Math.Min, x, y);
		public static Vector2 Max(Vector2 x, Vector2 y) => Exec(Math.Max, x, y);

		public static Vector2 Pow (Vector2 x, Vector2 y) => Exec(Math.Pow, x, y);
		public static Vector2 Sign(Vector2 x) => Exec(SignCore, x);

		public static Vector3 Min(Vector3 x, Vector3 y) => Exec(Math.Min, x, y);
		public static Vector3 Max(Vector3 x, Vector3 y) => Exec(Math.Max, x, y);

		public static Vector3 Pow (Vector3 x, Vector3 y) => Exec(Math.Pow, x, y);
		public static Vector3 Sign(Vector3 x) => Exec(SignCore, x);

		public static Vector4 Min(Vector4 x, Vector4 y) => Exec(Math.Min, x, y);
		public static Vector4 Max(Vector4 x, Vector4 y) => Exec(Math.Max, x, y);

		public static Vector4 Pow (Vector4 x, Vector4 y) => Exec(Math.Pow, x, y);
		public static Vector4 Sign(Vector4 x) => Exec(SignCore, x);

		public static float Sqrt (float x) => Exec(Math.Sqrt, x);
		public static float Rsqrt(float x)
		{
			// ReSharper disable once CompareOfFloatsByEqualityOperator
			if (x == 0)
			{
				return 0;
			}

			fiu union;
			union.tmp = 0;
			union.f = x;
			union.tmp -= 1 << 23; // x - 2 ^ m
			union.tmp >>= 1;      // x / 2
			union.tmp += 1 << 29; // x + ((b + 1) / 2) * 2 ^ m
			return union.f;
		}

		public static Vector2 Sqrt (Vector2 x) => Exec(Math.Sqrt, x);
		public static Vector2 Rsqrt(Vector2 x) => new Vector2(Rsqrt(x.X), Rsqrt(x.Y));

		public static Vector3 Sqrt (Vector3 x) => Exec(Math.Sqrt, x);
		public static Vector3 Rsqrt(Vector3 x) => new Vector3(Rsqrt(x.X), Rsqrt(x.Y), Rsqrt(x.Z));

		public static Vector4 Sqrt (Vector4 x) => Exec(Math.Sqrt, x);
		public static Vector4 Rsqrt(Vector4 x) => new Vector4(Rsqrt(x.X), Rsqrt(x.Y), Rsqrt(x.Z), Rsqrt(x.W));

		public static float Lerp(float x, float y, float s) => x + s*(y - x);

		public static float Dot(Vector2 x, Vector2 y) => x.X*y.X + x.Y*y.Y;
		public static float Dot(Vector3 x, Vector3 y) => x.X*y.X + x.Y*y.Y + x.Z*y.Z;
		public static float Dot(Vector4 x, Vector4 y) => x.X*y.X + x.Y*y.Y + x.Z*y.Z + x.W*y.W;

		public static Vector2 Lerp(Vector2 x, Vector2 y, float s) => new Vector2(
			Smoothstep(x.X, y.X, s),
			Smoothstep(x.Y, y.Y, s));

		public static Vector3 Lerp(Vector3 x, Vector3 y, float s) => new Vector3(
			Smoothstep(x.X, y.X, s),
			Smoothstep(x.Y, y.Y, s),
			Smoothstep(x.Z, y.Z, s));

		public static Vector4 Lerp(Vector4 x, Vector4 y, float s) => new Vector4(
			Smoothstep(x.X, y.X, s),
			Smoothstep(x.Y, y.Y, s),
			Smoothstep(x.Z, y.Z, s),
			Smoothstep(x.W, y.W, s));

		public static float Smoothstep(float edge0, float edge1, float x)
		{
			x = Clamp((x - edge0) / (edge1 - edge0), 0, 1);
			return x * x * (3 - 2 * x);
		}

		public static Vector2 Smoothstep(Vector2 edge0, Vector2 edge1, float x) => new Vector2(
			Smoothstep(edge0.X, edge1.X, x),
			Smoothstep(edge0.Y, edge1.Y, x));

		public static Vector3 Smoothstep(Vector3 edge0, Vector3 edge1, float x) => new Vector3(
			Smoothstep(edge0.X, edge1.X, x),
			Smoothstep(edge0.Y, edge1.Y, x),
			Smoothstep(edge0.Z, edge1.Z, x));

		public static Vector4 Smoothstep(Vector4 edge0, Vector4 edge1, float x) => new Vector4(
			Smoothstep(edge0.X, edge1.X, x),
			Smoothstep(edge0.Y, edge1.Y, x),
			Smoothstep(edge0.Z, edge1.Z, x),
			Smoothstep(edge0.W, edge1.W, x));
	}
}
