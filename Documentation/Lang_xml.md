# Pseudo-CSharp (*experimental*)

The files must be suffixed by `.Bind.cs`.

#### Base structure

First, you should follow this base template :

```csharp
namespace <View namespace>
{
	using System;
	using StaticBind.Descriptors;

	public partial class <View class name>
	{
		[Bindings]
		public void DeclareBindings(<Source class full name> source)
		{
			<Bindings>
		}
	}
}
```

