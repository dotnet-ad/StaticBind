using System.Xml.Serialization;
using System.Linq;

namespace StaticBind.Build
{
	[XmlRoot("Property")]
	public class Property
	{
		[XmlAttribute("From")]
		public string From { get; set; }

		[XmlAttribute("To")]
		public string To { get; set; }

		[XmlAttribute("Converter")]
		public string Converter { get; set; }

		[XmlAttribute("When")]
		public string When { get; set; }

		[XmlIgnore]
		public bool HasEvent => !string.IsNullOrEmpty(this.When);

		[XmlIgnore]
		public string WhenEventHandler
		{
			get
			{
				if (!HasEvent)
					return null;
				
				var splits = this.When?.Split(':');

				if (splits.Length == 1)
					return "EventHandler";

				var handler = splits.First();
				if (handler.EndsWith("EventArgs", System.StringComparison.Ordinal))
					return $"EventHandler<{handler}>";
				
				return handler;
			}
		}

		[XmlIgnore]
		public string WhenEventName
		{
			get
			{
				if (!HasEvent)
					return null;

				var splits = this.When?.Split(':');

				if (splits.Length == 1)
					return splits.First();

				return splits.ElementAt(1);
			}
		}
	}
}
