using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace XyrusWorx.Runtime.Expressions 
{
	[PublicAPI]
	public sealed class SymbolScope : Resource
	{
		private readonly string mNamespace;
		private readonly DynamicStructureBuilder mStructureBuilder;
		private readonly Dictionary<Symbol, object> mDeclaredSymbols;
		private readonly SymbolScope mOuterScope;

		public SymbolScope() : this(null){}
		public SymbolScope(SymbolScope outerScope)
		{
			mOuterScope = outerScope;
			mNamespace = $"ns{Guid.NewGuid():N}".Substring(0, 10);
			mStructureBuilder = new DynamicStructureBuilder(mNamespace);
			mDeclaredSymbols = new Dictionary<Symbol, object>();
		}

		public bool IsDeclared([NotNull] Symbol symbol)
		{
			if (symbol == null)
			{
				throw new ArgumentNullException(nameof(symbol));
			}

			if (mDeclaredSymbols.ContainsKey(symbol))
			{
				return true;
			}

			if (mOuterScope != null && mOuterScope.IsDeclared(symbol))
			{
				return true;
			}

			return false;
		}
		
		[NotNull]
		internal T Resolve<T>([NotNull] Symbol symbol)
		{
			if (symbol == null)
			{
				throw new ArgumentNullException(nameof(symbol));
			}

			object result = null;

			if (mDeclaredSymbols.ContainsKey(symbol))
			{
				result = mDeclaredSymbols[symbol];
			}

			if (mOuterScope != null)
			{
				result = mOuterScope.Resolve<T>(symbol);
			}

			if (result == null)
			{
				throw new CompilerException($"Symbol \"{symbol}\" can't be resolved in the current scope.");
			}

			if (!(result is T t))
			{
				throw new CompilerException($"Symbol \"{symbol}\" can't be resolved as \"{typeof(T)}\" in the current scope.");
			}

			return t;
		}
		internal void Declare<T>([NotNull] Symbol symbol, [NotNull] T semantics)
		{
			if (symbol == null)
			{
				throw new ArgumentNullException(nameof(symbol));
			}
			
			if (semantics == null)
			{
				throw new ArgumentNullException(nameof(semantics));
			}

			if (mDeclaredSymbols.ContainsKey(symbol))
			{
				throw new CompilerException($"The symbol \"{symbol}\" has already been declared in the current scope.");
			}

			mDeclaredSymbols.Add(symbol, semantics);
		}

		[NotNull]
		internal DynamicStructureBuilder GetStructureBuilder() => mStructureBuilder;

		protected override void DisposeOverride()
		{
			mStructureBuilder.Dispose();
			mDeclaredSymbols.Clear();
		}
	}
}