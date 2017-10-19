// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace StaticBind.Sample.Views.iOS
{
	[Register ("ViewController")]
	partial class ViewController
	{
		[Outlet]
		UIKit.UILabel dateLabel { get; set; }

		[Outlet]
		UIKit.UILabel descriptionLabel { get; set; }

		[Outlet]
		UIKit.UITextField entryField { get; set; }

		[Outlet]
		UIKit.UIButton propertyButton { get; set; }

		[Outlet]
		UIKit.UIButton wholeButton { get; set; }

		[Action ("onPropertyClick:")]
		partial void onPropertyClick (Foundation.NSObject sender);

		[Action ("onWholeClick:")]
		partial void onWholeClick (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (dateLabel != null) {
				dateLabel.Dispose ();
				dateLabel = null;
			}

			if (descriptionLabel != null) {
				descriptionLabel.Dispose ();
				descriptionLabel = null;
			}

			if (entryField != null) {
				entryField.Dispose ();
				entryField = null;
			}

			if (wholeButton != null) {
				wholeButton.Dispose ();
				wholeButton = null;
			}

			if (propertyButton != null) {
				propertyButton.Dispose ();
				propertyButton = null;
			}
		}
	}
}
