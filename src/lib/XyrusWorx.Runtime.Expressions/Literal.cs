using System;
using System.Diagnostics;
using JetBrains.Annotations;

namespace XyrusWorx.Runtime.Expressions 
{
	[PublicAPI]
	[DebuggerDisplay("{Value}")]
	public sealed class Literal
	{
		public Literal([NotNull] object value, ComputationType type = null)
		{
			Value = value ?? throw new ArgumentNullException(nameof(value));

			var computedType = ComputationTypeResolver.Find(value.GetType());
			if (type != null)
			{
				if (type.AcceptsValueOf(computedType))
				{
					Type = type;
				}
				else
				{
					throw new ArgumentException($"\"{value}\" is not assignable to \"{type.Key}\"");
				}
			}
			else
			{
				Type = computedType;
			}
		}
		
		[NotNull]
		public object Value { get; }
		
		[NotNull]
		public ComputationType Type { get; }
	}
}