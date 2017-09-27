using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace XyrusWorx.Runtime.Native 
{
	[StructLayout(LayoutKind.Explicit)]
	[SuppressMessage("ReSharper", "InconsistentNaming")]
	struct FloatOrInteger
	{
		[FieldOffset(0)]
		public float f;

		[FieldOffset(0)]
		public int i;
	}
}