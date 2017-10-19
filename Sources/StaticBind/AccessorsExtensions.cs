using System.Windows.Input;
namespace StaticBind
{
	using System;
	using System.Linq.Expressions;

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

		public static Accessor<TProperty, TSubProperty> Then<TSource, TProperty, TSubProperty>(this Accessor<TSource, TProperty> parent, string name, Func<TProperty, TSubProperty> getter, Action<TProperty,TSubProperty> setter)
		{
			return new ChildAccessor<TSource, TProperty, TSubProperty>(parent, name, getter, setter);
		}

		public static CommandAccessor<TSource, TProperty, TSubProperty> Command<TSource, TProperty, TSubProperty>(this Accessor<TSource, TProperty> parent, Expression<Func<TProperty, TSubProperty>> name) where TSubProperty : ICommand
		{
			return new CommandAccessor<TSource, TProperty, TSubProperty>(parent, name);
		}

		public static CommandAccessor<TSource, TProperty, TSubProperty> Command<TSource, TProperty, TSubProperty>(this Accessor<TSource, TProperty> parent, string name, Func<TProperty, TSubProperty> getter, Action<TProperty, TSubProperty> setter) where TSubProperty : ICommand
		{
			return new CommandAccessor<TSource, TProperty, TSubProperty>(parent, name, getter, setter);
		}
	}
}
