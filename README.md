![SB](./Documentation/logo.png)

Declare your data bindings and generate compiled code instead of heavily relying on reflection !

## Quickstart

**1)** Add a reference to the NuGet package to your target project.

**2)** Create a source class from which the data will be pulled (*commonly called a `ViewModel`*).

`QuickStartViewModel.cs`

```csharp
namespace StaticBind.Sample.ViewModels
{
	using System.ComponentModel;

	public class QuickStartViewModel : INotifyPropertyChanged
	{
		private string title = "Initial title";

		public event PropertyChangedEventHandler PropertyChanged;

		public string Title
		{
			get => this.title;
			set 
			{
				if(this.title != value)
				{
					this.title = value;
					this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Title)));
				}
			}
		}
	}
}
```

**3)** Create a view class which will present data to your user (*for example a ViewController on iOS or an Activity on Android*). This class can contain a set of controls the data can be bound to.

`QuickStartViewController.designer.cs` *(Xamarin.iOS, Android sample in sources)*

```csharp
using Foundation;

namespace StaticBind.Sample.Views.iOS
{
	[Register ("QuickStartViewController")]
	partial class QuickStartViewController
	{
		[Outlet]
		UIKit.UITextField entryField { get; set; }

		[Outlet]
		UIKit.UILabel titleLabel { get; set; }
		
		// ...
	}
}
```

**3)** Create a binding file associated to each view class. You can choose between **xml** or **pseudo-c#** (*see the section bellow for more details*) to declare your bindings. Make sure to have `*.Bind.xml` or `*.Bind.cs` suffix.

`QuickStartViewController.Bind.xml` *(Xamarin.iOS, Android sample in sources)*

```xml
<?xml version="1.0" encoding="UTF-8" ?>
<Bindings Visibility="Inner">

    <Source Class="StaticBind.Sample.ViewModels.QuickStartViewModel">
        <Property From="Title" To="entryField.Text" />
        <Property From="Title" To="titleLabel.Text" />
    </Source>

    <Target Class="StaticBind.Sample.Views.iOS.QuickStartViewController">
        <Property From="entryField.Text" To="Title" When="EditingChanged" />
    </Target>

</Bindings>
```

**Bonus**

You can *attach* your binding file to your view file by adding a `DependentUpon` tag inside your `.csproj`.

```xml
<None Include="QuickStartViewController.Bind.xml">
  <DependentUpon>QuickStartViewController.cs</DependentUpon>
</None>
```

**3)** Build the project to generate the binding code.

**4)** Initialize and manage bindings lifecycle.

`QuickStartViewController.cs` *(Xamarin.iOS, Android sample in sources)*

```csharp
namespace StaticBind.Sample.Views.iOS
{
	using System;
	using UIKit;
	using StaticBind.Sample.ViewModels;

	public partial class QuickStartViewController : UIViewController
	{
		public QuickStartViewController (IntPtr handle) : base (handle) {}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			this.Bind(new QuickStartViewModel());
		}

		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);

			this.Bindings.AreActive = true;
		}

		public override void ViewDidDisappear(bool animated)
		{
			base.ViewDidDisappear(animated);

			this.Bindings.AreActive = false;
		}
	}
}
```

**5)** Your data are now synced between your ViewModel and your View!

## Declarative languages

You have two options for declaring your bindings : **XML** or **Pseudo-CSharp**. I found the first to be more readable and structured, but since you don't have auto-completion, it can be error prone (*but in the end, it will not compile if you have an error in your declarations, like XAML*). The second one uses a C# like syntax (*its actually compiled but never used*) and you have auto-completion for writing your binding paths. Be careful, only the syntax of the code will be parsed, the semantic will be ignored so you have to follow specific writing rules.

### XML

The files must be suffixed by `.Bind.xml`.

[Documentation](Documentation/Lang_xml.md)

### Pseudo-CSharp (*experimental*)

*WIP*

## Generated code

All the generated code is based onto an including thin layer responsable for observing your source state. All thoses entities are available inside `StaticBind.dll` that is referenced by your project when referencing the NuGet package.

All the intermediate code is available in the intermediate output folder (generally `obj`).

## Roadmap / Ideas

* [ ] IsEnabled converter (bool -> T)
* [ ] Command parameter
* [ ] Samples
	* [X] iOS
	* [ ] Android 
* [ ] Create plugins to add auto-completion for the XML language.
	* [ ] Visual Studio for Mac 
	* [ ] Visual Studio 2017 
	* [ ] Remove Pseudo-CSharp

## Contributions

Contributions are welcome! If you find a bug please report it and if you want a feature please report it.

If you want to contribute code please file an issue and create a branch off of the current dev branch and file a pull request.

## License

MIT © [Aloïs Deniel](http://aloisdeniel.github.io)
