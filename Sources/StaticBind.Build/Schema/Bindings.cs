using System.Xml.Serialization;

namespace StaticBind.Build
{
	[XmlRoot("Bindings")]
	public class Bindings
	{
		[XmlAttribute("Visibility")]
		public Visibility Visibility { get; set; }

		[XmlAttribute("Converter")]
		public string Converter { get; set; }

		[XmlElement("Source")]
		public Target Source { get; set; } = new Target();

		[XmlElement("Target")]
		public Target Target { get; set; } = new Target();
	}
}
