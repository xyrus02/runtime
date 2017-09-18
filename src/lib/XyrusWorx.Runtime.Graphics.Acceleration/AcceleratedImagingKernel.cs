using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using JetBrains.Annotations;
using SlimDX;
using SlimDX.D3DCompiler;
using SlimDX.Direct3D11;
using SlimDX.DXGI;
using XyrusWorx.Runtime.Graphics.IO;
using XyrusWorx.Runtime.IO;
using XyrusWorx.Windows;
using D3DBuffer = SlimDX.Direct3D11.Buffer;
using Device = SlimDX.Direct3D11.Device;
using MapFlags = SlimDX.Direct3D11.MapFlags;

namespace XyrusWorx.Runtime.Graphics
{
	[PublicAPI]
	public class AcceleratedImagingKernel : AcceleratedKernel, IImagingKernel
	{
		private Form mOffScreenWindow;
		private PixelShader mPixelShader;
		private SwapChain mSwapChain;
		private VertexShader mVertexShader;
		private ShaderBytecode mVsByteCode;
		private D3DBuffer mVertexBuffer;
		private DataStream mVertices;

		public AcceleratedImagingKernel([NotNull] AcceleratedComputationProvider provider, [NotNull] AcceleratedKernelBytecode bytecode) : base(provider, bytecode)
		{
			if (bytecode.KernelType != AcceleratedComputationKernelType.PixelShader)
			{
				throw new ArgumentException("A pixel shader kernel source was expected.");
			}

			Resources = new ShaderResourceList(this);
		}

		public IList<IStructuredBuffer> Resources { get; }
		public IDataStream<Vector4<byte>> Compute(int arrayWidth, int arrayHeight)
		{
			if (arrayWidth <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(arrayWidth));
			}

			if (arrayHeight <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(arrayHeight));
			}

			var device = Provider.HardwareDevice;
			var context = device.ImmediateContext;

