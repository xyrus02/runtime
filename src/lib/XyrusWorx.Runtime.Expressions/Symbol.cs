using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using JetBrains.Annotations;

namespace XyrusWorx.Runtime.Expressions 
{
	[PublicAPI]
	[DebuggerDisplay("{Key}")]
	public sealed class Symbol : IEquatable<Symbol>
	{
		public Symbol([NotNull] string label)
		{
			if (label.NormalizeNull() == null)
			{
				throw new ArgumentNullException(nameof(label));
			}

			if (!IsValid(label))
			{
				throw new CompilerException($"The symbol \"{label}\" is invalid.");
			}
			
			Key = label.Trim();
		}
		
		[NotNull]
		public string Key { get; }
		
		public static bool IsValid(string label)
		{
			if (label.NormalizeNull() == null)
			{
				return false;
			}
			
			label = label?.Trim() ?? string.Empty;

			if (!Regex.IsMatch(label, "^[a-z_][_a-z0-9]*$", RegexOptions.IgnoreCase))
			{
				return false;
			}

			return true;
		}
		
		public bool Equals(Symbol other)
		{
			if (ReferenceEquals(null, other))
			{
				return false;
			}
			
			if (ReferenceEquals(this, other))
			{
				return true;
			}
			
			return string.Equals(Key, other.Key, StringComparison.InvariantCulture);
		}
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
			{
				return false;
			}
			
			if (ReferenceEquals(this, obj))
			{
				return true;
			}
			
			return obj is Symbol symbol && Equals(symbol);
		}

		public override int GetHashCode() => StringComparer.InvariantCulture.GetHashCode(Key);
	}
}