# XML

The files must be suffixed by `.Bind.xml`.

##### Example :

```xml
<Bindings Visibility="Inner" Converter="StaticBind.Sample.Views.iOS.Converter">

    <Source Class="StaticBind.Sample.ViewModels.ViewModel">
        <Bind From="Title" To="entryField.Text" />
        <Bind From="Header.Date" To="dateLabel.Text" Converter="DateToString" />
        <Bind From="Title" To="titleLabel.Text" />
    </Source>

    <Target Class="StaticBind.Sample.Views.iOS.ViewController">
        <Bind From="entryField.Text" To="Title" When="EventHandler:EditingChanged" />
    </Target>

</Bindings>
```

