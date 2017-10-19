namespace StaticBind.Sample.ViewModels
{
	using Mvvmicro;

	public class QuickStartViewModel : Observable
	{
		private string title = "Initial title";

		public string Title
		{
			get => this.title;
			set => this.Set(ref this.title, value);
		}
	}
}
