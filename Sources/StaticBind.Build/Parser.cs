using System;
using System.Xml.Linq;
using System.Xml.Schema;
using System.IO;
using System.Xml.Serialization;

namespace StaticBind.Build
{
	public class Parser
	{
		public Parser()
		{
		}

		public Bindings Parse(Stream document)
		{
			using(var sr = new StreamReader(document))
			{
				var xs = new XmlSerializer(typeof(Bindings));
				return xs.Deserialize(sr) as Bindings;
			}
		}
	}
}
