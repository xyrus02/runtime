using System;
using JetBrains.Annotations;
using SlimDX;
using SlimDX.Direct3D11;

namespace XyrusWorx.Runtime.Imaging 
{
	[PublicAPI]
	public sealed class HardwareTextureData : Resource, IWritableTexture, IView, IMemoryBlock
	{
		private Texture2D mTexture;
		private DataBox mBox;

		public HardwareTextureData([NotNull] HardwareTexture texture)
		{
			if (texture == null)
			{
				throw new ArgumentNullException(nameof(texture));
			}
			
			mTexture = texture.GetTexture2D();
			mBox = texture.Device.ImmediateContext.MapSubresource(mTexture, 0, MapMode.WriteDiscard, MapFlags.None);
		}
		
		public TextureFormat Format => TextureFormat.Rgba;
		
		public int Stride => mTexture.Description.Width << 2;
		public int Height => mTexture.Description.Height;
		
		public IMemoryBlock RawMemory => this;
		
		public void Write(IntPtr source, int writeOffset, long bytesToWrite) 
			=> UnmanagedBlock.Copy(source, mBox.Data.DataPointer, 0, writeOffset, bytesToWrite);

		public unsafe uint this[int address]
		{
			set => *((uint*)(void*)(mBox.Data.DataPointer + address)) = value;
		}
		public Vector4<byte> this[Int2 xy]
		{
			set => this[xy.x, xy.y] = value;
		}
		public unsafe Vector4<byte> this[int x, int y]
		{
			set
			{
				var offset = y * Stride + (x << 2);
				var pPixel = (uint*)(void*)(mBox.Data.DataPointer + offset);

				*pPixel = Format.Pack(value);
			}
		}

		protected override void DisposeOverride()
		{
			mTexture?.Device.ImmediateContext.UnmapSubresource(mTexture, 0);
			mTexture = null;
			mBox = null;
		}

		IntPtr IMemoryBlock.GetPointer() => mBox.Data.DataPointer;
		long IMemoryBlock.Size => Stride * Height;
	}
}