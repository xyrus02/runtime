using System;
using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;

namespace XyrusWorx.Runtime.Expressions 
{
	[PublicAPI]
	[DebuggerDisplay("{Key}")]
	public abstract class ComputationType
	{
		internal ComputationType() {}

		[CanBeNull]
		public abstract Type CreateClrType();
		
		[NotNull]
		public abstract string Key { get; }
		public override string ToString() => Key;

		public bool AcceptsValueOf([NotNull] ComputationType other)
		{
			if (other == null)
			{
				throw new ArgumentNullException(nameof(other));
			}

			var otherDefaultValues = other.Deconstruct(other.GetDefaultValue());
			var convertResult = Construct(otherDefaultValues);

			if (convertResult.HasError)
			{
				return false;
			}

			return true;
		}

		[NotNull]
		public object Convert([NotNull] object value, [NotNull] ComputationType targetType)
		{
			var result = TryConvert(value, targetType);
			if (result.HasError)
			{
				throw new InvalidCastException(result.ErrorDescription);
			}

			return result.Data;
		}
		
		[NotNull]
		public Result<object> TryConvert([NotNull] object value, [NotNull] ComputationType targetType)
		{
			if (value == null)
			{
				throw new ArgumentNullException(nameof(value));
			}
			
			if (targetType == null)
			{
				throw new ArgumentNullException(nameof(targetType));
			}

			var sourceValues = Deconstruct(value);
			var convertResult = targetType.Construct(sourceValues);

			if (convertResult.HasError)
			{
				return Result.CreateError<Result<object>>($"Failed to convert \"{value}\" to \"{targetType.Key}\". {convertResult.ErrorDescription}".Trim());
			}

			return convertResult.Data;
		}

		[NotNull]
		protected abstract object GetDefaultValue();
		
		[NotNull]
		protected abstract Result<object> Construct([NotNull] IEnumerable<object> values);
		
		[NotNull]
		protected abstract IEnumerable<object> Deconstruct([NotNull] object value);
	}
}