using System;
using JetBrains.Annotations;

namespace XyrusWorx.Runtime.Expressions
{
	[PublicAPI]
	public abstract class Define : Symbol
	{
		private readonly object mValue;

		protected Define([NotNull] string name, [NotNull] Type type, [NotNull] object value) : base(name, type)
		{
			if (value == null)
			{
				throw new ArgumentNullException(nameof(value));
			}
			mValue = value;
		}

		public static Define<T> Create<T>([NotNull] string name, T value) where T: struct 
		{
			if (name == null) throw new ArgumentNullException(nameof(name));

			return new Define<T>(name, value);
		}

		public object GetValue() => mValue;
	}

	[PublicAPI]
	public class Define<T> : Define where T : struct
	{
		public Define([NotNull] string name, T value) : base(name, typeof(T), value)
		{
		}
	}
}