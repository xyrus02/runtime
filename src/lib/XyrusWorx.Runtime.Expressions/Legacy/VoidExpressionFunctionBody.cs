using System.Linq.Expressions;
using JetBrains.Annotations;

namespace XyrusWorx.Runtime.Expressions
{
	[PublicAPI]
	public class VoidExpressionFunctionBody<TDelegate> : ExpressionFunctionBody<TDelegate>
	{
		internal VoidExpressionFunctionBody([NotNull] Expression<TDelegate> expression) : base(expression, null)
		{
		}
	}
}