using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using JetBrains.Annotations;
using XyrusWorx.Collections;

namespace XyrusWorx.Runtime.Expressions
{
	[PublicAPI]
	public class Function : Declaration
	{
		private static Dictionary<Type, string> mIntrinsicTypes;
		private FunctionBody mBody;

		public Function([NotNull] string label, [NotNull] FunctionBody body) : base(label, body.Type)
		{
			if (body == null)
			{
				throw new ArgumentNullException(nameof(body));
			}

			mBody = body;
		}

		[NotNull]
		public FunctionBody Body => mBody;

		public virtual void Write(StringBuilder builder, IKernelWriterContext context)
		{
			WriteFunction(builder, Label, Type, Body.Parameters.Select(x => Expression.Parameter(x.Type, x.Label)));

			var body = new StringBuilder();
			Body.Write(body, context);

			builder.AppendLine(Environment.NewLine + "{");
			builder.AppendLine(string.Join(Environment.NewLine, body.ToString().Trim().Split(new[] { Environment.NewLine }, StringSplitOptions.None).Select(x => $"\t{x}")));
			builder.AppendLine("}" + Environment.NewLine);
		}

		public static IReadOnlyDictionary<Type, string> IntrinsicTypes => mIntrinsicTypes;
		public static void CheckTypeSupport(Type type)
		{
			if (mIntrinsicTypes == null)
			{
				mIntrinsicTypes = new Dictionary<Type, string>
				{
					{typeof (bool), "bool"},
					{typeof (Vector2<bool>), "bool2"},
					{typeof (Vector3<bool>), "bool3"},
					{typeof (Vector4<bool>), "bool4"},
					{typeof (float), "float"},
					{typeof (double), "float"},
					{typeof (Float2), "float2"},
					{typeof (Float3), "float3"},
					{typeof (Float4), "float4"},
					{typeof (Vector2<float>), "float2"},
					{typeof (Vector3<float>), "float3"},
					{typeof (Vector4<float>), "float4"},
					{typeof (int), "int"},
					{typeof (Int2), "int2"},
					{typeof (Int3), "int3"},
					{typeof (Int4), "int4"},
					{typeof (Vector2<int>), "int2"},
					{typeof (Vector3<int>), "int3"},
					{typeof (Vector4<int>), "int4"},
					{typeof (uint), "uint"},
					{typeof (Vector2<uint>), "uint2"},
					{typeof (Vector3<uint>), "uint3"},
					{typeof (Vector4<uint>), "uint4"},
					{typeof (Float3x2), "float3x2"},
					{typeof (Float2x2), "float2x2"},
					{typeof (Float3x3), "float3x3"},
					{typeof (Float4x4), "float4x4"},
					{typeof (Matrix2x2<float>), "float2x2"},
					{typeof (Matrix3x3<float>), "float3x3"},
					{typeof (Matrix4x4<float>), "float4x4"},
					{typeof (Matrix2x2<int>), "int2x2"},
					{typeof (Matrix3x3<int>), "int3x3"},
					{typeof (Matrix4x4<int>), "int4x4"}
				};

			}

			if (!mIntrinsicTypes.ContainsKey(type))
			{
				throw new Exception(string.Format("The type \"{0}\" is not a supported constant type.", type.FullName));
			}
		}

		public static void WriteFunction(StringBuilder target, string name, Type returnType, IEnumerable<ParameterExpression> parameters)
		{
			IsValidSymbolLabel(name);

			if (returnType != null)
			{
				CheckTypeSupport(returnType);
			}

			target.Append($"{(returnType == null ? "void" : IntrinsicTypes[returnType])} {name}(");

			var paramArray = parameters.AsArray();
			if (paramArray.Length > 0)
			{
				for (var i = 0; i < paramArray.Length - 1; i++)
				{
					WriteParameterDeclaration(target, paramArray[i]);
					target.Append(", ");
				}

				WriteParameterDeclaration(target, paramArray[paramArray.Length - 1]);
			}

			target.AppendFormat(")");
		}
		public static void WriteParameterDeclaration(StringBuilder target, ParameterExpression expression)
		{
			IsValidSymbolLabel(expression.Name);
			CheckTypeSupport(expression.Type);

			target.Append($"{IntrinsicTypes[expression.Type]} {expression.Name}");
		}
	}
}