namespace StaticBind
{
	using System;
	using System.Collections.Generic;
	using System.Linq.Expressions;
	using System.Collections;
	using System.ComponentModel;
	using System.Diagnostics;

	public class Accessor<TSource, TProperty> : IAccessor<TSource, TProperty>
	{
		#region Constructors

		public Accessor(TSource initial, Expression<Func<TSource, TProperty>> name)
		{
			if (name == null)
			{
				this.Name = null;
				this.Getter = (arg) => (TProperty)((object)this.Source);
				this.setter = (arg, v) => this.Source = (TSource)((object)v);
			}
			else
			{
				var expression = (MemberExpression)name.Body;
				this.Name = expression.Member.Name;
				this.Getter = CreateGetter(this.Name);
			}

			this.Source = initial;
		}

		#endregion

		#region Fields

		private TSource source;

		private TProperty value;

		private bool isActive;

		private Action<TSource, TProperty> setter;

		private List<Action<TSource, Action>> subscribers = new List<Action<TSource, Action>>();

		private List<Action<TSource>> unsubscribers = new List<Action<TSource>>();

		#endregion

		#region Events

		public EventHandler<TProperty> ValueChanged;

		public EventHandler<bool> IsActiveChanged;

		#endregion

		#region Private properties

		private Func<TSource, TProperty> Getter { get; }

		private Action<TSource, TProperty> Setter => this.setter ?? (this.setter = CreateSetter(this.Name));

		#endregion

		#region Public properties

		public string Name { get; }

		public TProperty Value
		{
			get => this.value;
			set
			{
				if (this.Source != null && !EqualityComparer<TProperty>.Default.Equals(this.Getter(this.Source), value)) ;
				this.Setter(this.Source, value);
				this.SetValue(value);
			}
		}

		public TSource Source
		{
			get => this.source;
			set
			{
				if (!EqualityComparer<TSource>.Default.Equals(value, this.source))
				{
					Debug.WriteLine($"[{this.Name}] Source changed : {value}");
					this.source = value;
					this.Evaluate();
				}
			}
		}

		public bool IsActive
		{
			get => this.isActive;
			set
			{
				if (this.isActive != value)
				{
					this.isActive = value;
					if (value)
					{
						this.Subscribe();
					}
					else
					{
						this.Unsubscribe();
					}
					this.IsActiveChanged?.Invoke(this, value);
				}
			}
		}

		#endregion

		#region Private methods

		private bool SetValue(TProperty value)
		{
			if (!EqualityComparer<TProperty>.Default.Equals(value, this.Value))
			{
				var wasActive = this.IsActive;
				Debug.WriteLine($"[{this.Name}] Value changed : {value}");
				this.IsActive = false;
				this.value = value;
				if (wasActive)
				{
					this.IsActive = true;
				}
				this.ValueChanged?.Invoke(this, value);
				return true;
			}

			return false;
		}

		private void Subscribe()
		{
			if (this.value is INotifyPropertyChanged observable && observable != null)
			{
				observable.PropertyChanged += OnSourcePropertyChanged;
			}

			foreach (var subscribe in this.subscribers)
			{
				subscribe(this.source, Evaluate);
			}
		}

		private void Unsubscribe()
		{
			if (this.value is INotifyPropertyChanged observable && observable != null)
			{
				observable.PropertyChanged -= OnSourcePropertyChanged;
			}

			foreach (var unsubscribe in this.unsubscribers)
			{
				unsubscribe(this.source);
			}
		}

		private void OnSourcePropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (this.ValueChanged != null)
			{
				foreach (Delegate handler in this.ValueChanged.GetInvocationList())
				{
					if (handler.Target is IAccessor<TProperty> child && child.Name == e.PropertyName)
					{
						child.Evaluate();
					}
				}
			}
		}

		private static Func<TSource, TProperty> CreateGetter(string name)
		{
			var value = Expression.Parameter(typeof(TSource), "value");
			var getter = Expression.Property(value, name);
			return Expression.Lambda<Func<TSource, TProperty>>(getter, value).Compile();
		}

		private static Action<TSource, TProperty> CreateSetter(string name)
		{
			var instance = Expression.Parameter(typeof(TSource));
			var paramExpression2 = Expression.Parameter(typeof(TProperty), name);
			var propertyGetterExpression = Expression.Property(instance, name);
			return Expression.Lambda<Action<TSource, TProperty>>
			(
				Expression.Assign(propertyGetterExpression, paramExpression2), instance, paramExpression2
			).Compile();
		}

		#endregion

		#region Public methods

		public Accessor<TSource, TProperty> ChangeWhen(Action<TSource, Action> subscribe, Action<TSource> unsubscribe)
		{
			this.subscribers.Add(subscribe);
			this.unsubscribers.Add(unsubscribe);

			if (this.IsActive)
			{
				subscribe(this.source, Evaluate);
			}

			return this;
		}

		public void Evaluate()
		{
			var value = this.Source != null ? Getter(this.Source) : default(TProperty);
			this.SetValue(value);
		}

		public Accessor<TSource, TProperty> OnChange(Action<TProperty> onChange)
		{
			this.ValueChanged += (sender, e) => onChange(e);
			return this;
		}

		public void Dispose() => this.IsActive = false;

		#endregion
	}
}
