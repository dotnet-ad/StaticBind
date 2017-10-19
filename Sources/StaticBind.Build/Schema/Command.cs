namespace StaticBind.Build
{
	using System;
	using System.Xml.Serialization;

	[XmlRoot("Command")]
	public class Command
	{
		[XmlAttribute("From")]
		public string From { get; set; }

		[XmlAttribute("To")]
		public string To { get; set; }

		[XmlAttribute("ExecuteWhen")]
		public string ExecuteWhen { get; set; }

		[XmlAttribute("IsEnabled")]
		public string IsEnabled { get; set; }

	}
}
