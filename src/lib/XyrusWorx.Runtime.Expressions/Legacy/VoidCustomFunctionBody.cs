using JetBrains.Annotations;

namespace XyrusWorx.Runtime.Expressions
{
	[PublicAPI]
	public class VoidCustomFunctionBody : CustomFunctionBody
	{
		public VoidCustomFunctionBody() : base(null)
		{
		}
	}
}