using System;
using JetBrains.Annotations;
using XyrusWorx.Runtime.Native;

namespace XyrusWorx.Runtime
{
	[PublicAPI]
	public static class Mathf
	{
		// ReSharper disable once InconsistentNaming
		public const float PI = 3.14159265358979f;
		public const float E = 2.71828182845905f;
		public const float Epsilon = float.Epsilon;

		private static int Exec(Func<int, int> f, int x) => f(x);
		private static int Exec(Func<int, int, int> f, int x, int y) => f(x, y);
		private static int Exec(Func<int, int, int, int> f, int x, int y, int z) => f(x, y, z);

		private static Int2 Exec(Func<int, int> f, Int2 x) => new Int2(f(x.x), f(x.y));
		private static Int2 Exec(Func<int, int, int> f, Int2 x, Int2 y) => new Int2(f(x.x, y.x), f(x.y, y.y));
		private static Int2 Exec(Func<int, int, int, int> f, Int2 x, Int2 y, Int2 z) => new Int2(f(x.x, y.x, z.x), f(x.y, y.y, z.y));

		private static Int3 Exec(Func<int, int> f, Int3 x) => new Int3(f(x.x), f(x.y), f(x.z));
		private static Int3 Exec(Func<int, int, int> f, Int3 x, Int3 y) => new Int3(f(x.x, y.x), f(x.y, y.y), f(x.z, y.z));
		private static Int3 Exec(Func<int, int, int, int> f, Int3 x, Int3 y, Int3 z) => new Int3(f(x.x, y.x, z.x), f(x.y, y.y, z.y), f(x.z, y.z, z.z));

		private static Int4 Exec(Func<int, int> f, Int4 x) => new Int4(f(x.x), f(x.y), f(x.z), f(x.w));
		private static Int4 Exec(Func<int, int, int> f, Int4 x, Int4 y) => new Int4(f(x.x, y.x), f(x.y, y.y), f(x.z, y.z), f(x.w, y.w));
		private static Int4 Exec(Func<int, int, int, int> f, Int4 x, Int4 y, Int4 z) => new Int4(f(x.x, y.x, z.x), f(x.y, y.y, z.y), f(x.z, y.z, z.z), f(x.w, y.w, z.w));
		
		private static float Exec(Func<double, double> f, float x) => (float)f(x);
		private static float Exec(Func<double, double, double> f, float x, float y) => (float)f(x, y);
		private static float Exec(Func<double, double, double, double> f, float x, float y, float z) => (float)f(x, y, z);

		private static Float2 Exec(Func<double, double> f, Float2 x) => new Float2((float)f(x.x), (float)f(x.y));
		private static Float2 Exec(Func<double, double, double> f, Float2 x, Float2 y) => new Float2((float)f(x.x, y.x), (float)f(x.y, y.y));
		private static Float2 Exec(Func<double, double, double, double> f, Float2 x, Float2 y, Float2 z) => new Float2((float)f(x.x, y.x, z.x), (float)f(x.y, y.y, z.y));

		private static Float3 Exec(Func<double, double> f, Float3 x) => new Float3((float)f(x.x), (float)f(x.y), (float)f(x.z));
		private static Float3 Exec(Func<double, double, double> f, Float3 x, Float3 y) => new Float3((float)f(x.x, y.x), (float)f(x.y, y.y), (float)f(x.z, y.z));
		private static Float3 Exec(Func<double, double, double, double> f, Float3 x, Float3 y, Float3 z) => new Float3((float)f(x.x, y.x, z.x), (float)f(x.y, y.y, z.y), (float)f(x.z, y.z, z.z));

		private static Float4 Exec(Func<double, double> f, Float4 x) => new Float4((float)f(x.x), (float)f(x.y), (float)f(x.z), (float)f(x.w));
		private static Float4 Exec(Func<double, double, double> f, Float4 x, Float4 y) => new Float4((float)f(x.x, y.x), (float)f(x.y, y.y), (float)f(x.z, y.z), (float)f(x.w, y.w));
		private static Float4 Exec(Func<double, double, double, double> f, Float4 x, Float4 y, Float4 z) => new Float4((float)f(x.x, y.x, z.x), (float)f(x.y, y.y, z.y), (float)f(x.z, y.z, z.z), (float)f(x.w, y.w, z.w));

