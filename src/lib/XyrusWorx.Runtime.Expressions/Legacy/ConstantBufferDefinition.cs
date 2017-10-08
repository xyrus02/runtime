using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using JetBrains.Annotations;

namespace XyrusWorx.Runtime.Expressions
{
	[PublicAPI]
	public class ConstantBufferDefinition
	{
		private readonly string mName;
		private readonly List<Declaration> mSymbols;

		public ConstantBufferDefinition([NotNull] string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException(nameof(name));
			}

			Declaration.IsValidSymbolLabel(name);

			mName = name;
			mSymbols = new List<Declaration>();
		}

		public void Add([NotNull] Declaration declaration)
		{
			if (declaration == null)
			{
				throw new ArgumentNullException(nameof(declaration));
			}

			Declaration.IsValidSymbolLabel(declaration.Label);
			Function.CheckTypeSupport(declaration.Type);

			mSymbols.Add(declaration);
		}
		public void Add<T>(Expression<Func<T, object>> field) where T: struct
		{
			var fieldInfo = ReflectionHelpers.GetField(field);
			if (fieldInfo == null)
			{
				throw new ArgumentException(string.Format("Expression does not represent a field: {0}", field), nameof(field));
			}

			Declaration.IsValidSymbolLabel(fieldInfo.Name);
			Function.CheckTypeSupport(fieldInfo.FieldType);

			Add(new Declaration(fieldInfo.Name, fieldInfo.FieldType));
		}

		public string Name => mName;
		public IEnumerable<Declaration> Symbols => mSymbols.Select(x => x);

		public void Write(StringBuilder builder, IKernelWriterContext context)
		{
			builder.AppendLine($"cbuffer {mName}");
			builder.AppendLine("{");

			foreach (var symbol in mSymbols)
			{
				builder.Append($"\t{Function.IntrinsicTypes[symbol.Type]} {symbol.Label}");
				builder.AppendLine(";");
			}

			builder.AppendLine("};" + Environment.NewLine);
		}
	}
}