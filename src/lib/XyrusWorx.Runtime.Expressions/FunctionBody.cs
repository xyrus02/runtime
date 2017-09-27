using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text;
using JetBrains.Annotations;

namespace XyrusWorx.Runtime.Expressions
{
	[PublicAPI]
	public abstract class FunctionBody
	{
		protected FunctionBody(Type returnType)
		{
			if (returnType != null)
			{
				Function.CheckTypeSupport(returnType);
			}

			var collection = new ObservableCollection<Symbol>();

			collection.CollectionChanged += OnParameterCollectionChanged;
			Parameters = collection;

			Type = returnType;
		}

		public IList<Symbol> Parameters { get; }
		public Type Type { get; }

		protected virtual void InvalidateSignatureOverride()
		{
		}

		public abstract void Write(StringBuilder builder, IKernelWriterContext context);

		private void OnParameterCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			InvalidateSignatureOverride();
		}
	}
}