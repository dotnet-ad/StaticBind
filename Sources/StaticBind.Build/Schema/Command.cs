namespace StaticBind.Build
{
	using System;
	using System.Linq;
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
	
		[XmlIgnore]
		public string ExecuteWhenEventHandler
		{
			get
			{
					var splits = this.ExecuteWhen?.Split(':');

				if (splits.Length == 1)
					return "EventHandler";

				return splits.First();
			}
		}

		[XmlIgnore]
		public string ExecuteWhenEventName
		{
			get
			{
				var splits = this.ExecuteWhen?.Split(':');

				if (splits.Length == 1)
					return splits.First();

				return splits.ElementAt(1);
			}
		}

	}
}
