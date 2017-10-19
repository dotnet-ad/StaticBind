using NUnit.Framework;
using System;
using System.Xml.Linq;
namespace StaticBind.Build.Tests
{
	[TestFixture()]
	public class GeneratorTest
	{

		#region Fields

		Parser parser;

		BindingsGenerator generator;

		#endregion

		#region Initialization

		[SetUp()]
		public void Setup()
		{
			this.parser = new Parser();
			this.generator = new BindingsGenerator();
		}

		#endregion

		[Test()]
		public void TestCase()
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
				var result = parser.Parse(stream);
				var csharp = generator.Generate(result);
			}
		}
	}
}
