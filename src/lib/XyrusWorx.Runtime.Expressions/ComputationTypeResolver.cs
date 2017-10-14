using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace XyrusWorx.Runtime.Expressions 
{
	[PublicAPI]
	public static class ComputationTypeResolver
	{
		private static readonly Dictionary<Type, ComputationType> mTypes;
		private static readonly object mTypeLock = new object();

		static ComputationTypeResolver()
		{
			mTypes = new Dictionary<Type, ComputationType>();
		}

		public static void Add<T>([NotNull] ComputationType computationType) => Add(typeof(T), computationType);
		public static void Add([NotNull] Type clrType, [NotNull] ComputationType computationType)
		{
			if (clrType == null)
			{
				throw new ArgumentNullException(nameof(clrType));
			}
			
			if (computationType == null)
			{
				throw new ArgumentNullException(nameof(computationType));
			}
			
			lock (mTypeLock)
			{
				mTypes.Add(clrType, computationType);
			}
		}

		public static ComputationType Find<T>() => Find(typeof(T));
		public static ComputationType Find([NotNull] Type clrType)
		{
			if (clrType == null)
			{
				throw new ArgumentNullException(nameof(clrType));
			}
			
			lock (mTypeLock)
			{
				if (mTypes.ContainsKey(clrType))
				{
					return mTypes[clrType];
				}
			}
			
			throw new KeyNotFoundException($"The type \"{clrType}\" can't be resolved to any computation type.");
		}
	}
}