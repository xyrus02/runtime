using System;
using JetBrains.Annotations;

namespace XyrusWorx.Runtime.Graphics 
{
	[PublicAPI]
	public struct TextureFormat : IEquatable<TextureFormat>
	{
		// RRR     # of red byte
		// GGG     # of green byte
		// BBB     # of blue byte
		// AAA     # of alpha byte
		//
		// byte # starts with 1; zero in R, G, B or A means, that the byte is void
		//
		//                                                   RRR GGG BBB AAA   
		public static readonly TextureFormat Rgba      = 0b0_100_011_010_001_0u;
		public static readonly TextureFormat Bgra      = 0b0_010_011_100_001_0u;

		private readonly uint mLayout;
		
		public TextureFormat(uint layout)
		{
			mLayout = layout;
		}

		[Pure]
		public uint Map(uint sourceData, TextureFormat sourceFormat)
		{
			var sA = (int)((sourceFormat.mLayout & 0b0_000_000_000_111_0u) >> 0x1);
			var sR = (int)((sourceFormat.mLayout & 0b0_000_000_111_000_0u) >> 0x4);
			var sG = (int)((sourceFormat.mLayout & 0b0_000_111_000_000_0u) >> 0x7);
			var sB = (int)((sourceFormat.mLayout & 0b0_111_000_000_000_0u) >> 0xa);
			
			var tA = (int)((mLayout & 0b0_000_000_000_111_0u) >> 0x1);
			var tR = (int)((mLayout & 0b0_000_000_111_000_0u) >> 0x4);
			var tG = (int)((mLayout & 0b0_000_111_000_000_0u) >> 0x7);
			var tB = (int)((mLayout & 0b0_111_000_000_000_0u) >> 0xa);
			
			var o = 0u;
			
			o |= sA * tA > 0 ? (uint)((sourceData >> (sA - 1)) & 0xff << (tA - 1)) : 0u;
			o |= sR * tR > 0 ? (uint)((sourceData >> (sR - 1)) & 0xff << (tR - 1)) : 0u;
			o |= sG * tG > 0 ? (uint)((sourceData >> (sG - 1)) & 0xff << (tG - 1)) : 0u;
			o |= sB * tB > 0 ? (uint)((sourceData >> (sB - 1)) & 0xff << (tB - 1)) : 0u;

			return o;
		}
		
		[Pure]
		public uint Pack(Vector4<byte> pixelValue)
		{
			var sA = (int)((mLayout & 0b0_000_000_000_111_0u) >> 0x1);
			var sR = (int)((mLayout & 0b0_000_000_111_000_0u) >> 0x4);
			var sG = (int)((mLayout & 0b0_000_111_000_000_0u) >> 0x7);
			var sB = (int)((mLayout & 0b0_111_000_000_000_0u) >> 0xa);

			var o = sA == 0 ? 0 : (uint)pixelValue.w << (sA - 1);
			
			o |= sR == 0 ? 0 : (uint)pixelValue.x << (sR - 1);
			o |= sG == 0 ? 0 : (uint)pixelValue.y << (sG - 1);
			o |= sB == 0 ? 0 : (uint)pixelValue.z << (sB - 1);

			return o;
		}
		
		[Pure]
		public Vector4<byte> Unpack(uint data)
		{
			var sA = (int)((mLayout & 0b0_000_000_000_111_0u) >> 0x1);
			var sR = (int)((mLayout & 0b0_000_000_111_000_0u) >> 0x4);
			var sG = (int)((mLayout & 0b0_000_111_000_000_0u) >> 0x7);
			var sB = (int)((mLayout & 0b0_111_000_000_000_0u) >> 0xa);

			var a = sA > 0 ? (byte)((data >> (sA - 1)) & 0xff) : byte.MinValue;
			var r = sR > 0 ? (byte)((data >> (sR - 1)) & 0xff) : byte.MinValue;
			var g = sG > 0 ? (byte)((data >> (sG - 1)) & 0xff) : byte.MinValue;
			var b = sB > 0 ? (byte)((data >> (sB - 1)) & 0xff) : byte.MinValue;
			
			return new Vector4<byte>(r, g, b, a);
		}
		
		public bool Equals(TextureFormat other) => mLayout == other.mLayout;
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
			{
				return false;
			}
			
			return obj is TextureFormat && Equals((TextureFormat)obj);
		}
		public override int GetHashCode() => (int)mLayout;

		public static bool operator ==(TextureFormat left, TextureFormat right) => left.Equals(right);
		public static bool operator !=(TextureFormat left, TextureFormat right) => !left.Equals(right);
		
		public static implicit operator TextureFormat(uint layout) => new TextureFormat(layout);
	}
}