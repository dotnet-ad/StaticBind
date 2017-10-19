namespace StaticBind.Build
{
	using System;
	using System.Collections.Generic;
	using System.Xml.Serialization;
	using System.Linq;

	public class Target
	{
		[XmlAttribute("Class")]
		public string ClassFullname { get; set; }

		[XmlElement("Bind")]
		public List<Bind> Bindings { get; set; }

		[XmlElement("Command")]
		public List<Command> Commands { get; set; }

		[XmlIgnore]
		public string ClassName => this.ClassFullname.Split('.').Last();

		[XmlIgnore] 
		public string Namespace
		{
			get
			{
				var splits = this.ClassFullname.Split('.');
				return string.Join(".", splits.Take(splits.Length - 1));
			}
		}
	}
}
