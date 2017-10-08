using System;
using JetBrains.Annotations;

namespace XyrusWorx.Runtime.Expressions 
{
	[PublicAPI]
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Field)]
	public sealed class RuntimeKeyAttribute : Attribute
	{
		internal RuntimeKeyAttribute(string key)
		{
			Key = key;
		}
		
		public string Key { get; }
	}
}