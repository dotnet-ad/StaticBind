namespace StaticBind
{
	using System;
	using System.Collections.Generic;
	using System.Linq.Expressions;
	using System.ComponentModel;
	using System.Diagnostics;

	/// <summary>
	/// An accessor to a property from a source. It stops evaluation when the source is null instead of crashing. It also
	/// auto evaluates its value when the source implements INotifyPropertyChanged and raises a change on the associated
	/// property name.
	/// </summary>
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

		public Accessor(TSource initial, string name, Func<TSource, TProperty> getter, Action<TSource, TProperty> setter)
		{
			this.Name = name;
			this.Getter = getter;
			this.setter = setter;
			this.Source = initial;
		}

		#endregion

		#region Fields

		private TSource source;

		private TProperty value;

		private bool isActive;

		private Action<TSource, TProperty> setter;

		protected List<Action<TSource, Action>> subscribers = new List<Action<TSource, Action>>();

		protected List<Action<TSource>> unsubscribers = new List<Action<TSource>>();

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

		public bool HasSource => !EqualityComparer<TSource>.Default.Equals(this.Source, default(TSource));

		public TProperty Value
		{
			get => this.value;
			set
			{
				if (this.HasSource && !EqualityComparer<TProperty>.Default.Equals(this.Getter(this.Source), value))
				{
					this.Setter(this.Source, value);
				}

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

		/// <summary>
		/// Gets or sets a value indicating whether this instance is active. This property must be updated to unregister
		/// events whenever it is possible.
		/// </summary>
		/// <value><c>true</c> if is active; otherwise, <c>false</c>.</value>
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

		private bool SetValue(TProperty v)
		{
			if (!EqualityComparer<TProperty>.Default.Equals(v, this.Value))
			{
				var wasActive = this.IsActive;
				Debug.WriteLine($"[{this.Name}] Value changed : {v}");
				this.IsActive = false;
				this.value = v;
				if (wasActive)
				{
					this.IsActive = true;
				}
				this.ValueChanged?.Invoke(this, v);
				return true;
			}

			return false;
		}

		protected virtual void Subscribe()
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

		protected virtual void Unsubscribe()
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

		/// <summary>
		/// Subscribe an event to indicate when to evaluate the value.
		/// </summary>
		/// <returns>The when.</returns>
		/// <param name="subscribe">Subscribe.</param>
		/// <param name="unsubscribe">Unsubscribe.</param>
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

		/// <summary>
		/// Evaluate this value from the source value.
		/// </summary>
		public void Evaluate()
		{
			var v = this.HasSource ? Getter(this.Source) : default(TProperty);
			this.SetValue(v);
		}

		/// <summary>
		/// Registers an action that will be executed each time the value changes.
		/// </summary>
		/// <returns>The change.</returns>
		/// <param name="onChange">On change.</param>
		public virtual Accessor<TSource, TProperty> OnChange(Action<TProperty> onChange)
		{
			this.ValueChanged += (sender, e) => onChange(e);
			return this;
		}

		public void Dispose() => this.IsActive = false;

		#endregion
	}
}
