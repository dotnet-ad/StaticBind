namespace StaticBind.Sample.ViewModels
{
	using System.ComponentModel;

	public class QuickStartViewModel : INotifyPropertyChanged
	{
		private string title = "Initial title";

		public event PropertyChangedEventHandler PropertyChanged;

		public string Title
		{
			get => this.title;
			set 
			{
				if(this.title != value)
				{
					this.title = value;
					this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Title)));
				}
			}
		}
	}
}
