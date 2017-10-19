namespace StaticBind
{
	public class Accessor<TSource> : Accessor<TSource, TSource>
	{
		public Accessor(TSource initial) : base(initial, null)
		{
		}
	}
}
