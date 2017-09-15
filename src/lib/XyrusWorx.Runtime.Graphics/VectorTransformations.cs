using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using JetBrains.Annotations;

namespace XyrusWorx.Runtime.Graphics 
{
	[PublicAPI]
	[SuppressMessage("ReSharper", "InconsistentNaming")]
	public static class VectorTransformations
	{
		public static Vector2 yx (this Vector2 v) => new Vector2(v.Y, v.X);
		
		public static Vector2 xy (this Vector3 v) => new Vector2(v.X, v.Y);
		public static Vector2 xz (this Vector3 v) => new Vector2(v.X, v.Z);
		public static Vector2 yx (this Vector3 v) => new Vector2(v.Y, v.X);
		public static Vector2 yz (this Vector3 v) => new Vector2(v.Y, v.Z);
		public static Vector2 zx (this Vector3 v) => new Vector2(v.Z, v.X);
		public static Vector2 zy (this Vector3 v) => new Vector2(v.Z, v.Y);
		
		public static Vector2 xy (this Vector4 v) => new Vector2(v.X, v.Y);
		public static Vector2 xz (this Vector4 v) => new Vector2(v.X, v.Z);
		public static Vector2 xw (this Vector4 v) => new Vector2(v.X, v.W);
		public static Vector2 yx (this Vector4 v) => new Vector2(v.Y, v.X);
		public static Vector2 yz (this Vector4 v) => new Vector2(v.Y, v.Z);
		public static Vector2 yw (this Vector4 v) => new Vector2(v.Y, v.W);
		public static Vector2 zx (this Vector4 v) => new Vector2(v.Z, v.X);
		public static Vector2 zy (this Vector4 v) => new Vector2(v.Z, v.Y);
		public static Vector2 zw (this Vector4 v) => new Vector2(v.Z, v.W);
		public static Vector2 wx (this Vector4 v) => new Vector2(v.W, v.X);
		public static Vector2 wy (this Vector4 v) => new Vector2(v.W, v.Y);
		public static Vector2 wz (this Vector4 v) => new Vector2(v.W, v.Z);
		
		public static Vector3 xzy (this Vector3 v) => new Vector3(v.X, v.Z, v.Y);
		public static Vector3 yxz (this Vector3 v) => new Vector3(v.Y, v.X, v.Z);
		public static Vector3 yzx (this Vector3 v) => new Vector3(v.Y, v.Z, v.X);
		public static Vector3 zxy (this Vector3 v) => new Vector3(v.Z, v.X, v.Y);
		public static Vector3 zyx (this Vector3 v) => new Vector3(v.Z, v.Y, v.X);

		public static Vector3 xyz (this Vector4 v) => new Vector3(v.X, v.Y, v.Z);
		public static Vector3 xyw (this Vector4 v) => new Vector3(v.X, v.Y, v.W);
		public static Vector3 xzy (this Vector4 v) => new Vector3(v.X, v.Z, v.Y);
		public static Vector3 xzw (this Vector4 v) => new Vector3(v.X, v.Z, v.W);
		public static Vector3 xwy (this Vector4 v) => new Vector3(v.X, v.W, v.Y);
		public static Vector3 xwz (this Vector4 v) => new Vector3(v.X, v.W, v.Z);
		public static Vector3 yxz (this Vector4 v) => new Vector3(v.Y, v.X, v.Z);
		public static Vector3 yxw (this Vector4 v) => new Vector3(v.Y, v.X, v.W);
		public static Vector3 yzx (this Vector4 v) => new Vector3(v.Y, v.Z, v.X);
		public static Vector3 yzw (this Vector4 v) => new Vector3(v.Y, v.Z, v.W);
		public static Vector3 ywx (this Vector4 v) => new Vector3(v.Y, v.W, v.X);
		public static Vector3 ywz (this Vector4 v) => new Vector3(v.Y, v.W, v.Z);
		public static Vector3 zxy (this Vector4 v) => new Vector3(v.Z, v.X, v.Y);
		public static Vector3 zxw (this Vector4 v) => new Vector3(v.Z, v.X, v.W);
		public static Vector3 zyx (this Vector4 v) => new Vector3(v.Z, v.Y, v.X);
		public static Vector3 zyw (this Vector4 v) => new Vector3(v.Z, v.Y, v.W);
		public static Vector3 zwx (this Vector4 v) => new Vector3(v.Z, v.W, v.X);
		public static Vector3 zwy (this Vector4 v) => new Vector3(v.Z, v.W, v.Y);
		public static Vector3 wxy (this Vector4 v) => new Vector3(v.W, v.X, v.Y);
		public static Vector3 wxz (this Vector4 v) => new Vector3(v.W, v.X, v.Z);
		public static Vector3 wyx (this Vector4 v) => new Vector3(v.W, v.Y, v.X);
		public static Vector3 wyz (this Vector4 v) => new Vector3(v.W, v.Y, v.Z);
		public static Vector3 wzx (this Vector4 v) => new Vector3(v.W, v.Z, v.X);
		public static Vector3 wzy (this Vector4 v) => new Vector3(v.W, v.Z, v.Y);
		
