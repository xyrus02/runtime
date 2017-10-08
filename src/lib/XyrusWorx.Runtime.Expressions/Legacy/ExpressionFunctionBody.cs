using System;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using JetBrains.Annotations;
using XyrusWorx.Collections;

namespace XyrusWorx.Runtime.Expressions
{
	[PublicAPI]
	public abstract class ExpressionFunctionBody : FunctionBody
	{
		protected ExpressionFunctionBody(Type returnType) : base(returnType)
		{
		}

		protected void WriteToBuilder([NotNull] StringBuilder builder, [NotNull] IKernelWriterContext context, [NotNull] Expression expression)
		{
			if (builder == null)
			{
				throw new ArgumentNullException(nameof(builder));
			}
			if (context == null)
			{
				throw new ArgumentNullException(nameof(context));
			}
			if (expression == null)
			{
				throw new ArgumentNullException(nameof(expression));
			}

			builder.Append("return ");
			WriteExpression(builder, expression, context);
			builder.Append(";");
		}

		public static ExpressionFunctionBody<Func<T, TOut>> Create<T, TOut>([NotNull] Expression<Func<T, TOut>> expression)
			where T : struct
			where TOut : struct
		{
			if (expression == null)
			{
				throw new ArgumentNullException(nameof(expression));
			}

			return new ExpressionFunctionBody<Func<T, TOut>>(expression, typeof(TOut));
		}
		
		public static ExpressionFunctionBody<Func<T1, T2, TOut>> Create<T1, T2, TOut>([NotNull] Expression<Func<T1, T2, TOut>> expression)
			where T1 : struct
			where T2 : struct
			where TOut : struct
		{
			if (expression == null)
			{
				throw new ArgumentNullException(nameof(expression));
			}

			return new ExpressionFunctionBody<Func<T1, T2, TOut>>(expression, typeof(TOut));
		}
		
		public static ExpressionFunctionBody<Func<T1, T2, T3, TOut>> Create<T1, T2, T3, TOut>([NotNull] Expression<Func<T1, T2, T3, TOut>> expression)
			where T1 : struct
			where T2 : struct
			where T3 : struct
			where TOut : struct
		{
			if (expression == null)
			{
				throw new ArgumentNullException(nameof(expression));
			}

			return new ExpressionFunctionBody<Func<T1, T2, T3, TOut>>(expression, typeof(TOut));
		}

		public static VoidExpressionFunctionBody<Action<T>> CreateVoid<T>([NotNull] Expression<Action<T>> expression) 
			where T : struct
		{
			if (expression == null)
			{
				throw new ArgumentNullException(nameof(expression));
			}

			return new VoidExpressionFunctionBody<Action<T>>(expression);
		}
		
		public static VoidExpressionFunctionBody<Action<T1, T2>> CreateVoid<T1, T2>([NotNull] Expression<Action<T1, T2>> expression) 
			where T1 : struct 
			where T2 : struct
		{
			if (expression == null)
			{
				throw new ArgumentNullException(nameof(expression));
			}

			return new VoidExpressionFunctionBody<Action<T1, T2>>(expression);
		}
		
		public static VoidExpressionFunctionBody<Action<T1, T2, T3>> CreateVoid<T1, T2, T3>([NotNull] Expression<Action<T1, T2, T3>> expression)
			where T1 : struct
			where T2 : struct
			where T3 : struct
		{
			if (expression == null)
			{
				throw new ArgumentNullException(nameof(expression));
			}

			return new VoidExpressionFunctionBody<Action<T1, T2, T3>>(expression);
		}

