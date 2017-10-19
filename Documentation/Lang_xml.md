# XML

The files must be suffixed by `.Bind.xml`.

## Example

```xml
<Bindings Visibility="Inner" Converter="StaticBind.Sample.Views.iOS.Converter">

    <Source Class="StaticBind.Sample.ViewModels.MainViewModel">
        <Property From="Entry" To="entryField.Text" />
        <Property From="Header.Date" To="dateLabel.Text" Converter="DateToString" />
        <Property From="Header.Title" To="Title" />
        <Property From="Header.Title" To="descriptionLabel.Text" />
		<Command From="UpdateCommand" To="wholeButton" IsEnabled="Enabled" ExecuteWhen="EventHandler:TouchUpInside" />
        <Command From="Header.UpdateCommand" To="propertyButton" IsEnabled="Enabled" ExecuteWhen="EventHandler:TouchUpInside" />
    </Source>

    <Target Class="StaticBind.Sample.Views.iOS.ViewController">
        <Property From="entryField.Text" To="Entry" When="EventHandler:EditingChanged" />
    </Target>

</Bindings>
```

## Tags

### Bindings *(root)*

* **Name:** `Bindings`
* **Description:** The root element that will contain your bindings.
* **Attributes:** 
	* `Visibility` - Enum:`Inner` -  *(default:`Inner`)* - The way the code will be generated. `Inner` for being general as an inner class of a partial class.
	* `Converter` - String -  *(optional)* - The fullname of your  converter class as soon has you have referenced value converters.
* **Children:** `Target`, `Source`
* **Example**: 

```xml
<Bindings 
	Visibility="Inner"
	Converter="StaticBind.Sample.Converter"> 
</Bindings>
```

### Target

* **Name:** `Target`
* **Description:** The target object for your bindings.
* **Attributes:** 
	* `Class` - String -  *(required)* - The fullname of your class.
* **Children:** `Property`, `Command`
* **Example**: 

```xml
<Target 
	Class="StaticBind.Sample.Views.Droid.QuickStartActivity"> 
</Target>
```

### Source

* **Name:** `Source`
* **Description:** The source object for your bindings.
* **Attributes:** 
	* `Class` - String -  *(required)* - The fullname of your class.
* **Children:** `Property`, `Command`
* **Example**: 

```xml
<Source 
	Class="StaticBind.Sample.ViewModels.QuickStartViewModel"> 
</Source>
```

### Property

* **Name:** `Property`
* **Description:** A binding to a property.
* **Attributes:** 
	* `From` - String -  *(required)* - The path to your source property.
	* `To` - String -  *(required)* - The path to your target property.
	* `Converter` - String -  *(required)* - The name of your converter method if you have a converter class declared on your root tag.
	* `When` - Event -  *(required)* - The event that will trigger a value evaluation.
* **Example**: 

```xml
<Property 
	From="entryField.Text" 
	To="Date" 
	Converter="StringToDate" 
	When="System.EventHandler:TextChanged" />
```

### Command

* **Name:** `Command`
* **Description:** A binding to a command.
* **Attributes:** 
	* `From` - String -  *(required)* - The path to your source command property.
	* `To` - String -  *(required)* - The path to your target property.
	* `IsEnabled` - String -  *(required)* -  The path to your target property that is bound to the can execute state of the command.
	* `ExecuteWhen` - Event -  *(required)* - The event that will trigger a command execution.
* **Example**: 

```xml
<Command 
	From="Header.UpdateCommand" 
	To="propertyButton" 
	IsEnabled="Enabled" 
	ExecuteWhen="System.EventHandler:TouchUpInside" />
```