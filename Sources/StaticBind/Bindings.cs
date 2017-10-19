namespace StaticBind
{
	using System;

	public class Bindings<TSource,TTarget>
	{
		public Bindings(Accessor<TSource> source, Accessor<TTarget> target)
		{
			this.source = source;
			this.target = target;
		}

		private Accessor<TSource> source;

		private Accessor<TTarget> target;

		public TSource Source => this.source.Source;

		public TTarget Target => this.target.Source;

		public bool AreActive
		{
			get => this.source.IsActive && this.target.IsActive;
			set
			{
				this.source.IsActive = value;
				this.target.IsActive = value;
			}
		}
	}
}
