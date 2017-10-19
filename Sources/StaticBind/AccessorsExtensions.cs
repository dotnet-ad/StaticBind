using System.Windows.Input;
namespace StaticBind
{
	using System;
	using System.Linq.Expressions;

	public static class AccessorsExtensions
	{
		public static Accessor<TSource> CreateAccessor<TSource>(this TSource source)
		{
			return new Accessor<TSource>();
		}

		public static Accessor<TProperty, TSubProperty> Property<TSource, TProperty, TSubProperty>(this Accessor<TSource, TProperty> parent, Expression<Func<TProperty, TSubProperty>> name)
		{
			return new ChildAccessor<TSource, TProperty, TSubProperty>(parent, name);
		}

		public static Accessor<TProperty, TSubProperty> Property<TSource, TProperty, TSubProperty>(this Accessor<TSource, TProperty> parent, string name, Func<TProperty, TSubProperty> getter, Action<TProperty,TSubProperty> setter = null)
		{
			return new ChildAccessor<TSource, TProperty, TSubProperty>(parent, name, getter, setter);
		}

		public static CommandTrigger<TSource, TCommandProperty, TTarget, TTargetProperty> Command<TSource, TCommandProperty, TTarget, TTargetProperty>(this Accessor<TSource, TCommandProperty> parent, Accessor<TTarget,TTargetProperty> target) where TCommandProperty : ICommand
		{
			return new CommandTrigger<TSource, TCommandProperty, TTarget, TTargetProperty>(parent, target);
		}
	}
}
