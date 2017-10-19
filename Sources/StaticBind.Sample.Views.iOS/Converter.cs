using System;
namespace StaticBind.Sample.Views.iOS
{
	public class Converter
	{
		#region Static

		private static Lazy<Converter> instance = new Lazy<Converter>(() => new Converter());

		public static Converter Default => instance.Value;

		#endregion

		public string DateToString(DateTime v) => v.ToString();
	}
}