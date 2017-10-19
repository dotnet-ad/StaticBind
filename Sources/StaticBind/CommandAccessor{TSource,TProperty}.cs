namespace StaticBind
{
	using System;
	using System.Linq.Expressions;
	using System.Windows.Input;

	public class CommandAccessor<TSource, TProperty, TSubProperty> : ChildAccessor<TSource, TProperty, TSubProperty> where TSubProperty : ICommand
	{
		public CommandAccessor(Accessor<TSource, TProperty> parent, Expression<Func<TProperty, TSubProperty>> name) : base(parent, name)
		{
		}

		public CommandAccessor(Accessor<TSource, TProperty> parent, string name, Func<TProperty, TSubProperty> getter, Action<TProperty, TSubProperty> setter) : base(parent, name, getter, setter)
		{
		}

		public EventHandler<bool> CanExecuteChanged;

		protected override void Subscribe()
		{
			base.Subscribe();

			if(this.Value != null)
			{
				this.Value.CanExecuteChanged += OnCanExecuteChanged;
			}
		}

		protected override void Unsubscribe()
		{
			base.Unsubscribe();

			if (this.Value != null)
			{
				this.Value.CanExecuteChanged -= OnCanExecuteChanged;
			}
		}

		private void OnCanExecuteChanged(object sender, EventArgs e)
		{
			this.CanExecuteChanged?.Invoke(this, this.Value?.CanExecute(null) ?? false);
		}

		public virtual CommandAccessor<TSource, TProperty, TSubProperty> OnCanExecuteChange(Action<bool> onChange)
		{
			this.CanExecuteChanged += (sender, e) => onChange(e);
			return this;
		}
	}
}
