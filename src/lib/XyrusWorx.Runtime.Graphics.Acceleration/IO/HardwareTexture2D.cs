using System;
using JetBrains.Annotations;
using SlimDX.Direct3D11;
using SlimDX.DXGI;
using MapFlags = SlimDX.Direct3D11.MapFlags;

namespace XyrusWorx.Runtime.Graphics.IO
{
	[PublicAPI]
	public class HardwareTexture2D : HardwareBufferResource, ITexture2D
	{
		private AcceleratedComputationProvider mProvider;
		private Texture2D mTexture;
		private ShaderResourceView mResourceView;

		private int mArrayWidth;
		private int mArrayHeight;

		public HardwareTexture2D([NotNull] AcceleratedComputationProvider provider, int width, int height)
		{
			if (provider == null)
			{
				throw new ArgumentNullException(nameof(provider));
			}
			
			if (width <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(width));
			}
			
			if (height <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(height));
			}

			mProvider = provider;
			mArrayWidth = width;
			mArrayHeight = height;

			var description = new Texture2DDescription
			{
				ArraySize = 1,
				MipLevels = 1,
				BindFlags = BindFlags.ShaderResource,
				Format = Format.R32G32B32A32_Float,
				CpuAccessFlags = CpuAccessFlags.Write,
				Width = width,
				Height = height,
				OptionFlags = ResourceOptionFlags.None,
				SampleDescription = new SampleDescription(1, 0),
				Usage = ResourceUsage.Dynamic
			};

			mTexture = new Texture2D(provider.HardwareDevice, description);
			mResourceView = new ShaderResourceView(provider.HardwareDevice, mTexture);
		}

		public sealed override int BufferSize => mArrayWidth * mArrayHeight * 4;
		public sealed override int ElementCount => mArrayWidth * mArrayHeight;

		public int Stride => mArrayWidth * 4;
		public int Width => mArrayWidth;
		public int Height => mArrayHeight;

		public void LoadFromSwap(ComputationSwapBufferResource<Float4> swap)
		{
			var context = mProvider.HardwareDevice.ImmediateContext;
			var box = context.MapSubresource(mTexture, 0, MapMode.WriteDiscard, MapFlags.None);

			using (var stream = swap.Read())
			{
				var buffer = new byte[stream.Length];
				stream.Read(buffer, 0, buffer.Length);
				box.Data.Write(buffer, 0, buffer.Length);
			}
			
			context.UnmapSubresource(mTexture, 0);
		}

		public void Write(byte[] data)
		{
			Write(data, 0, data.Length);
		}
		public void Write(byte[] data, int offset, int count)
		{
			var context = mProvider.HardwareDevice.ImmediateContext;
			var box = context.MapSubresource(mTexture, 0, MapMode.WriteDiscard, MapFlags.None);

			box.Data.WriteRange(data, offset, count);
			context.UnmapSubresource(mTexture, 0);
		}
		public void Write(IntPtr ptr)
		{
			var context = mProvider.HardwareDevice.ImmediateContext;
			var box = context.MapSubresource(mTexture, 0, MapMode.WriteDiscard, MapFlags.None);

			box.Data.WriteRange(ptr, BufferSize);
			context.UnmapSubresource(mTexture, 0);
		}

		internal Texture2D Texture => mTexture;
		internal ShaderResourceView ResourceView => mResourceView;

		protected override void OnCleanup()
		{
			mTexture?.Dispose();
		}
	}
}