using System;
using System.Text.RegularExpressions;
using JetBrains.Annotations;

namespace XyrusWorx.Runtime.Expressions
{
	[PublicAPI]
	public class Symbol
	{
		private string mName;

		public Symbol([NotNull] string name, Type type)
		{
			if (name == null)
			{
				throw new ArgumentNullException(nameof(name));
			}

			Check(name);
			mName = name;
			Type = type;
		}

		public string Name
		{
			get { return mName; }
			set
			{
				if (string.IsNullOrWhiteSpace(value))
				{
					throw new ArgumentNullException(nameof(value));
				}

				Check(value);
				mName = value;
			}
		}
		public Type Type
		{
			get;
		}

		public static void Check(string label)
		{
			label = label?.Trim() ?? string.Empty;

			if (!Regex.IsMatch(label, "^[a-z_][_a-z0-9]*$", RegexOptions.IgnoreCase))
			{
				throw new Exception(string.Format("The name \"{0}\" is invalid in the target device context.", label));
			}
		}
	}
}