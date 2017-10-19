using System;
namespace StaticBind
{
	public interface IAccessor<TSource> : IDisposable
	{
		string Name { get; }

		TSource Source { get; set; }

		void Evaluate();
	}
}
