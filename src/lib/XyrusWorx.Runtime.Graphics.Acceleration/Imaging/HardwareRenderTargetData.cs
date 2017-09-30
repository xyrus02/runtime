using System;
using JetBrains.Annotations;
using SlimDX;
using SlimDX.Direct3D11;

namespace XyrusWorx.Runtime.Imaging 
{
	[PublicAPI]
	public sealed class HardwareRenderTargetData : Resource, IReadableTexture, IView, IMemoryBlock
	{
		private Texture2D mTexture;
		private DataBox mBox;

		public HardwareRenderTargetData([NotNull] HardwareRenderTarget renderTarget)
		{
			if (renderTarget == null)
			{
				throw new ArgumentNullException(nameof(renderTarget));
			}
			
			mTexture = renderTarget.GetTexture2D();
			mBox = renderTarget.Device.ImmediateContext.MapSubresource(mTexture, 0, MapMode.Read, MapFlags.None);
		}
		
		public TextureFormat Format => TextureFormat.Rgba;
		
		public int Stride => mTexture.Description.Width << 2;
		public int Height => mTexture.Description.Height;
		
		public IMemoryBlock RawMemory => this;
		
		public void Read(IntPtr target, int readOffset, long bytesToRead)
			=> UnmanagedBlock.Copy(mBox.Data.DataPointer, target, readOffset, 0, bytesToRead);

		public Vector4<byte> this[Int2 xy]
		{
			get => this[xy.x, xy.y];
		}
		public unsafe Vector4<byte> this[int x, int y]
		{
			get
			{
				var offset = y * Stride + (x << 2);
				var pPixel = (uint*)(void*)(mBox.Data.DataPointer + offset);
				
				return Format.Unpack(*pPixel);
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