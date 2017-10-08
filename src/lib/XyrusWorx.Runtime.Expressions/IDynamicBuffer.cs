using System;
using System.Reflection;
using JetBrains.Annotations;

namespace XyrusWorx.Runtime.Expressions 
{
	[PublicAPI]
	public interface IDynamicBuffer : IDisposable
	{
		[NotNull]
		IDynamicBuffer SetValue([NotNull] string fieldName, [CanBeNull] object value);

		[NotNull]
		IWritable GetUnmanagedBuffer();

		[NotNull]
		TypeInfo GetClrTypeInfo();
	}
}