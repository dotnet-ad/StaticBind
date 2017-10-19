namespace StaticBind.Sample.Views.iOS
{
	using System;
	using StaticBind;

	public partial class ViewController
	{
		public Bindings<StaticBind.Sample.ViewModels.MainViewModel, StaticBind.Sample.Views.iOS.ViewController> Bindings { get; private set; }

		public void Bind(StaticBind.Sample.ViewModels.MainViewModel source, StaticBind.Sample.Views.iOS.Converter converter)
		{
			var s_root = source.CreateAccessor();
			var t_root = this.CreateAccessor();

			var s_1 = s_root.Then(x => x.Entry);
			var t_2 = t_root.Then(x => x.entryField);
			var t_3 = t_2.Then(x => x.Text);
			s_1.OnChange(v => t_3.Value = v);
			var s_4 = s_root.Then(x => x.Header);
			var s_5 = s_4.Then(x => x.Date);
			var t_6 = t_root.Then(x => x.dateLabel);
			var t_7 = t_6.Then(x => x.Text);
			s_5.OnChange(v => t_7.Value = converter.DateToString(v));
			var s_8 = s_4.Then(x => x.Title);
			var t_9 = t_root.Then(x => x.Title);
			s_8.OnChange(v => t_9.Value = v);
			var t_10 = t_root.Then(x => x.descriptionLabel);
			var t_11 = t_10.Then(x => x.Text);
			s_8.OnChange(v => t_11.Value = v);
			EventHandler t_h_12 = null;
			t_3.ChangeWhen((x, a) => { t_h_12 = (s, e) => a(); x.EditingChanged += t_h_12; }, (x) => x.EditingChanged -= t_h_12);
			t_3.OnChange(v => s_1.Value = v);

			this.Bindings = new Bindings<StaticBind.Sample.ViewModels.MainViewModel, StaticBind.Sample.Views.iOS.ViewController>(s_root, t_root);
		}
	}
}
