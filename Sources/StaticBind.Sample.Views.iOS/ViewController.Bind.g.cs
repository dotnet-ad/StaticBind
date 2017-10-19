namespace StaticBind.Sample.Views.iOS
{
	using StaticBind;

	public partial class ViewController
	{
		public Bindings<ViewModels.MainViewModel, StaticBind.Sample.Views.iOS.ViewController> Bindings { get; private set; }

		public void Bind(ViewModels.MainViewModel source, Conversions.Converter converter)
		{
			var s_root = source.CreateAccessor();
			var t_root = this.CreateAccessor();

			var s_1 = s_root.Then(x => x.Entry);
			var t_2 = t_root.Then(x => x.entryField);
			var t_3 = t_2.Then(x => x.Text);
			s_1.OnChange(v => t_3.Value = v);
			var s_4 = s_root.Then(x => x.Header);
			var s_5 = s_4.Then(x => x.Title);
			var t_6 = t_root.Then(x => x.Title);
			s_5.OnChange(v => t_6.Value = v);
			var t_7 = t_root.Then(x => x.descriptionLabel);
			var t_8 = t_7.Then(x => x.Text);
			s_5.OnChange(v => t_8.Value = v);
			var s_9 = s_4.Then(x => x.Date);
			var t_10 = t_root.Then(x => x.dateLabel);
			var t_11 = t_10.Then(x => x.Text);
			s_9.OnChange(v => t_11.Value = converter.DateToString(v));
			System.EventHandler t_h_12 = null;
			t_3.ChangeWhen((x, a) => { t_h_12 = (s, e) => a(); x.EditingChanged += t_h_12; }, (x) => x.EditingChanged -= t_h_12);
			t_3.OnChange(v => s_1.Value = v);

			this.Bindings = new Bindings<ViewModels.MainViewModel, StaticBind.Sample.Views.iOS.ViewController>(s_root, t_root);
		}
	}
}
