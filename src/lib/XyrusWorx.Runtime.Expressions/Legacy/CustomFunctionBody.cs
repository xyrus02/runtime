using System;
using System.Text;
using JetBrains.Annotations;
using XyrusWorx.Collections;

namespace XyrusWorx.Runtime.Expressions
{
	[PublicAPI]
	public class CustomFunctionBody : FunctionBody
	{
		private string mContent;

		public CustomFunctionBody(string content, params Declaration[] parameters) : this(content, null, parameters)
		{
		}
		public CustomFunctionBody(string content = null, Type returnType = null, params Declaration[] parameters) : base(returnType)
		{
			Content = content;
			parameters?.Foreach(x => Parameters.Add(x));
		}

		public string Content
		{
			get { return mContent; }
			set
			{
				if (Equals(value, mContent))
				{
					return;
				}

				mContent = value;
			}
		}

		public override void Write(StringBuilder builder, IKernelWriterContext context)
		{
			builder.Append(mContent);
		}
	}

	[PublicAPI]
	public class CustomFunctionBody<T> : CustomFunctionBody where T : struct
	{
		public CustomFunctionBody(params Declaration[] parameters) : this(null, parameters)
		{
		}
		public CustomFunctionBody(string content = null, params Declaration[] parameters) : base(content, typeof(T), parameters) { }
	}
}