		private static double ClampCore(double a, double b, double c) => a < b ? b : a > c ? c : a;
		private static double RoundCore(double a, double b) => Math.Round(a, (int)b);
		private static double Log2Core(double a) => Math.Log(a, 2);

		private static int SignCore(int a) => Math.Sign(a);
		private static double SignCore(double a) => Math.Sign(a);

		public static float Acos(this float x) => Exec(Math.Acos, x);
		public static float Asin(this float x) => Exec(Math.Asin, x);
		public static float Atan(this float x) => Exec(Math.Atan, x);

		public static Float2 Acos(this Float2 x) => Exec(Math.Acos, x);
		public static Float2 Asin(this Float2 x) => Exec(Math.Asin, x);
		public static Float2 Atan(this Float2 x) => Exec(Math.Atan, x);

		public static Float3 Acos(this Float3 x) => Exec(Math.Acos, x);
		public static Float3 Asin(this Float3 x) => Exec(Math.Asin, x);
		public static Float3 Atan(this Float3 x) => Exec(Math.Atan, x);

		public static Float4 Acos(this Float4 x) => Exec(Math.Acos, x);
		public static Float4 Asin(this Float4 x) => Exec(Math.Asin, x);
		public static Float4 Atan(this Float4 x) => Exec(Math.Atan, x);

		public static float  Atan2(float y, float x) => Exec(Math.Atan2, y, x);
		public static Float2 Atan2(Float2 y, Float2 x) => Exec(Math.Atan2, y, x);
		public static Float3 Atan2(Float3 y, Float3 x) => Exec(Math.Atan2, y, x);
		public static Float4 Atan2(Float4 y, Float4 x) => Exec(Math.Atan2, y, x);

		public static int   Abs  (this int x)   => Exec(Math.Abs, x);
		public static float Abs  (this float x) => Exec(Math.Abs, x);
		public static float Ceil (this float x) => Exec(Math.Ceiling, x);
		public static float Floor(this float x) => Exec(Math.Floor, x);
		public static float Clamp(this float x, float min, float max) => Exec(ClampCore, x, min, max);
		public static float Round(this float x, float digits) => Exec(RoundCore, x, digits);
		public static float Trunc(this float x) => (int)x;

		public static Int2   Abs  (this Int2 x)   => Exec(Math.Abs, x);
		public static Float2 Abs  (this Float2 x) => Exec(Math.Abs, x);
		public static Float2 Ceil (this Float2 x) => Exec(Math.Ceiling, x);
		public static Float2 Floor(this Float2 x) => Exec(Math.Floor, x);
		public static Float2 Clamp(this Float2 x, Float2 min, Float2 max) => Exec(ClampCore, x, min, max);
		public static Float2 Round(this Float2 x, Float2 digits) => Exec(RoundCore, x, digits);
		public static Float2 Trunc(this Float2 x) => new Float2((int)x.x, (int)x.y);

		public static Int3   Abs  (this Int3 x)   => Exec(Math.Abs, x);
		public static Float3 Abs  (this Float3 x) => Exec(Math.Abs, x);
		public static Float3 Ceil (this Float3 x) => Exec(Math.Ceiling, x);
		public static Float3 Floor(this Float3 x) => Exec(Math.Floor, x);
		public static Float3 Clamp(this Float3 x, Float3 min, Float3 max) => Exec(ClampCore, x, min, max);
		public static Float3 Round(this Float3 x, Float3 digits) => Exec(RoundCore, x, digits);
		public static Float3 Trunc(this Float3 x) => new Float3((int)x.x, (int)x.y, (int)x.z);

		public static Int4   Abs  (this Int4 x)   => Exec(Math.Abs, x);
		public static Float4 Abs  (this Float4 x) => Exec(Math.Abs, x);
		public static Float4 Ceil (this Float4 x) => Exec(Math.Ceiling, x);
		public static Float4 Floor(this Float4 x) => Exec(Math.Floor, x);
		public static Float4 Clamp(this Float4 x, Float4 min, Float4 max) => Exec(ClampCore, x, min, max);
		public static Float4 Round(this Float4 x, Float4 digits) => Exec(RoundCore, x, digits);
		public static Float4 Trunc(this Float4 x) => new Float4((int)x.x, (int)x.y, (int)x.z, (int)x.w);

		public static float Cos(this float x) => Exec(Math.Cos, x);
		public static float Sin(this float x) => Exec(Math.Sin, x);
		public static float Tan(this float x) => Exec(Math.Tan, x);

