namespace StaticBind.Sample.Views.iOS
{
	using System;
	using StaticBind.Descriptors;

	public partial class ViewController
	{
		[Bindings]
		public void InitializeBindings(ViewModels.MainViewModel source, Conversions.Converter converter)
		{
			this.Bind(() => source.Entry, () => this.entryField.Text)
			    .Bind(() => source.Header.Title, () => this.Title)
				.Bind(() => source.Header.Title, () => this.descriptionLabel.Text)
			    .Bind(() => source.Header.Date, () => this.dateLabel.Text, Conversion.Value<DateTime,string>(converter.DateToString));
			
			this.Bind(() => this.entryField.Text, () => source.Entry, When.Event<System.EventHandler>(nameof(this.entryField.EditingChanged)));
		}
	}
}