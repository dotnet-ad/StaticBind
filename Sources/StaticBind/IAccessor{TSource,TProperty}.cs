namespace StaticBind
{
	public interface IAccessor<TSource, TProperty> : IAccessor<TSource>
	{
		TProperty Value { get; }
	}
}