		public static Vector3 Expand(this Vector2 v, float z) => new Vector3(v.X, v.Y, z);
		
		public static Vector4 xywz (this Vector4 v) => new Vector4(v.X, v.Y, v.W, v.Z);
		public static Vector4 xzyw (this Vector4 v) => new Vector4(v.X, v.Z, v.Y, v.W);
		public static Vector4 xzwy (this Vector4 v) => new Vector4(v.X, v.Z, v.W, v.Y);
		public static Vector4 xwyz (this Vector4 v) => new Vector4(v.X, v.W, v.Y, v.Z);
		public static Vector4 xwzy (this Vector4 v) => new Vector4(v.X, v.W, v.Z, v.Y);
		public static Vector4 yxzw (this Vector4 v) => new Vector4(v.Y, v.X, v.Z, v.W);
		public static Vector4 yxwz (this Vector4 v) => new Vector4(v.Y, v.X, v.W, v.Z);
		public static Vector4 yzxw (this Vector4 v) => new Vector4(v.Y, v.Z, v.X, v.W);
		public static Vector4 yzwx (this Vector4 v) => new Vector4(v.Y, v.Z, v.W, v.X);
		public static Vector4 ywxz (this Vector4 v) => new Vector4(v.Y, v.W, v.X, v.Z);
		public static Vector4 ywzx (this Vector4 v) => new Vector4(v.Y, v.W, v.Z, v.X);
		public static Vector4 zxyw (this Vector4 v) => new Vector4(v.Z, v.X, v.Y, v.W);
		public static Vector4 zxwy (this Vector4 v) => new Vector4(v.Z, v.X, v.W, v.Y);
		public static Vector4 zyxw (this Vector4 v) => new Vector4(v.Z, v.Y, v.X, v.W);
		public static Vector4 zywx (this Vector4 v) => new Vector4(v.Z, v.Y, v.W, v.X);
		public static Vector4 zwxy (this Vector4 v) => new Vector4(v.Z, v.W, v.X, v.Y);
		public static Vector4 zwyx (this Vector4 v) => new Vector4(v.Z, v.W, v.Y, v.X);
		public static Vector4 wxyz (this Vector4 v) => new Vector4(v.W, v.X, v.Y, v.Z);
		public static Vector4 wxzy (this Vector4 v) => new Vector4(v.W, v.X, v.Z, v.Y);
		public static Vector4 wyxz (this Vector4 v) => new Vector4(v.W, v.Y, v.X, v.Z);
		public static Vector4 wyzx (this Vector4 v) => new Vector4(v.W, v.Y, v.Z, v.X);
		public static Vector4 wzxy (this Vector4 v) => new Vector4(v.W, v.Z, v.X, v.Y);
		public static Vector4 wzyx (this Vector4 v) => new Vector4(v.W, v.Z, v.Y, v.X);
		
		public static Vector4 Expand(this Vector2 v, float z, float w) => new Vector4(v.X, v.Y, z, w);
		public static Vector4 Expand(this Vector3 v, float w) => new Vector4(v.X, v.Y, v.Z, w);
	}
}