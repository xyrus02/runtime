using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using SlimDX;
using SlimDX.D3DCompiler;
using XyrusWorx.Diagnostics;

namespace XyrusWorx.Runtime.Graphics
{
	[PublicAPI]
	public sealed class AcceleratedKernelBytecode : Resource
	{
		private static Regex mOutputExpression = new Regex(
			@"^(?:(?:\b[a-z]:|\\\\[a-z0-9 %._-]+\\[a-z0-9 $%._-]+)\\|\\?[^\\/:*?""<>|\x00-\x1F]+\\?)(?:[^\\/:*?""<>" 
			+ @"|\x00-\x1F]+\\)*[^\\/:*?""<>|\x00-\x1F]*Shader@0x[a-f0-9]{8}\((\d+),(\d+)\):\s((?:warning)|(?:error))\"
			+ @"sX(\d+):\s(.*?)$", RegexOptions.IgnoreCase | RegexOptions.Multiline);

		private ShaderBytecode mBytecode;
		private AcceleratedComputationKernelProfile mProfile;

		internal AcceleratedKernelBytecode(Stream stream, AcceleratedComputationKernelProfile profile)
		{
			var buffer = new byte[stream.Length];
			stream.Read(buffer, 0, buffer.Length);

			mBytecode = new ShaderBytecode(new DataStream(buffer, true, false));
			CompilerMessages = new List<AcceleratedKernelCompilerMessage>();
			mProfile = profile;
		}
		internal AcceleratedKernelBytecode(string source, string entryPoint, AcceleratedComputationKernelProfile profile)
		{
			var profileName = GetProfileName(profile);
			
			string compilerOutput;

			// DO NOT REMOVE THE "PACK_MATRIX_ROW_MAJOR" FLAG!!!!!!!!!
			mBytecode = ShaderBytecode.Compile(source, entryPoint, profileName, ShaderFlags.PackMatrixRowMajor | ShaderFlags.OptimizationLevel3, EffectFlags.None, null, null, out compilerOutput);
			mProfile = profile;

			CompilerMessages = ParseCompilerOutput(compilerOutput);
			HlslSourceCode = source;
			ShaderAssemblySourceCode = mBytecode.Disassemble(DisassemblyFlags.None);
		}

		public string HlslSourceCode { get; }
		public string ShaderAssemblySourceCode { get; }

		public IEnumerable<AcceleratedKernelCompilerMessage> CompilerMessages { get; }

		public AcceleratedComputationKernelProfile KernelProfile => mProfile;
		public AcceleratedComputationKernelType KernelType => GetType(KernelProfile);

		internal ShaderBytecode HardwareBytecode => mBytecode;
		internal static AcceleratedComputationKernelType GetType(AcceleratedComputationKernelProfile profile)
		{
			switch (GetProfileName(profile).Substring(0, 2).ToLower())
			{
				case "cs":
					return AcceleratedComputationKernelType.ComputeShader;
				case "ps":
					return AcceleratedComputationKernelType.PixelShader;
				default:
					throw new NotSupportedException($"The kernel profile \"{profile}\" is not supported.");
			}
		}
		internal static string GetProfileName(AcceleratedComputationKernelProfile profile)
		{
			switch (profile)
			{
				case AcceleratedComputationKernelProfile.DirectCompute4:
					return "cs_4_0";
				case AcceleratedComputationKernelProfile.DirectCompute5:
					return "cs_5_0";
				case AcceleratedComputationKernelProfile.PixelShader40:
					return "ps_4_0";
				case AcceleratedComputationKernelProfile.PixelShader50:
					return "ps_5_0";
				default:
					throw new NotSupportedException($"The kernel profile \"{profile}\" is not supported.");
			}
		}
		internal static IEnumerable<AcceleratedKernelCompilerMessage> ParseCompilerOutput(string output)
		{
			var matches = mOutputExpression.Matches(output).OfType<Match>();
			return
				from match in matches
				let lineNumber = match.Groups[1].Value.TryDeserialize<int>()
				let columnNumber = match.Groups[2].Value.TryDeserialize<int>()
				let type = match.Groups[3].Value.TryDeserialize<LogMessageClass>()
				let code = match.Groups[4].Value.TryDeserialize<uint>()
				let message = match.Groups[5].Value

				select new AcceleratedKernelCompilerMessage
				{
					Code = code,
					Message = message,
					Type = type,
					LineNumber = lineNumber,
					ColumnNumber = columnNumber,
					ErrorDescription = type == LogMessageClass.Error ? message : null,
					HasError = type == LogMessageClass.Error,
					ErrorDetails = type != LogMessageClass.Error ? null : new ErrorDetails
					{
						HResult = (int)(0x8000000 | code),
						StackTrace = null
					}
			};
		}

		protected override void DisposeOverride()
		{
			mBytecode?.Dispose();
			mBytecode = null;
		}
	}
}