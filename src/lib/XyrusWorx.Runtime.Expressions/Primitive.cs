using JetBrains.Annotations;

namespace XyrusWorx.Runtime.Expressions 
{
	[PublicAPI]
	public enum Primitive
	{
		[RuntimeKey("undefined")]
		Undefined,
		
		[RuntimeKey("float")]
		Float,
		
		[RuntimeKey("int")]
		Integer,
		
		[RuntimeKey("uint")]
		UnsignedInteger,
		
		[RuntimeKey("bool")]
		Boolean
	}
}