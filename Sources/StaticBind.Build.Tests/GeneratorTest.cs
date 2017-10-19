using NUnit.Framework;
using System;
using System.Xml.Linq;
using System.Threading.Tasks;

namespace StaticBind.Build.Tests
{
	[TestFixture()]
	public class GeneratorTest
	{

		#region Fields

		XmlParser xml;
		CsharpParser csharp;

		CSharpGenerator generator;

		#endregion

		#region Initialization

		[SetUp()]
		public void Setup()
		{
			this.xml = new XmlParser();
			this.generator = new CSharpGenerator();
			this.csharp = new CsharpParser();
		}

		#endregion

		[Test()]
		public void TestCaseXml()
		{
			using (var stream = Utils.GenerateStream(@"
<Bindings Visibility=""Inner"" Converter=""StaticBind.Sample.Views.iOS.Converter"">

    <Source Class=""StaticBind.Sample.ViewModels.MainViewModel"">
        <Bind From=""Entry"" To=""entryField.Text"" />
        <Bind From=""Header.Date"" To=""dateLabel.Text"" Converter=""DateToString"" />
        <Bind From=""Header.Title"" To=""Title"" />
        <Bind From=""Header.Title"" To=""descriptionLabel.Text"" />
    </Source>

    <Target Class=""StaticBind.Sample.Views.iOS.ViewController"">
        <Bind From=""entryField.Text"" To=""Entry"" When=""EventHandler:EditingChanged"" />
    </Target>

</Bindings>"))
			{
				var result = xml.Parse(stream);
				var csharp = generator.Generate(result);
			}
		}

		[Test()]
		public async Task TestCaseCSharp()
		{
			using (var stream = Utils.GenerateStream(@"
namespace StaticBind.Sample.Views.iOS
{
	using System;
	using StaticBind.Descriptors;

	public partial class ViewController
	{
		[Bindings]
		public void InitializeBindings(ViewModels.MainViewModel source, Conversions.Converter converter)
		{
			this.Bind(() => source.Entry, () => this.entryField.Text)
			    .Bind(() => source.Header.Title, () => this.Title)
				.Bind(() => source.Header.Title, () => this.descriptionLabel.Text)
			    .Bind(() => source.Header.Date, () => this.dateLabel.Text, Conversion.Value<DateTime,string>(converter.DateToString));
			
			this.Bind(() => this.entryField.Text, () => source.Entry, When.Event<System.EventHandler>(nameof(this.entryField.EditingChanged)));
		}
	}
}"))
			{
				var result = this.csharp.Parse(stream);
				var csharp = generator.Generate(result);
			}

		}
	}
}
