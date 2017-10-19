using System;
using System.Linq.Expressions;

namespace StaticBind
{
	public static class AccessorsExtensions
	{
		public static Accessor<TSource> CreateAccessor<TSource>(this TSource source)
		{
			return new Accessor<TSource>(source);
		}

		public static Accessor<TProperty, TSubProperty> Then<TSource, TProperty, TSubProperty>(this Accessor<TSource, TProperty> parent, Expression<Func<TProperty, TSubProperty>> name)
		{
			return new ChildAccessor<TSource, TProperty, TSubProperty>(parent, name);
		}
	}
}
