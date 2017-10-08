using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using JetBrains.Annotations;
using XyrusWorx.Collections;

namespace XyrusWorx.Runtime.Expressions
{
	[PublicAPI]
	public class DynamicStructureBuilder : Resource
	{
		[NotNull]
		private static readonly string mAssemblyName = $"{typeof(DynamicStructureBuilder).Namespace}.Dynamic.Structures";
		
		private readonly Dictionary<StringKey, DynamicStructure> mInstances;
		private ModuleBuilder mModuleBuilder;

		public DynamicStructureBuilder([NotNull] string typeNamespace)
		{
			if (typeNamespace.NormalizeNull() == null)
			{
				throw new ArgumentNullException(nameof(typeNamespace));
			}
			
			var assemblyName = new AssemblyName(typeNamespace.Trim());
			var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndCollect);
			var moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName.Name);

			mModuleBuilder = moduleBuilder;
			mInstances = new Dictionary<StringKey, DynamicStructure>();
		}
		
		[NotNull]
		public IDynamicBufferBuilder CreateBuffer([NotNull] string typeName) => DynamicStructure.Create(this, typeName);

		internal void AddBuilder(StringKey builderKey, DynamicStructure builder) => mInstances.Add(builderKey, builder);
		internal bool ContainsBuilder(StringKey builderKey) => mInstances.ContainsKey(builderKey);
		internal void RemoveBuilder(StringKey builderKey)
		{
			if (mInstances.ContainsKey(builderKey))
			{
				mInstances[builderKey] = null;
			}
		}

		internal ModuleBuilder GetModuleBuilder() => mModuleBuilder;

		protected override void DisposeOverride()
		{
			mInstances.Values.ToArray().Foreach(x => x?.Dispose());
			mInstances.Clear();
			
			mModuleBuilder = null;
		}
	}
}