		public static float Cosh(this float x) => Exec(Math.Cosh, x);
		public static float Sinh(this float x) => Exec(Math.Sinh, x);
		public static float Tanh(this float x) => Exec(Math.Tanh, x);

		public static float Exp  (this float x) => Exec(Math.Exp, x);
		public static float Log  (this float x) => Exec(Math.Log, x);
		public static float Log2 (this float x) =>Exec(Log2Core, x);
		public static float Log10(this float x) => Exec(Math.Log10, x);

		public static Float2 Cos(this Float2 x) => Exec(Math.Cos, x);
		public static Float2 Sin(this Float2 x) => Exec(Math.Sin, x);
		public static Float2 Tan(this Float2 x) => Exec(Math.Tan, x);

		public static Float2 Cosh(this Float2 x) => Exec(Math.Cosh, x);
		public static Float2 Sinh(this Float2 x) => Exec(Math.Sinh, x);
		public static Float2 Tanh(this Float2 x) => Exec(Math.Tanh, x);

		public static Float2 Exp  (this Float2 x) => Exec(Math.Exp, x);
		public static Float2 Log  (this Float2 x) => Exec(Math.Log, x);
		public static Float2 Log2 (this Float2 x) => Exec(Log2Core, x);
		public static Float2 Log10(this Float2 x) => Exec(Math.Log10, x);

		public static Float3 Cos(this Float3 x) => Exec(Math.Cos, x);
		public static Float3 Sin(this Float3 x) => Exec(Math.Sin, x);
		public static Float3 Tan(this Float3 x) => Exec(Math.Tan, x);

		public static Float3 Cosh(this Float3 x) => Exec(Math.Cosh, x);
		public static Float3 Sinh(this Float3 x) => Exec(Math.Sinh, x);
		public static Float3 Tanh(this Float3 x) => Exec(Math.Tanh, x);

		public static Float3 Exp  (this Float3 x) => Exec(Math.Exp, x);
		public static Float3 Log  (this Float3 x) => Exec(Math.Log, x);
		public static Float3 Log2 (this Float3 x) => Exec(Log2Core, x);
		public static Float3 Log10(this Float3 x) => Exec(Math.Log10, x);

		public static Float4 Cos(this Float4 x) => Exec(Math.Cos, x);
		public static Float4 Sin(this Float4 x) => Exec(Math.Sin, x);
		public static Float4 Tan(this Float4 x) => Exec(Math.Tan, x);

		public static Float4 Cosh(this Float4 x) => Exec(Math.Cosh, x);
		public static Float4 Sinh(this Float4 x) => Exec(Math.Sinh, x);
		public static Float4 Tanh(this Float4 x) => Exec(Math.Tanh, x);

		public static Float4 Exp  (this Float4 x) => Exec(Math.Exp, x);
		public static Float4 Log  (this Float4 x) => Exec(Math.Log, x);
		public static Float4 Log2 (this Float4 x) => Exec(Log2Core, x);
		public static Float4 Log10(this Float4 x) => Exec(Math.Log10, x);

		public static int Min(int x, int y) => Exec(Math.Min, x, y);
		public static int Max(int x, int y) => Exec(Math.Max, x, y);
		public static int Sign(int x) => Exec(SignCore, x);
		
		public static float Min(float x, float y) => Exec(Math.Min, x, y);
		public static float Max(float x, float y) => Exec(Math.Max, x, y);

		public static float Pow (this float x, float power) => Exec(Math.Pow, x, power);
		public static float Sign(this float x) => Exec(SignCore, x);

		public static Int2 Min(Int2 x, Int2 y) => Exec(Math.Min, x, y);
		public static Int2 Max(Int2 x, Int2 y) => Exec(Math.Max, x, y);
		public static Int2 Sign(this Int2 x) => Exec(SignCore, x);
		
		public static Float2 Min(Float2 x, Float2 y) => Exec(Math.Min, x, y);
		public static Float2 Max(Float2 x, Float2 y) => Exec(Math.Max, x, y);
		
		public static Float2 Pow (this Float2 x, Float2 power) => Exec(Math.Pow, x, power);
		public static Float2 Sign(this Float2 x) => Exec(SignCore, x);
		
		public static Int3 Min(Int3 x, Int3 y) => Exec(Math.Min, x, y);
		public static Int3 Max(Int3 x, Int3 y) => Exec(Math.Max, x, y);
		public static Int3 Sign(this Int3 x) => Exec(SignCore, x);

