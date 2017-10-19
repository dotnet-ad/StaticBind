namespace StaticBind
{
	using System;
	using System.Linq.Expressions;

	public class ChildAccessor<TSource, TProperty, TSubProperty> : Accessor<TProperty, TSubProperty>
	{
		public ChildAccessor(Accessor<TSource, TProperty> parent, Expression<Func<TProperty, TSubProperty>> name) : base(parent.Value, name)
		{
			parent.ValueChanged += OnParentValueChanged;
			parent.IsActiveChanged += OnParentIsActiveChanged;
		}

		public ChildAccessor(Accessor<TSource, TProperty> parent, string name, Func<TProperty, TSubProperty> getter, Action<TProperty, TSubProperty> setter) : base(parent.Value, name, getter, setter)
		{
			parent.ValueChanged += OnParentValueChanged;
			parent.IsActiveChanged += OnParentIsActiveChanged;
		}

		private void OnParentValueChanged(object sender, TProperty value)
		{
			this.Source = value;
		}

		private void OnParentIsActiveChanged(object sender, bool value)
		{
			this.IsActive = value;
		}
	}
}
