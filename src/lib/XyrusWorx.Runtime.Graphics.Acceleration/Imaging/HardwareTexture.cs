using System;
using JetBrains.Annotations;
using SlimDX.Direct3D11;
using SlimDX.DXGI;

namespace XyrusWorx.Runtime.Graphics.Imaging
{

	[PublicAPI]
	public class HardwareTexture : HardwareResource
	{
		private Texture2D mTexture;
		private ShaderResourceView mResourceView;

		private int mArrayWidth;
		private int mArrayHeight;

		public HardwareTexture([NotNull] AccelerationDevice device, int width, int height) : base(device)
		{
			if (width <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(width));
			}
			
			if (height <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(height));
			}

			mArrayWidth = width;
			mArrayHeight = height;

			var description = new Texture2DDescription
			{
				ArraySize = 1,
				MipLevels = 1,
				BindFlags = BindFlags.ShaderResource,
				Format = SlimDX.DXGI.Format.R32G32B32A32_Float,
				CpuAccessFlags = CpuAccessFlags.Write,
				Width = width,
				Height = height,
				OptionFlags = ResourceOptionFlags.None,
				SampleDescription = new SampleDescription(1, 0),
				Usage = ResourceUsage.Dynamic
			};

			mTexture = new Texture2D(Device, description);
			mResourceView = new ShaderResourceView(Device, mTexture);
		}

		public int Width => mArrayWidth;
		public int Height => mArrayHeight;

		[Pure, NotNull]
		public HardwareTextureData LockBits() => new HardwareTextureData(this);

		[NotNull]
		internal Texture2D GetTexture2D() => mTexture;
		internal override ShaderResourceView GetShaderResourceView() => mResourceView;
	}
}