			return ComputeUsingPsProfile(device, context, arrayWidth, arrayHeight);
		}

		protected override void OnInitialize()
		{
			var device = Provider.HardwareDevice;

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
			var vsBytecode = ShaderBytecode.Compile(vsSource, "main", "vs_5_0", ShaderFlags.None, EffectFlags.None);

			mSwapChain = Provider.CreateSwapChain(description);
			mVsByteCode = vsBytecode;
			mPixelShader = new PixelShader(device, Bytecode.HardwareBytecode);
			mVertexShader = new VertexShader(device, vsBytecode);
			mVertexBuffer = CreateVertexBuffer(device, out mVertices);
		}
		protected override void OnDestroy()
		{
			mOffScreenWindow?.Close();
			mSwapChain?.Dispose();
			mPixelShader?.Dispose();
			mVertexShader?.Dispose();
			mVsByteCode?.Dispose();
			mVertexBuffer?.Dispose();
			mVertices?.Dispose();

			mOffScreenWindow = null;
			mSwapChain = null;
			mPixelShader = null;
			mVertexShader = null;
			mVsByteCode = null;
			mVertexBuffer = null;
			mVertices = null;
		}

		protected override void SetConstant(StructuredHardwareResource constant, int address)
		{
			Provider.HardwareDevice.ImmediateContext.PixelShader.SetConstantBuffer(constant?.HardwareBuffer, address);
		}

		private D3DBuffer CreateVertexBuffer(Device device, out DataStream vertices)
		{
			vertices = new DataStream(20 * 4, true, true);

			vertices.Write(new Vector3(-0.5f, -0.5f, 0.5f)); vertices.Write(new Vector2(1f, 1f));
			vertices.Write(new Vector3(-0.5f,  0.5f, 0.5f)); vertices.Write(new Vector2(0f, 1f));
			vertices.Write(new Vector3( 0.5f, -0.5f, 0.5f)); vertices.Write(new Vector2(1f, 0f));
			vertices.Write(new Vector3( 0.5f,  0.5f, 0.5f)); vertices.Write(new Vector2(0f, 0f));

			vertices.Position = 0;

			return new D3DBuffer(device, vertices, (int)vertices.Length, ResourceUsage.Default, BindFlags.VertexBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0);

		}
		private ImageOutputBuffer CreateRenderTargetBuffer(DeviceContext context, int arrayWidth, int arrayHeight)
		{
			Execution.ExecuteOnUiThread(() =>
			{
				mOffScreenWindow.Width = arrayWidth;
				mOffScreenWindow.Height = arrayHeight;
			});

			mSwapChain.ResizeBuffers(1, arrayWidth, arrayHeight, Format.B8G8R8A8_UNorm, SwapChainFlags.None);
			context.Rasterizer.SetViewports(new Viewport(0.0f, 0.0f, arrayWidth, arrayHeight));

			context.Rasterizer.State =
				RasterizerState.FromDescription(context.Device,
					new RasterizerStateDescription
					{
						CullMode = CullMode.None,
						FillMode = FillMode.Solid
					});

			var buffer = new ImageOutputBuffer(Provider, mSwapChain);
			
			return buffer;
		}
		private IDataStream<Vector4<byte>> ComputeUsingPsProfile(Device device, DeviceContext context, int arrayWidth, int arrayHeight)
		{
			using (var output = CreateRenderTargetBuffer(context, arrayWidth, arrayHeight))
			{
				context.ClearRenderTargetView(output.View, new Color4(0, 0, 0));
				context.InputAssembler.InputLayout = new InputLayout(device, ShaderSignature.GetInputSignature(mVsByteCode), new[]
				{
					new InputElement("POSITION", 0, Format.R32G32B32_Float, 0),
					new InputElement("TEXCOORD", 0, Format.R32G32_Float, 12, 0, InputClassification.PerVertexData, 0)
				});
				context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleStrip;
				context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(mVertexBuffer, 20, 0));

				context.VertexShader.Set(mVertexShader);
				context.PixelShader.Set(mPixelShader);

				Constants.CastTo<AcceleratedKernelResourceList>()?.SendContext();
				Resources.CastTo<AcceleratedKernelResourceList>()?.SendContext();

				context.PixelShader.SetSampler(SamplerState.FromDescription(device, new SamplerDescription
				{
					AddressU = TextureAddressMode.Wrap,
					AddressV = TextureAddressMode.Wrap,
					AddressW = TextureAddressMode.Wrap,
					Filter = Filter.MinPointMagMipLinear
				}), 0);

				context.OutputMerger.SetTargets(output.View);
				context.Draw(4, 0);

				mSwapChain.Present(0, PresentFlags.None);

				context.VertexShader.Set(null);
				context.PixelShader.Set(null);

				var sourceDescription = output.HardwareBuffer.Description;

				sourceDescription.BindFlags = 0;
				sourceDescription.CpuAccessFlags = CpuAccessFlags.Read | CpuAccessFlags.Write;
				sourceDescription.Usage = ResourceUsage.Staging;

				var emitBuffer = new Texture2D(device, sourceDescription);

				context.CopyResource(output.HardwareBuffer, emitBuffer);

				return new ComputationDataStream<Vector4<byte>>(Provider, context.MapSubresource(emitBuffer, 0, MapMode.Read, MapFlags.None).Data, emitBuffer, 0)
				{
					OwnedResource = emitBuffer
				};
			}
		}

		class ShaderResourceList : AcceleratedKernelResourceList<IStructuredBuffer>
		{
			private readonly AcceleratedImagingKernel mParent;

			public ShaderResourceList(AcceleratedImagingKernel parent)
			{
				mParent = parent;
			}

			protected override void SetElement(IStructuredBuffer item, int index)
			{
				var rv = new[]
				{
					item.CastTo<HardwareTexture2D>()?.ResourceView,
					item.CastTo<StructuredHardwareBufferResource>()?.View
				};

				var rvv = rv.FirstOrDefault();
				
				mParent.Provider.HardwareDevice.ImmediateContext.PixelShader.SetShaderResource(rvv, index);
			}
		}
	}
}