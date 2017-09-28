using System;
using System.Windows.Forms;
using JetBrains.Annotations;
using SlimDX;
using SlimDX.D3DCompiler;
using SlimDX.Direct3D11;
using SlimDX.DXGI;
using XyrusWorx.Runtime.Graphics.Imaging;
using XyrusWorx.Runtime.Imaging;
using XyrusWorx.Windows;
using Buffer = SlimDX.Direct3D11.Buffer;
using Format = SlimDX.DXGI.Format;
using MapFlags = SlimDX.Direct3D11.MapFlags;
using PixelShader = SlimDX.Direct3D11.PixelShader;
using ShaderBytecode = SlimDX.D3DCompiler.ShaderBytecode;
using ShaderFlags = SlimDX.D3DCompiler.ShaderFlags;
using SwapChain = SlimDX.DXGI.SwapChain;
using Usage = SlimDX.DXGI.Usage;
using VertexShader = SlimDX.Direct3D11.VertexShader;

namespace XyrusWorx.Runtime.Graphics 
{
	[PublicAPI]
	public interface IAcceleratedImagingKernelConfiguration
	{
		[NotNull]
		AcceleratedImagingKernel TextureSize(Int2 size);
		
		[NotNull]
		AcceleratedImagingKernel TextureSize(int width, int height);
	}
	
	[PublicAPI]
	public sealed class AcceleratedImagingKernel : AcceleratedKernel, IImagingKernel
	{
		[NotNull]
		private readonly AccelerationDevice mDevice;
		private static readonly ShaderBytecode mVertexShaderBytecode;
		
		private int mWidth;
		private int mHeight;
		
		private Form mOffScreenWindow;
		private SwapChain mSwapChain;
		private PixelShader mPixelShader;
		private VertexShader mVertexShader;
		private Buffer mVertexBuffer;
		private DataStream mVertices;
		private HardwareRenderTarget mRenderTarget;
		private UnmanagedBlock mOutputTextureMemory;
		private ReadOnlyTextureView mOutputTexture;

		static AcceleratedImagingKernel()
		{
			var vsSource =
				@"struct VS_INPUT{
					float3 pos : POSITION;
					float2 tex : TEXCOORD;
				};
				struct PS_INPUT{
					float4 pos : SV_POSITION;
					float2 tex : TEXCOORD;
				};

				PS_INPUT main(VS_INPUT input)
				{
					PS_INPUT output=(PS_INPUT)0;

					output.pos = float4(input.pos,0.5);
					output.tex = float2(1 - input.tex.y, input.tex.x);

					return output;
				}
				";
			mVertexShaderBytecode = ShaderBytecode.Compile(vsSource, "main", "vs_5_0", ShaderFlags.None, EffectFlags.None);
		}
		internal AcceleratedImagingKernel([NotNull] AccelerationDevice device, int width, int height) : base(device)
		{
			mDevice = device;
			CreateView(width, height);
			
			// todo: resource lists
		}

		public IResourcePool<IWritableMemory> Constants { get; }
		public IResourcePool<IWritableMemory> Textures { get; }
		public IReadableTexture Output { get; }
		
		public void Execute()
		{
			var context = Device.ImmediateContext;
			var view = mRenderTarget.GetRenderTargetView();
			
			context.ClearRenderTargetView(view, new Color4(0, 0, 0));
			context.InputAssembler.InputLayout = new InputLayout(Device, ShaderSignature.GetInputSignature(mVertexShaderBytecode), new[]
			{
				new InputElement("POSITION", 0, Format.R32G32B32_Float, 0),
				new InputElement("TEXCOORD", 0, Format.R32G32_Float, 12, 0, InputClassification.PerVertexData, 0)
			});
			context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleStrip;
			context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(mVertexBuffer, 20, 0));

			context.VertexShader.Set(mVertexShader);
			context.PixelShader.Set(mPixelShader);

			Constants.CastTo<AcceleratedKernelResourceList>()?.SendContext();
			Textures.CastTo<AcceleratedKernelResourceList>()?.SendContext();

			context.PixelShader.SetSampler(SamplerState.FromDescription(Device, new SamplerDescription
			{
				AddressU = TextureAddressMode.Wrap,
				AddressV = TextureAddressMode.Wrap,
				AddressW = TextureAddressMode.Wrap,
				Filter = Filter.MinPointMagMipLinear
			}), 0);

			context.OutputMerger.SetTargets(view);
			context.Draw(4, 0);

			mSwapChain.Present(0, PresentFlags.None);

			context.VertexShader.Set(null);
			context.PixelShader.Set(null);
			
			var sourceDescription = mRenderTarget.GetTexture2D().Description;

			sourceDescription.BindFlags = 0;
			sourceDescription.CpuAccessFlags = CpuAccessFlags.Read | CpuAccessFlags.Write;
			sourceDescription.Usage = ResourceUsage.Staging;

