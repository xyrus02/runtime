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
		private readonly List<Symbol> mSymbols;

		public ConstantBufferDefinition([NotNull] string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException(nameof(name));
			}

			Symbol.Check(name);

			mName = name;
			mSymbols = new List<Symbol>();
		}

		public void Add([NotNull] Symbol symbol)
		{
			if (symbol == null)
			{
				throw new ArgumentNullException(nameof(symbol));
			}

			Symbol.Check(symbol.Name);
			Function.CheckTypeSupport(symbol.Type);

			mSymbols.Add(symbol);
		}
		public void Add<T>(Expression<Func<T, object>> field) where T: struct
		{
			var fieldInfo = ReflectionHelpers.GetField(field);
			if (fieldInfo == null)
			{
				throw new ArgumentException(string.Format("Expression does not represent a field: {0}", field), nameof(field));
			}

			Symbol.Check(fieldInfo.Name);
			Function.CheckTypeSupport(fieldInfo.FieldType);

			Add(new Symbol(fieldInfo.Name, fieldInfo.FieldType));
		}

		public string Name => mName;
		public IEnumerable<Symbol> Symbols => mSymbols.Select(x => x);

		public void Write(StringBuilder builder, IKernelWriterContext context)
		{
			builder.AppendLine($"cbuffer {mName}");
			builder.AppendLine("{");

			foreach (var symbol in mSymbols)
			{
				builder.Append($"\t{Function.IntrinsicTypes[symbol.Type]} {symbol.Name}");
				builder.AppendLine(";");
			}

			builder.AppendLine("};" + Environment.NewLine);
		}
	}
}