namespace StaticBind
{
	using System;
	using System.Windows.Input;
	using System.Collections.Generic;

	public class CommandTrigger<TSource, TCommandProperty, TTarget, TTargetProperty> where TCommandProperty : ICommand
	{
		public CommandTrigger(Accessor<TSource, TCommandProperty> commandAccessor, Accessor<TTarget, TTargetProperty> targetAccessor)
		{
			this.commandAccessor = commandAccessor;
			this.targetAccessor = targetAccessor;

			this.commandAccessor.ValueChanged += OnCommandChanged;
			this.commandAccessor.IsActiveChanged += OnCommandActive;
			this.targetAccessor.IsActiveChanged += OnTargetActive;
		}

		private Accessor<TSource, TCommandProperty> commandAccessor;

		private Accessor<TTarget, TTargetProperty> targetAccessor;

		#region CanExecute

		private TCommandProperty command;

		public EventHandler<bool> CanExecuteChanged;

		private void OnCommandActive(object sender, bool isActive)
		{
			this.UnsubsribeCommand();

			if (isActive)
			{
				this.SubsribeCommand();
			}
		}

		private void SubsribeCommand()
		{
			if (this.commandAccessor.IsActive)
			{
				this.command = this.commandAccessor.Value;
				if(this.command != null)
					this.command.CanExecuteChanged += OnCanExecuteChanged;
				this.OnCanExecuteChanged(this, EventArgs.Empty);
			}
		}

		private void UnsubsribeCommand()
		{
			if (command != null)
			{
				this.command.CanExecuteChanged -= OnCanExecuteChanged;
				this.command = default(TCommandProperty);
			}
		}

		private void OnCanExecuteChanged(object sender, EventArgs e)
		{
			this.CanExecuteChanged?.Invoke(this, this.command?.CanExecute(null) ?? false);
		}

		public virtual CommandTrigger<TSource, TCommandProperty, TTarget, TTargetProperty> OnCanExecuteChange(Action<bool> onChange)
		{
			this.CanExecuteChanged += (sender, e) => onChange(e);
			return this;
		}

		private void OnCommandChanged(object sender, TCommandProperty e) => this.SubsribeCommand();

		#endregion

		#region Execute

		private TTargetProperty targetValue;

		protected List<Action<TTargetProperty, Action>> subscribers = new List<Action<TTargetProperty, Action>>();

		protected List<Action<TTargetProperty>> unsubscribers = new List<Action<TTargetProperty>>();

		public void ExecuteWhen(Action<TTargetProperty,Action> subscribe, Action<TTargetProperty> unsubscribe)
		{
			subscribers.Add(subscribe);
			unsubscribers.Add(unsubscribe);

			if (this.targetAccessor.IsActive && this.targetAccessor.Value != null)
			{
				subscribe(this.targetAccessor.Value, Execute);
			}
		}

		private void SubsribeTargetEvents()
		{
			if (this.targetAccessor.IsActive)
			{
				this.targetValue = this.targetAccessor.Value;
				if (this.targetValue != null)
				{
					foreach (var subscribe in this.subscribers)
					{
						subscribe(this.targetValue, Execute);
					}
				}
			}
		}

		private void UnsubsribeTargetEvents()
		{
			if (targetValue != null)
			{
				foreach (var unsubscribe in this.unsubscribers)
				{
					unsubscribe(this.targetValue);
				}

				this.targetValue = default(TTargetProperty);
			}
		}

		private void OnTargetActive(object sender, bool isActive)
		{
			this.UnsubsribeTargetEvents();

			if (isActive)
			{
				this.SubsribeTargetEvents();
			}
		}

		private void OnTargetValueChanged(object sender, TTargetProperty e)
		{
			if (!EqualityComparer<TTargetProperty>.Default.Equals(this.targetValue, e))
			{
				this.SubsribeCommand();
			}
		}

		public void Execute() => this.command?.Execute(null);

		#endregion

	}
}
