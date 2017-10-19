using System;
using System.Linq.Expressions;

namespace StaticBind.Descriptors
{
	/** 
	 *   [Bindings]
	 *   private object BindingDescriptors => 
	 * 		this.CreateBindings()
	 * 			.Source<ViewModels.ViewModel>()
	 * 				.Property(vm => vm.Title, v => v.label.Text)
	 * 			.Target()
	 * 				.Property(vm => v.field.Text, v => v.title, When<System.EventHandler>(nameof(EditingChanged));
	 *   
	 * 
	 */

	public class Target<TTo>
	{
		public Source<TFrom, TTo> Source<TFrom>()
			=> throw DescriptorExtensions.Error;
	}

	public class Source<TFrom,TTo>
	{
		public Source<TFrom, TTo> Property<T>(Expression<Func<TFrom,T>> from, Expression<Func<TTo,T>> to) 
			=> throw DescriptorExtensions.Error;

		public Source<TFrom, TTo> Property<T, THandler>(Expression<Func<TFrom,T>> from, Expression<Func<TTo,T>> to, When<THandler> when) 
			=> throw DescriptorExtensions.Error;

		public Source<TFrom, TTo> Property<TFromValue,TToValue>(Expression<Func<TFrom,TFromValue>> from, Expression<Func<TTo,TToValue>> to, Conversion<TFromValue,TToValue> converter)
			=> throw DescriptorExtensions.Error;

		public Source<TFrom, TTo> Property<TFromValue, TToValue, THandler>(Expression<Func<TFrom, TFromValue>> from, Expression<Func<TTo, TToValue>> to, Conversion<TFromValue, TToValue> converter, When<THandler> when)
			=> throw DescriptorExtensions.Error;

		public Source<TTo, TFrom> Target() 
			=> throw DescriptorExtensions.Error;
	}

	public static class DescriptorExtensions
	{
		internal static Exception Error => new NotSupportedException("Descriptors should never be executed!");

		public static Target<TTo> CreateBindings<TTo>(this TTo target) 
			=> throw Error;
	}


	public static class Conversion
	{
		public static Conversion<TFrom, TTo> Value<TFrom,TTo>(Func<TFrom,TTo> converter)
		{
			throw DescriptorExtensions.Error;
		}
	}

	public class When
	{
		public  static When<THandler> Event<THandler>(string name)
			 => throw DescriptorExtensions.Error;

		public static When<EventHandler> Event(string name) 
			=> throw DescriptorExtensions.Error;
	}

	public class When<THandler> 
	{
		private When() {}
	}

	public class Conversion<TFrom,TTo> 	
	{
		private Conversion() {} 
	}
}
