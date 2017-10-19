namespace StaticBind.Sample.Views.iOS
{
	using System;
	using UIKit;
	using StaticBind.Sample.ViewModels;

	public static partial class Converters
	{
	}

	public partial class ViewController : UIViewController
	{
		protected ViewController(IntPtr handle) : base(handle)
		{
		}

		partial void onWholeClick(Foundation.NSObject sender)
		{
			this.Bindings.Source.UpdateWhole();
		}

		partial void onPropertyClick(Foundation.NSObject sender)
		{
			this.Bindings.Source.UpdateProperties();
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			this.Bind(new MainViewModel(), Converter.Default);
		}

		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);

			this.Bindings.IsActive = true;
		}

		public override void ViewDidDisappear(bool animated)
		{
			base.ViewDidDisappear(animated);

			this.Bindings.IsActive = false;
		}

		public override void DidReceiveMemoryWarning()
		{
			base.DidReceiveMemoryWarning();

		}
	}
}
