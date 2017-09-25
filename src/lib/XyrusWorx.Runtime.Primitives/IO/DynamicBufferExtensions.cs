using JetBrains.Annotations;

namespace XyrusWorx.Runtime.IO 
{
	[PublicAPI]
	public static class DynamicBufferExtensions
	{
		[NotNull]
		public static IDynamicBufferBuilder Field<T>([NotNull] this IDynamicBufferBuilder builder, [NotNull] string fieldName) where T: struct => builder.Field(fieldName, typeof(T));
	}
}