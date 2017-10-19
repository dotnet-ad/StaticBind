namespace StaticBind.Sample.Views.iOS
{
	using System;

	public class Converter
	{
		#region Default

		private static readonly Lazy<Converter> instance = new Lazy<Converter>(() => new Converter());

		public static Converter Default => instance.Value;

		#endregion

		public string DateToString(DateTime d) => d.ToString("O");

	}
}
