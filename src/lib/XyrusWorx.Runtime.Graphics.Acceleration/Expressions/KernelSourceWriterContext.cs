using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using JetBrains.Annotations;

namespace XyrusWorx.Runtime.Expressions
{
	[PublicAPI]
	public class KernelSourceWriterContext : IKernelWriterContext
	{
		internal KernelSourceWriterContext()
		{
			Defines = new SymbolScope<Expression>();
			ConstantBuffers = new SymbolScope<ConstantBufferDefinition>();
		}

		public SymbolScope<Expression> Defines { get; }
		public SymbolScope<ConstantBufferDefinition> ConstantBuffers { get; }

		public static T Define<T>(string name) => default(T);
		public static T Constant<T>(string name) => default(T);
		public static T Call<T>(string name, params object[] parameters) => default(T);

		public Expression ResolveMethodCall(MethodInfo method, IList arguments)
		{
			if (method.DeclaringType == typeof(KernelSourceWriterContext))
			{
				if (method.Name == nameof(Define))
				{
					return GetDefine(method.GetGenericArguments(), arguments);
				}

				if (method.Name == nameof(Constant))
				{
					return GetConstant(method.GetGenericArguments(), arguments);
				}
			}

			return null;
		}
		public string GetSystemFunctionName(MethodInfo method)
		{
			if (method.DeclaringType == typeof (Mathf))
			{
				return method.Name.ToLower();
			}

			if (method.DeclaringType == typeof (Math))
			{
				switch (method.Name)
				{
					case nameof(Math.Acos):
					case nameof(Math.Asin):
					case nameof(Math.Atan):
					case nameof(Math.Atan2):
					case nameof(Math.Floor):
					case nameof(Math.Round):
					case nameof(Math.Cos):
					case nameof(Math.Sin):
					case nameof(Math.Tan):
					case nameof(Math.Cosh):
					case nameof(Math.Sinh):
					case nameof(Math.Tanh):
					case nameof(Math.Exp):
					case nameof(Math.Log):
					case nameof(Math.Log10):
					case nameof(Math.Min):
					case nameof(Math.Max):
					case nameof(Math.Pow):
					case nameof(Math.Sign):
					case nameof(Math.Sqrt):
						return method.Name.ToLower();

					case nameof(Math.Truncate):
						return nameof(Mathf.Trunc).ToLower();
					case nameof(Math.Ceiling):
						return nameof(Mathf.Ceil).ToLower();

					default:
						return null;
				}
			}

			return null;
		}

		public string GetProgramHeader()
		{
			var result = new StringBuilder();

			foreach (var define in Defines.Keys)
			{
				result.Append($"#define {define} (");
				ExpressionFunctionBody.WriteExpression(result, Defines[define], this);
				result.AppendLine(")");
			}

			if (Defines.Keys.Any())
			{
				result.AppendLine("");
			}

			foreach (var buffer in ConstantBuffers.Values)
			{
				buffer.Write(result, this);
			}

			return result.ToString();
		}

		private Expression GetDefine(IList<Type> types, IList arguments)
		{
			if (arguments.Count != 1 || !(arguments[0] is ConstantExpression))
			{
				throw new KernelSourceException("Failed to obtain preprocessor definition: invalid access to surrogate method.");
			}

			var defineName = (((ConstantExpression)arguments[0]).Value as string)?.Trim();
			var requestedType = types[0];

			if (string.IsNullOrWhiteSpace(defineName))
			{
				throw new KernelSourceException($"Unknown preprocessor definition: \"{defineName ?? "<null>"}\"");
			}

			Declaration.IsValidSymbolLabel(defineName);
			Function.CheckTypeSupport(requestedType);

			var define = Defines[defineName];
			if (define == null)
			{
				throw new KernelSourceException($"Unknown preprocessor definition: \"{defineName}\"");
			}

			if (requestedType != define.Type)
			{
				if (define.Type == typeof(double))
				{
					if (requestedType != typeof(double) && requestedType != typeof(float))
					{
						throw new KernelSourceException($"The preprocessor definition points to an expression of type \"{Function.IntrinsicTypes[define.Type]}\" but an expression of type \"{Function.IntrinsicTypes[requestedType]}\" was requested.");
					}
				}
				else
				{
					throw new KernelSourceException($"The preprocessor definition points to an expression of type \"{Function.IntrinsicTypes[define.Type]}\" but an expression of type \"{Function.IntrinsicTypes[requestedType]}\" was requested.");
				}
			}

			return Expression.Parameter(requestedType, defineName);
		}
		private Expression GetConstant(IList<Type> types, IList arguments)
		{
			if (arguments.Count != 1 || !(arguments[0] is ConstantExpression))
			{
				throw new KernelSourceException("Failed to obtain preprocessor definition: invalid access to surrogate method.");
			}

			var constantName = (((ConstantExpression)arguments[0]).Value as string)?.Trim();
			var requestedType = types[0];

			if (string.IsNullOrWhiteSpace(constantName))
			{
				throw new KernelSourceException($"Invalid preprocessor definition access: \"{constantName ?? "<null>"}\" is not a valid definition label.");
			}

			Declaration.IsValidSymbolLabel(constantName);
			Function.CheckTypeSupport(requestedType);

			var symbols = ConstantBuffers.Values.SelectMany(x => x.Symbols);
			var constant = symbols.FirstOrDefault(x => Equals(constantName, x.Label));

			if (constant == null)
			{
				throw new KernelSourceException($"Unknown preprocessor definition: \"{constantName}\"");
			}

			if (requestedType != constant.Type)
			{
				if (constant.Type == typeof(double))
				{
					if (requestedType != typeof(double) && requestedType != typeof(float))
					{
						throw new KernelSourceException($"The preprocessor definition points to an expression of type \"{Function.IntrinsicTypes[constant.Type]}\" but an expression of type \"{Function.IntrinsicTypes[requestedType]}\" was requested.");
					}
				}
				else
				{
					throw new KernelSourceException($"The preprocessor definition points to an expression of type \"{Function.IntrinsicTypes[constant.Type]}\" but an expression of type \"{Function.IntrinsicTypes[requestedType]}\" was requested.");
				}
			}

			return Expression.Parameter(constant.Type, constant.Label);
		}
	}
}