			// todo: maybe we don't need to copy the texture?
			using (var buffer = new Texture2D(Device, sourceDescription))
			{
				context.CopyResource(mRenderTarget.GetTexture2D(), buffer);

				DataBox box = null;
				try
				{
					box = context.MapSubresource(buffer, 0, MapMode.Read, MapFlags.None);
					mOutputTextureMemory.Read(box.Data.DataPointer, 0, mWidth * mHeight * 4);
				}
				finally
				{
					if (box != null)
					{
						context.UnmapSubresource(buffer, 0);
					}
				}
			}
		}
		
		[NotNull]
		public static IAcceleratedImagingKernelConfiguration FromBytecode([NotNull] AccelerationDevice device, [NotNull] IReadableMemory bytecode)
		{
			if (device == null)
			{
				throw new ArgumentNullException(nameof(device));
			}
			
			if (bytecode == null)
			{
				throw new ArgumentNullException(nameof(bytecode));
			}

			return new AcceleratedImagingKernelConfiguration(device, k => k.Load(bytecode));
		}
		
		[NotNull]
		public static IAcceleratedImagingKernelConfiguration FromSource([NotNull] AccelerationDevice device, [NotNull] AcceleratedKernelSourceWriter source, CompilerContext context = null)
		{
			if (device == null)
			{
				throw new ArgumentNullException(nameof(device));
			}
			
			if (source == null)
			{
				throw new ArgumentNullException(nameof(source));
			}

			context = context ?? new CompilerContext();
			return new AcceleratedImagingKernelConfiguration(device, k => k.Compile(source, context));
		}
		
		protected override string GetProfileName() => "ps_5_0";

		private Buffer CreateVertexBuffer(out DataStream vertices)
		{
			vertices = new DataStream(20 * 4, true, true);

			vertices.Write(new Vector3(-0.5f, -0.5f, 0.5f)); vertices.Write(new Vector2(1f, 1f));
			vertices.Write(new Vector3(-0.5f,  0.5f, 0.5f)); vertices.Write(new Vector2(0f, 1f));
			vertices.Write(new Vector3( 0.5f, -0.5f, 0.5f)); vertices.Write(new Vector2(1f, 0f));
			vertices.Write(new Vector3( 0.5f,  0.5f, 0.5f)); vertices.Write(new Vector2(0f, 0f));

			vertices.Position = 0;

			return new Buffer(Device, vertices, (int)vertices.Length, ResourceUsage.Default, BindFlags.VertexBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0);

		}
		private void CreateView(int width, int height)
		{
			if (width <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(width));
			}
			
			if (height <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(height));
			}
			
			Destroy();
			
			mWidth = width;
			mHeight = height;
			
			Execution.ExecuteOnUiThread(() =>
			{
				mOffScreenWindow = new Form();
				mOffScreenWindow.Left = -65535;
				mOffScreenWindow.Top = -65535;
				mOffScreenWindow.FormBorderStyle = FormBorderStyle.None;
				mOffScreenWindow.Width = 1;
				mOffScreenWindow.Height = 1;
				mOffScreenWindow.ShowInTaskbar = false;
				mOffScreenWindow.Show();
				mOffScreenWindow.Hide();
			});

			var description = new SwapChainDescription
			{
				BufferCount = 1,
				IsWindowed = true,
				ModeDescription = new ModeDescription(0, 0, new Rational(60, 1), Format.B8G8R8A8_UNorm),
				SampleDescription = new SampleDescription(1, 0),
				Usage = Usage.RenderTargetOutput | Usage.Shared,
				OutputHandle = mOffScreenWindow.Handle
			};
			
			mSwapChain = mDevice.CreateSwapChain(description);
			mPixelShader = new PixelShader(Device, Bytecode);
			mVertexShader = new VertexShader(Device, mVertexShaderBytecode);
			mVertexBuffer = CreateVertexBuffer(out mVertices);
			
			Execution.ExecuteOnUiThread(() =>
			{
				mOffScreenWindow.Width = mWidth;
				mOffScreenWindow.Height = mHeight;
			});

			mSwapChain.ResizeBuffers(1, mWidth, mHeight, Format.B8G8R8A8_UNorm, SwapChainFlags.None);
			Device.ImmediateContext.Rasterizer.SetViewports(new Viewport(0.0f, 0.0f, mWidth, mHeight));

			Device.ImmediateContext.Rasterizer.State =
				RasterizerState.FromDescription(Device,
					new RasterizerStateDescription
					{
						CullMode = CullMode.None,
						FillMode = FillMode.Solid
					});

			mRenderTarget = new HardwareRenderTarget(mDevice, mSwapChain);
			mOutputTextureMemory = new UnmanagedBlock(mWidth * mHeight * 4);
			mOutputTexture = new ReadOnlyTextureView(mOutputTextureMemory, mWidth * 4, TextureFormat.Rgba);
			
		}
		private void Destroy()
		{
			mOffScreenWindow?.Close();
			mSwapChain?.Dispose();
			mPixelShader?.Dispose();
			mVertexShader?.Dispose();
			mVertexBuffer?.Dispose();
			mVertices?.Dispose();
			mRenderTarget?.Dispose();
			mOutputTextureMemory?.Dispose();

			mOffScreenWindow = null;
			mSwapChain = null;
			mPixelShader = null;
			mVertexShader = null;
			mVertexBuffer = null;
			mVertices = null;
			mRenderTarget = null;
			mOutputTextureMemory = null;
			mOutputTexture = null;
		}
		
		sealed class AcceleratedImagingKernelConfiguration : IAcceleratedImagingKernelConfiguration
		{
			private readonly AccelerationDevice mDevice;
			private readonly Action<AcceleratedImagingKernel> mConstructor;

			public AcceleratedImagingKernelConfiguration(AccelerationDevice device, Action<AcceleratedImagingKernel> constructor)
			{
				mDevice = device;
				mConstructor = constructor;
			}
			
			public AcceleratedImagingKernel TextureSize(Int2 size) => TextureSize(size.x, size.y);
			public AcceleratedImagingKernel TextureSize(int width, int height)
			{
				if (width <= 0)
				{
					throw new ArgumentOutOfRangeException(nameof(width));
				}
			
				if (height <= 0)
				{
					throw new ArgumentOutOfRangeException(nameof(height));
				}
			
				var kernel = new AcceleratedImagingKernel(mDevice, width, height);
				mConstructor(kernel);
				return kernel;
			}
		}
	}
}