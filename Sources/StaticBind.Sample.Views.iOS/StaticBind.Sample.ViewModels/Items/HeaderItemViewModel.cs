namespace StaticBind.Sample.ViewModels
{
	using System;
	using Mvvmicro;

	public class HeaderItemViewModel : Observable
	{
		public HeaderItemViewModel()
		{
			this.UpdateCommand = new RelayCommand(ExecuteUpdateCommand);
		}

		private string title;

		private DateTime date;

		public string Title
		{
			get => this.title;
			set => this.Set(ref this.title, value);
		}

		public DateTime Date
		{
			get => this.date;
			set => this.Set(ref this.date, value);
		}

		public RelayCommand UpdateCommand { get; }

		public void ExecuteUpdateCommand()
		{
			this.Title = "Property update";
			this.Date = DateTime.Now;
		}
	}
}
