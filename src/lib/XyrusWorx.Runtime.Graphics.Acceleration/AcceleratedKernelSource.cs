using System;
using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;
using SlimDX;
using SlimDX.D3DCompiler;

namespace XyrusWorx.Runtime.Graphics
{
	[PublicAPI]
	public class AcceleratedKernelSource
	{
		private readonly Stream mByteCode;
		private readonly AcceleratedComputationKernelProfile? mStaticProfile;
		private AcceleratedComputationKernelProfile mProfile;

		private string mEntryPoint = @"main";
		private string mSource;

		public AcceleratedKernelSource([NotNull] string source)
		{
			if (string.IsNullOrWhiteSpace(source))
			{
				throw new ArgumentNullException(nameof(source));
			}

			mSource = source;
			mProfile = AcceleratedComputationKernelProfile.DirectCompute5;
		}
		public AcceleratedKernelSource([NotNull] Stream byteCodeStream, AcceleratedComputationKernelProfile profile)
		{
			if (byteCodeStream == null)
			{
				throw new ArgumentNullException(nameof(byteCodeStream));
			}
			if (!byteCodeStream.CanRead)
			{
				throw new ArgumentException("Stream must be readable", nameof(byteCodeStream));
			}

			mByteCode = byteCodeStream;
			mStaticProfile = profile;
			mProfile = AcceleratedComputationKernelProfile.DirectCompute5;
		}

		public bool IsPrecompiled => mByteCode != null;

		public string Source
		{
			get
			{
				if (IsPrecompiled)
				{
					throw new InvalidOperationException("The kernel has been precompiled. The source code is not available.");
				}

				return mSource;
			}
			set
			{
				if (IsPrecompiled)
				{
					throw new InvalidOperationException("The kernel has been precompiled. The source code can't be changed.");
				}

				mSource = value;
			}
		}
		public string EntryPoint
		{
			get
			{
				if (IsPrecompiled)
				{
					throw new InvalidOperationException("The kernel has been precompiled. The entry point is not available.");
				}
				
				return mEntryPoint;
			}
			set
			{
				if (IsPrecompiled)
				{
					throw new InvalidOperationException("The kernel has been precompiled. The entry point can't be changed.");
				}

				if (string.IsNullOrWhiteSpace(value))
				{
					throw new ArgumentNullException(nameof(value));
				}

				mEntryPoint = value;

			}
		}

		public AcceleratedComputationKernelProfile KernelProfile
		{
			get { return mStaticProfile.GetValueOrDefault(mProfile); }
			set
			{
				if (IsPrecompiled)
				{
					throw new InvalidOperationException("The kernel has been precompiled. The profile can't be changed.");
				}

				mProfile = value;
			}
		}
		public AcceleratedComputationKernelType KernelType
		{
			get { return AcceleratedKernelBytecode.GetType(KernelProfile); }
		}

		public AcceleratedKernelBytecode Compile()
		{
			if (IsPrecompiled)
			{
				return new AcceleratedKernelBytecode(mByteCode, KernelProfile);
			}

			return new AcceleratedKernelBytecode(Source, EntryPoint, KernelProfile);
		}
		public static IEnumerable<AcceleratedKernelCompilerMessage> Verify([NotNull] string source, [NotNull] string entryPoint)
		{
			if (string.IsNullOrWhiteSpace(source))
			{
				throw new ArgumentNullException(nameof(source));
			}
			if (string.IsNullOrWhiteSpace(entryPoint))
			{
				throw new ArgumentNullException(nameof(entryPoint));
			}

			string compilerOutput;
			try
			{
				using (ShaderBytecode.Compile(source, entryPoint,
					AcceleratedKernelBytecode.GetProfileName(AcceleratedComputationKernelProfile.DirectCompute5),
					ShaderFlags.None, EffectFlags.None, null, null, out compilerOutput))
				{
				}
			}
			catch(CompilationException exception)
			{
				compilerOutput = exception.Message;
			}
			

			var messages = AcceleratedKernelBytecode.ParseCompilerOutput(compilerOutput);

			return messages;
		}
	}
}