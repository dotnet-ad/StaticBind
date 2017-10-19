namespace StaticBind.Sample.Views.iOS
{
	using System;
	using UIKit;
	using StaticBind.Sample.ViewModels;
	using Conversions;

	public partial class ViewController : UIViewController
	{
		protected ViewController(IntPtr handle) : base(handle)
		{
		}

		partial void onWholeClick(Foundation.NSObject sender)
		{
			this.Bindings.Source.UpdateWhole.Execute(null);
		}

		partial void onPropertyClick(Foundation.NSObject sender)
		{
			this.Bindings.Source.UpdateProperties.Execute(null);
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
