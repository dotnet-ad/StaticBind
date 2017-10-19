namespace StaticBind.Sample.ViewModels
{
	using System;
	using Mvvmicro;

	public class MainViewModel : Observable
	{
		public MainViewModel()
		{
			this.UpdateCommand = new RelayCommand(ExecuteUpdateCommand, () => this.Header == null);
		}

		private string entry;

		private HeaderItemViewModel header;

		public HeaderItemViewModel Header 
		{
			get => this.header;
			set
			{
				var state = this.Set(ref this.header, value);
				if(state.HasChanged)
				{
					this.Entry = state.NewValue?.Title;

					if(state.OldValue != null)
						state.OldValue.PropertyChanged -= OnHeaderPropertyChanged;

					if (state.NewValue != null)
						state.NewValue.PropertyChanged += OnHeaderPropertyChanged;

					this.UpdateCommand.RaiseCanExecuteChanged();
				}
			}
		}

		private void OnHeaderPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(this.header.Title))
			{
				this.Entry = this.header.Title;
			}
		}

		public void ExecuteUpdateCommand()
		{
			this.Header = new HeaderItemViewModel()
			{
				Title = "Whole update",
				Date = DateTime.Now,
			};
		}

		public string Entry
		{
			get => this.entry;
			set 
			{
				if(this.Set(ref this.entry, value).HasChanged && this.Header != null)
				{
					this.Header.Title = value;
				}
			}
		}

		public RelayCommand UpdateCommand { get; }
	}
}
