using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using JetBrains.Annotations;
using XyrusWorx.Collections;
using XyrusWorx.Runtime.Expressions;

namespace XyrusWorx.Runtime
{
	[PublicAPI]
	public class DynamicHardwareConstantBufferContext : Resource
	{
		[NotNull]
		private readonly AccelerationDevice mProvider;
		private static readonly string mAssemblyName = $"{typeof(DynamicHardwareConstantBufferContext).Namespace}.Dynamic.BufferStructures";
		
		private readonly Dictionary<StringKey, AcceleratedDynamicBuffer> mInstances;
		private ModuleBuilder mModuleBuilder;

		public DynamicHardwareConstantBufferContext([NotNull] AccelerationDevice provider, [NotNull] string typeNamespace)
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

		internal AccelerationDevice GetProvider() => mProvider;
		internal ModuleBuilder GetModuleBuilder() => mModuleBuilder;

		protected override void DisposeOverride()
		{
			mInstances.Values.ToArray().Foreach(x => x?.Dispose());
			mInstances.Clear();
			
			mModuleBuilder = null;
		}
	}

}
