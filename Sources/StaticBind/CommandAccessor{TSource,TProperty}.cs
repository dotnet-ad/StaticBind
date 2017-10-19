namespace StaticBind
{
	using System;
	using System.Linq.Expressions;
	using System.Windows.Input;

	public class CommandAccessor<TSource, TProperty> : Accessor<TSource, TProperty> where TProperty : ICommand
	{
		public CommandAccessor(TSource initial, Expression<Func<TSource, TProperty>> name) : base(initial, name)
		{
		}

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
			
		}

		public Accessor<TSource, TProperty> OnChange(Action<TProperty> onChange)
		{
			this.ValueChanged += (sender, e) => onChange(e);
			return this;
		}
	}
}
