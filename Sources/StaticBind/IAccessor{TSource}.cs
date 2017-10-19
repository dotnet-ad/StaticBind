namespace StaticBind
{
	using System;

	public interface IAccessor<TSource> : IDisposable
	{
		string Name { get; }

		TSource Source { get; set; }

		void Evaluate();
	}
}
