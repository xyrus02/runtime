using System;
using System.Linq.Expressions;
using System.Reflection;
using JetBrains.Annotations;

namespace XyrusWorx.Runtime.Expressions
{
	static class ReflectionHelpers
	{
		[CanBeNull]
		public static MemberInfo GetMember<T>(Expression<Action<T>> member) => typeof(T).GetMember(member);

		[CanBeNull]
		public static MemberInfo GetMember<T>(Expression<Func<T, object>> member) => typeof(T).GetMember(member);

		[CanBeNull]
		public static FieldInfo GetField<T>(Expression<Func<T, object>> field) => typeof(T).GetMember(field) as FieldInfo;

		[CanBeNull]
		public static PropertyInfo GetProperty<T>(Expression<Func<T, object>> property) => typeof(T).GetMember(property) as PropertyInfo;

		[CanBeNull]
		public static MethodInfo GetMethod<T>(Expression<Action<T>> method) => typeof (T).GetMember(method) as MethodInfo;

		[CanBeNull]
		public static MethodInfo GetMethod<T>(Expression<Func<T, object>> method) => typeof(T).GetMember(method) as MethodInfo;
	}
}