		public static Float3 Min(Float3 x, Float3 y) => Exec(Math.Min, x, y);
		public static Float3 Max(Float3 x, Float3 y) => Exec(Math.Max, x, y);

		public static Float3 Pow (this Float3 x, Float3 power) => Exec(Math.Pow, x, power);
		public static Float3 Sign(this Float3 x) => Exec(SignCore, x);
		
		public static Int4 Min(Int4 x, Int4 y) => Exec(Math.Min, x, y);
		public static Int4 Max(Int4 x, Int4 y) => Exec(Math.Max, x, y);
		public static Int4 Sign(this Int4 x) => Exec(SignCore, x);

		public static Float4 Min(Float4 x, Float4 y) => Exec(Math.Min, x, y);
		public static Float4 Max(Float4 x, Float4 y) => Exec(Math.Max, x, y);

		public static Float4 Pow (this Float4 x, Float4 power) => Exec(Math.Pow, x, power);
		public static Float4 Sign(this Float4 x) => Exec(SignCore, x);

		public static float Sqrt (this float x) => Exec(Math.Sqrt, x);
		public static float Rsqrt(this float x)
		{
			// ReSharper disable once CompareOfFloatsByEqualityOperator
			if (x == 0)
			{
				return 0;
			}

			FloatOrInteger union;
			union.i = 0;
			union.f = x;
			union.i -= 1 << 23; // x - 2 ^ m
			union.i >>= 1;      // x / 2
			union.i += 1 << 29; // x + ((b + 1) / 2) * 2 ^ m
			return union.f;
		}

		public static Float2 Sqrt (this Float2 x) => Exec(Math.Sqrt, x);
		public static Float2 Rsqrt(this Float2 x) => new Float2(Rsqrt(x.x), Rsqrt(x.y));

		public static Float3 Sqrt (this Float3 x) => Exec(Math.Sqrt, x);
		public static Float3 Rsqrt(this Float3 x) => new Float3(Rsqrt(x.x), Rsqrt(x.y), Rsqrt(x.z));

		public static Float4 Sqrt (this Float4 x) => Exec(Math.Sqrt, x);
		public static Float4 Rsqrt(this Float4 x) => new Float4(Rsqrt(x.x), Rsqrt(x.y), Rsqrt(x.z), Rsqrt(x.w));

		public static float Lerp(float x, float y, float s) => x + s*(y - x);

		public static float Dot(this Float2 x, Float2 other) => x.x*other.x + x.y*other.y;
		public static float Dot(this Float3 x, Float3 other) => x.x*other.x + x.y*other.y + x.z*other.z;
		public static float Dot(this Float4 x, Float4 other) => x.x*other.x + x.y*other.y + x.z*other.z + x.w*other.w;

		public static Float2 Lerp(Float2 x, Float2 y, float s) => new Float2(
			Lerp(x.x, y.x, s),
			Lerp(x.y, y.y, s));

		public static Float3 Lerp(Float3 x, Float3 y, float s) => new Float3(
			Lerp(x.x, y.x, s),
			Lerp(x.y, y.y, s),
			Lerp(x.z, y.z, s));

		public static Float4 Lerp(Float4 x, Float4 y, float s) => new Float4(
			Lerp(x.x, y.x, s),
			Lerp(x.y, y.y, s),
			Lerp(x.z, y.z, s),
			Lerp(x.w, y.w, s));

		public static float Smoothstep(float edge0, float edge1, float x)
		{
			x = Clamp((x - edge0) / (edge1 - edge0), 0, 1);
			return x * x * (3 - 2 * x);
		}

		public static Float2 Smoothstep(Float2 edge0, Float2 edge1, float x) => new Float2(
			Smoothstep(edge0.x, edge1.x, x),
			Smoothstep(edge0.y, edge1.y, x));

		public static Float3 Smoothstep(Float3 edge0, Float3 edge1, float x) => new Float3(
			Smoothstep(edge0.x, edge1.x, x),
			Smoothstep(edge0.y, edge1.y, x),
			Smoothstep(edge0.z, edge1.z, x));

		public static Float4 Smoothstep(Float4 edge0, Float4 edge1, float x) => new Float4(
			Smoothstep(edge0.x, edge1.x, x),
			Smoothstep(edge0.y, edge1.y, x),
			Smoothstep(edge0.z, edge1.z, x),
			Smoothstep(edge0.w, edge1.w, x));
	}
}
