using Android.App;
using Android.Widget;
using Android.OS;

namespace StaticBind.Sample.Views.Droid
{
	[Activity(Label = "StaticBind", MainLauncher = true, Icon = "@mipmap/icon")]
	public class MainActivity : Activity
	{
		private Button quickstartButton;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			// Set our view from the "main" layout resource
			SetContentView(Resource.Layout.Main);

			this.quickstartButton = FindViewById<Button>(Resource.Id.quickstartButton);
			this.quickstartButton.Click += (s, e) => this.StartActivity(typeof(QuickStartActivity));
		}
	}
}