		public static void WriteExpression(StringBuilder target, Expression expression, IKernelWriterContext context)
		{
			if (WriteBlock(target, expression, context))
			{
				return;
			}

			if (WriteConstantExpression(target, expression, context))
			{
				return;
			}
			if (WriteParameterExpression(target, expression, context))
			{
				return;
			}
			if (WriteMemberAccessExpression(target, expression, context))
			{
				return;
			}

			if (WriteUnaryExpression(target, expression, context))
			{
				return;
			}
			if (WriteBinaryExpression(target, expression, context))
			{
				return;
			}
			if (WriteNewExpression(target, expression, context))
			{
				return;
			}

			if (WriteMethodCallExpression(target, expression, context))
			{
				return;
			}

			throw new Exception(string.Format("Unsupported expression node: {0} in {1}", expression.NodeType, expression.GetType().Name));
		}
		public static bool WriteBlock(StringBuilder target, Expression expression, IKernelWriterContext context, bool wrapLastExpressionInReturnStatement = false)
		{
			var block = expression as BlockExpression;
			if (block == null)
			{
				return false;
			}

			var blockBuilder = new StringBuilder();

			for (var i = 0; i < block.Expressions.Count - 1; i++)
			{
				var statement = block.Expressions[i];
				WriteExpression(blockBuilder, statement, context);
				blockBuilder.AppendLine(";");
			}

			if (wrapLastExpressionInReturnStatement)
			{
				blockBuilder.Append("return ");
				WriteExpression(blockBuilder, block.Expressions[block.Expressions.Count - 1], context);
				blockBuilder.AppendLine(";");
			}
			else
			{
				WriteExpression(blockBuilder, block.Expressions[block.Expressions.Count - 1], context);
				blockBuilder.AppendLine(";");
			}

			target.AppendLine(Environment.NewLine + "{");
			target.AppendLine(string.Join(Environment.NewLine, blockBuilder.ToString().Trim().Split(new[] { Environment.NewLine }, StringSplitOptions.None).Select(x => $"\t{x}")));
			target.AppendLine("}");

			return true;
		}
		public static bool WriteNewExpression(StringBuilder target, Expression expression, IKernelWriterContext context)
		{
			var constructor = expression as NewExpression;
			if (constructor == null)
			{
				return false;
			}

			Function.CheckTypeSupport(constructor.Type);

			target.Append(Function.IntrinsicTypes[constructor.Type]);
			target.Append("(");

			if (constructor.Arguments.Count > 0)
			{
				for (var i = 0; i < constructor.Arguments.Count - 1; i++)
				{
					WriteExpression(target, constructor.Arguments[i], context);
					target.Append(", ");
				}

				WriteExpression(target, constructor.Arguments[constructor.Arguments.Count - 1], context);
			}

			target.Append(")");

			return true;
		}
		public static bool WriteUnaryExpression(StringBuilder target, Expression expression, IKernelWriterContext context)
		{
			var unary = expression as UnaryExpression;
			if (unary == null)
			{
				return false;
			}

			Function.CheckTypeSupport(unary.Type);

			switch (unary.NodeType)
			{
				case ExpressionType.Convert:
				case ExpressionType.ConvertChecked:
					target.Append($"({Function.IntrinsicTypes[unary.Type]})");
					WriteExpression(target, unary.Operand, context);
					target.Append("");
					break;
				case ExpressionType.Negate:
				case ExpressionType.NegateChecked:
					target.Append("-");
					WriteExpression(target, unary.Operand, context);
					break;
				case ExpressionType.UnaryPlus:
					target.Append("+");
					WriteExpression(target, unary.Operand, context);
					break;
				case ExpressionType.Not:
					target.Append("!");
					WriteExpression(target, unary.Operand, context);
					break;
				case ExpressionType.PostDecrementAssign:
					WriteExpression(target, unary.Operand, context);
					target.Append("--");
					break;
				case ExpressionType.PostIncrementAssign:
					WriteExpression(target, unary.Operand, context);
					target.Append("++");
					break;
				case ExpressionType.PreDecrementAssign:
					target.Append("--");
					WriteExpression(target, unary.Operand, context);
					break;
				case ExpressionType.PreIncrementAssign:
					target.Append("++");
					WriteExpression(target, unary.Operand, context);
					break;
				case ExpressionType.OnesComplement:
					target.Append("~");
					WriteExpression(target, unary.Operand, context);
					break;
				case ExpressionType.Decrement:
					return WriteBinaryExpression(target, Expression.MakeBinary(ExpressionType.Subtract, unary.Operand, Expression.Constant(1)), context);
				case ExpressionType.Increment:
					return WriteBinaryExpression(target, Expression.MakeBinary(ExpressionType.Add, unary.Operand, Expression.Constant(1)), context);
				case ExpressionType.IsTrue:
					return WriteBinaryExpression(target, Expression.MakeBinary(ExpressionType.Equal, Expression.Constant(true), unary.Operand), context);
				case ExpressionType.IsFalse:
					return WriteBinaryExpression(target, Expression.MakeBinary(ExpressionType.Equal, Expression.Constant(false), unary.Operand), context);
				default:
					throw new Exception(string.Format("Unsupported unary expression: {0}", unary.NodeType));
			}

			return true;
		}
		public static bool WriteBinaryExpression(StringBuilder target, Expression expression, IKernelWriterContext context)
		{
			var binary = expression as BinaryExpression;
			if (binary == null)
			{
				return false;
			}

			Function.CheckTypeSupport(binary.Type);

			var yieldOperation = new Action<string>(op =>
			{
				target.Append("(");
				WriteExpression(target, binary.Left, context);
				target.Append($" {op} ");
				WriteExpression(target, binary.Right, context);
				target.Append(")");
			});

			switch (binary.NodeType)
			{
				case ExpressionType.Add:
				case ExpressionType.AddChecked:
					yieldOperation("+");
					break;
				case ExpressionType.And:
					yieldOperation("&");
					break;
				case ExpressionType.AndAlso:
					yieldOperation("&&");
					break;
				case ExpressionType.Divide:
					yieldOperation("/");
					break;
				case ExpressionType.Equal:
					yieldOperation("==");
					break;
				case ExpressionType.ExclusiveOr:
					yieldOperation("^");
					break;
				case ExpressionType.GreaterThan:
					yieldOperation(">");
					break;
				case ExpressionType.GreaterThanOrEqual:
					yieldOperation(">=");
					break;
				case ExpressionType.LeftShift:
					yieldOperation("<<");
					break;
				case ExpressionType.LessThan:
					yieldOperation("<");
					break;
				case ExpressionType.LessThanOrEqual:
					yieldOperation("<=");
					break;
				case ExpressionType.Modulo:
					yieldOperation("%");
					break;
				case ExpressionType.Multiply:
				case ExpressionType.MultiplyChecked:
					yieldOperation("*");
					break;
				case ExpressionType.NotEqual:
					yieldOperation("!=");
					break;
				case ExpressionType.Or:
					yieldOperation("|");
					break;
				case ExpressionType.OrElse:
					yieldOperation("||");
					break;
				case ExpressionType.Power:
					target.Append("pow(");
					WriteExpression(target, binary.Left, context);
					target.Append(", ");
					WriteExpression(target, binary.Right, context);
					target.Append(")");
					break;
				case ExpressionType.RightShift:
					yieldOperation(">>");
					break;
				case ExpressionType.Subtract:
				case ExpressionType.SubtractChecked:
					yieldOperation("-");
					break;
				case ExpressionType.Assign:
					yieldOperation("=");
					break;
				case ExpressionType.Index:
					WriteExpression(target, binary.Left, context);
					target.Append("[");
					WriteExpression(target, binary.Right, context);
					target.Append("]");
					break;
				case ExpressionType.AddAssign:
				case ExpressionType.AddAssignChecked:
					yieldOperation("+=");
					break;
				case ExpressionType.AndAssign:
					yieldOperation("&=");
					break;
				case ExpressionType.DivideAssign:
					yieldOperation("/=");
					break;
				case ExpressionType.ExclusiveOrAssign:
					yieldOperation("^=");
					break;
				case ExpressionType.LeftShiftAssign:
					yieldOperation("<<=");
					break;
				case ExpressionType.ModuloAssign:
					yieldOperation("&=");
					break;
				case ExpressionType.MultiplyAssign:
				case ExpressionType.MultiplyAssignChecked:
					yieldOperation("*=");
					break;
				case ExpressionType.OrAssign:
					yieldOperation("|=");
					break;
				case ExpressionType.PowerAssign:
					WriteExpression(target, binary.Left, context);
					target.Append(" = pow(");
					WriteExpression(target, binary.Left, context);
					target.Append(", ");
					WriteExpression(target, binary.Right, context);
					target.Append(")");
					break;
				case ExpressionType.RightShiftAssign:
					yieldOperation(">>=");
					break;
				case ExpressionType.SubtractAssign:
				case ExpressionType.SubtractAssignChecked:
					yieldOperation("-=");
					break;
				default:
					throw new Exception(string.Format("Unsupported binary expression: {0}", binary.NodeType));
			}

			return true;
		}
		public static bool WriteMemberAccessExpression(StringBuilder target, Expression expression, IKernelWriterContext context)
		{
			var access = expression as MemberExpression;
			if (access == null)
			{
				return false;
			}

			Declaration.IsValidSymbolLabel(access.Member.Name);

			WriteExpression(target, access.Expression, context);
			target.AppendFormat(".{0}", access.Member.Name);

			return true;
		}
		public static bool WriteParameterExpression(StringBuilder target, Expression expression, IKernelWriterContext context)
		{
			var parameter = expression as ParameterExpression;
			if (parameter == null)
			{
				return false;
			}

			Declaration.IsValidSymbolLabel(parameter.Name);
			Function.CheckTypeSupport(parameter.Type);
			target.Append(parameter.Name);

			return true;
		}
		public static bool WriteConstantExpression(StringBuilder target, Expression expression, IKernelWriterContext context)
		{
			var constant = expression as ConstantExpression;
			if (constant == null)
			{
				return false;
			}

			if (typeof(int) == constant.Type)
			{
				target.Append(((int)constant.Value).ToString(CultureInfo.InvariantCulture));
			}
			else if (typeof(uint) == constant.Type)
			{
				target.AppendFormat("{0}u", ((uint)constant.Value).ToString(CultureInfo.InvariantCulture));
			}
			else if (typeof(float) == constant.Type)
			{
				target.Append(((float)constant.Value).ToString(CultureInfo.InvariantCulture));
			}
			else if (typeof(double) == constant.Type)
			{
				target.Append(((float)(double)constant.Value).ToString(CultureInfo.InvariantCulture));
			}
			else if (typeof(bool) == constant.Type)
			{
				target.Append((bool)constant.Value ? "true" : "false");
			}
			else
			{
				throw new Exception(string.Format("The type \"{0}\" is not a supported constant type.", constant.Type.FullName));
			}

			return true;
		}
		public static bool WriteMethodCallExpression(StringBuilder target, Expression expression, IKernelWriterContext context)
		{
			var call = expression as MethodCallExpression;
			if (call == null)
			{
				return false;
			}

			if (call.Object == null) // static
			{
				var resolved = context.ResolveMethodCall(call.Method, call.Arguments);
				if (resolved != null)
				{
					target.Append("(");
					WriteExpression(target, resolved, context);
					target.Append(")");
					return true;
				}

				var systemFunctionName = context.GetSystemFunctionName(call.Method);
				if (!string.IsNullOrWhiteSpace(systemFunctionName))
				{
					target.Append($"{systemFunctionName}(");

					var args = call.Arguments;
					if (args.Count > 0)
					{
						for (var i = 0; i < args.Count - 1; i++)
						{
							WriteExpression(target, args[i], context);
							target.Append(", ");
						}

						WriteExpression(target, args[args.Count - 1], context);
					}

					target.Append(")");
					return true;
				}
			}

			return false;
		}
	}

	[PublicAPI]
	public class ExpressionFunctionBody<TDelegate> : ExpressionFunctionBody
	{
		private readonly Expression mExpression;
		private readonly Expression<TDelegate> mExpressionRoot;

		internal protected ExpressionFunctionBody([NotNull] Expression<TDelegate> expression, Type returnType) : base(returnType)
		{
			if (expression == null)
			{
				throw new ArgumentNullException(nameof(expression));
			}

			mExpression = expression.Body;
			mExpressionRoot = expression;
			expression.Parameters.Select(x => new Declaration(x.Name, x.Type)).Foreach(Parameters.Add);
		}

		[NotNull]
		public Expression Expression => mExpression;

		[NotNull]
		public Expression<TDelegate> Source => mExpressionRoot;

		public override void Write(StringBuilder builder, IKernelWriterContext context)
		{
			WriteToBuilder(builder, context, mExpression);
			builder.AppendLine();
		}
	}
}