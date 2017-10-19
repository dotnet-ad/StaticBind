using Android.Widget;
namespace StaticBind.Sample.Views.Droid
{
	using Android.App;
	using Android.OS;
	using StaticBind.Sample.ViewModels;

	[Activity(Label = "QuickStartActivity")]
	public partial class QuickStartActivity : Activity
	{
		private TextView titleLabel;

		private EditText entryField;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			SetContentView(Resource.Layout.Quickstart);

			this.titleLabel = FindViewById<TextView>(Resource.Id.titleLabel);
			this.entryField = FindViewById<EditText>(Resource.Id.entryField);

			this.Bind(new QuickStartViewModel());
		}

		protected override void OnResume()
		{
			base.OnResume();

			this.Bindings.AreActive = true;
		}

		protected override void OnPause()
		{
			base.OnPause();

			this.Bindings.AreActive = false;
		}
	}
}
