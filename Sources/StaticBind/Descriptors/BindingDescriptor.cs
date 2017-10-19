using System;
using System.Linq.Expressions;

namespace StaticBind.Descriptors
{
	public static class Conversion
	{
		public static Conversion<TFrom, TTo> Value<TFrom,TTo>(Func<TFrom,TTo> converter)
		{
			throw BindingDescriptor.Error;
		}
	}

	public class When
	{
		public  static When<THandler> Event<THandler>(string name)
		{
			throw BindingDescriptor.Error;
		}

		public static When<EventHandler> Event(string name)
		{
			throw BindingDescriptor.Error;
		}
	}

	public class When<THandler> 
	{
		private When() {}
	}

	public class Conversion<TFrom,TTo> 	
	{
		private Conversion() {} 
	}

	public static class BindingDescriptor
	{
		internal static Exception Error => new NotSupportedException("Descriptors should never be executed!");

		public static object Bind<T>(this object target, Expression<Func<T>> from, Expression<Func<T>> to)
		{
			throw Error;
		}

		public static object Bind<TFrom, TTo>(this object target, Expression<Func<TFrom>> from, Expression<Func<TTo>> to, Conversion<TFrom,TTo> converter)
		{
			throw Error;
		}

		public static object Bind<T,THandler>(this object target, Expression<Func<T>> from, Expression<Func<T>> to, When<THandler> when)
		{
			throw Error;
		}

		public static object Bind<TFrom, TTo, THandler>(this object target, Expression<Func<TFrom>> from, Expression<Func<TTo>> to, Conversion<TFrom, TTo> converter, When<THandler> when)
		{
			throw Error;
		}
	}
}
