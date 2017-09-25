using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using JetBrains.Annotations;
using XyrusWorx.Collections;
using XyrusWorx.Runtime.IO;

namespace XyrusWorx.Runtime.Graphics
{
	[PublicAPI]
	public class AcceleratedDynamicBufferContext : Resource
	{
		[NotNull]
		private readonly AcceleratedComputationProvider mProvider;
		private static readonly string mAssemblyName = $"{typeof(AcceleratedDynamicBufferContext).Namespace}.Dynamic.BufferStructures";
		
		private readonly Dictionary<StringKey, AcceleratedDynamicBuffer> mInstances;
		private ModuleBuilder mModuleBuilder;

		public AcceleratedDynamicBufferContext([NotNull] AcceleratedComputationProvider provider, [NotNull] string typeNamespace)
		{
			if (provider == null)
			{
				throw new ArgumentNullException(nameof(provider));
			}
			
			if (typeNamespace.NormalizeNull() == null)
			{
				throw new ArgumentNullException(nameof(typeNamespace));
			}
			
			var assemblyName = new AssemblyName(typeNamespace.Trim());
			var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndCollect);
			var moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName.Name);

			mProvider = provider;
			mModuleBuilder = moduleBuilder;
			mInstances = new Dictionary<StringKey, AcceleratedDynamicBuffer>();
		}
		
		[NotNull]
		public IDynamicBufferBuilder CreateBuffer([NotNull] string typeName) => AcceleratedDynamicBuffer.Create(this, typeName);

		internal void AddBuilder(StringKey builderKey, AcceleratedDynamicBuffer builder) => mInstances.Add(builderKey, builder);
		internal bool ContainsBuilder(StringKey builderKey) => mInstances.ContainsKey(builderKey);
		internal void RemoveBuilder(StringKey builderKey)
		{
			if (mInstances.ContainsKey(builderKey))
			{
				mInstances[builderKey] = null;
			}
		}

		internal AcceleratedComputationProvider GetProvider() => mProvider;
		internal ModuleBuilder GetModuleBuilder() => mModuleBuilder;

		protected override void DisposeOverride()
		{
			mInstances.Values.ToArray().Foreach(x => x?.Dispose());
			mInstances.Clear();
			
			mModuleBuilder = null;
		}
	}

}
