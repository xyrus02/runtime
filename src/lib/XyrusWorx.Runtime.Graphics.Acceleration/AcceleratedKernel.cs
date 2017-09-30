using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using SlimDX;
using SlimDX.D3DCompiler;
using SlimDX.Direct3D11;
using XyrusWorx.Diagnostics;
using XyrusWorx.Runtime.Expressions;

namespace XyrusWorx.Runtime
{
	[PublicAPI]
	public abstract class AcceleratedKernel : Resource
	{
		private ShaderBytecode mBytecode;
		private Device mDevice;

		internal AcceleratedKernel([NotNull] AccelerationDevice device)
		{
			if (device == null)
			{
				throw new ArgumentNullException(nameof(device));
			}
			
			mDevice = device.GetDevice();
		}
		
		internal Device Device => mDevice;
		internal ShaderBytecode Bytecode => mBytecode;
		
		protected abstract string GetProfileName();
		protected virtual void Deallocate(){}

		protected sealed override void DisposeOverride()
		{
			try
			{
				Deallocate();
			}
			finally
			{
				mBytecode?.Dispose();
				mBytecode = null;
				mDevice = null;
			}
		}

		protected void Load([NotNull] IReadableMemory bytecode)
		{
			if (bytecode == null)
			{
				throw new ArgumentNullException(nameof(bytecode));
			}
			
			var bytecodeData = new byte[bytecode.Size];
			
			bytecode.CopyTo(bytecodeData);
			mBytecode = new ShaderBytecode(new DataStream(bytecodeData, true, false));
		}
		protected void Compile([NotNull] KernelSourceWriter source, [NotNull] CompilerContext context)
		{
			if (source == null)
			{
				throw new ArgumentNullException(nameof(source));
			}
			
			if (context == null)
			{
				throw new ArgumentNullException(nameof(context));
			}
			
			mBytecode = ShaderBytecode.Compile(
				source.SourceCode, KernelSourceWriter.EntryPointName, GetProfileName(), 
				ShaderFlags.PackMatrixRowMajor | ShaderFlags.OptimizationLevel3, 
				EffectFlags.None, null, null, out var output);

			var parseRegex = new Regex(
				@"^(?:(?:\b[a-z]:|\\\\[a-z0-9 %._-]+\\[a-z0-9 $%._-]+)\\|\\?[^\\/:*?""<>|\x00-\x1F]+\\?)(?:[^\\/:*?""<>"
				+ @"|\x00-\x1F]+\\)*[^\\/:*?""<>|\x00-\x1F]*Shader@0x[a-f0-9]{8}\((\d+),(\d+)\):\s((?:warning)|(?:error))\"
				+ @"sX(\d+):\s(.*?)$", RegexOptions.IgnoreCase | RegexOptions.Multiline);
			
			var matches = parseRegex.Matches(output).OfType<Match>();
			var messages =
				from match in matches

				let lineNumber = match.Groups[1].Value.TryDeserialize<int>()
				let columnNumber = match.Groups[2].Value.TryDeserialize<int>()
				let type = match.Groups[3].Value.TryDeserialize<LogMessageClass>()
				let code = match.Groups[4].Value.TryDeserialize<uint>()
				let message = match.Groups[5].Value

				select new
				{
					Type = type,
					Text = message //match.Groups[0].Value
				};

			var hasError = false;
			var errors = new List<string>();

			foreach (var message in messages)
			{
				context.Writer.Write(message.Text, message.Type);

				if (message.Type == LogMessageClass.Error)
				{
					hasError = true;
					errors.Add(message.Text);
				}
			}

			if (!hasError)
			{
				return;
			}
			
			mBytecode?.Dispose();
			mBytecode = null;
				
			throw new Exception($"The source code contains errors:\r\n{string.Join("\r\n", errors)}");
		}
	}

}