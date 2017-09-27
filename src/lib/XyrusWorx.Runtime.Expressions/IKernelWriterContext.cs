using System.Collections;
using System.Linq.Expressions;
using System.Reflection;
using JetBrains.Annotations;

namespace XyrusWorx.Runtime.Expressions 
{
	[PublicAPI]
	public interface IKernelWriterContext 
	{
		[NotNull]
		SymbolScope<Expression> Defines { get; }
		
		[NotNull]
		SymbolScope<ConstantBufferDefinition> ConstantBuffers { get; }
		
		Expression ResolveMethodCall(MethodInfo method, IList arguments);
		string GetSystemFunctionName(MethodInfo method);
		string GetProgramHeader();
	}
}