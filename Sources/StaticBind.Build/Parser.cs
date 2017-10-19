using System;
using System.Xml.Linq;
using System.Xml.Schema;
using System.IO;
using System.Xml.Serialization;
using System.Linq;

namespace StaticBind.Build
{
	public class Parser
	{
		public Parser()
		{
		}

		private Bindings Validate(Bindings bindings)
		{
			if (bindings == null)
				throw new InvalidOperationException("Empty root element");

			if (string.IsNullOrEmpty(bindings.Source?.ClassFullname))
				throw new InvalidOperationException("The 'Source' element must be declared and have a 'Class' property.");
			
			if (string.IsNullOrEmpty(bindings.Target?.ClassFullname))
				throw new InvalidOperationException("The 'Target' element must be declared and have a 'Class' property.");

			if(string.IsNullOrEmpty(bindings.Converter) && (bindings.Source.Bindings.Concat(bindings.Target.Bindings).Any(x=> !string.IsNullOrEmpty(x.Converter))))
			{
				throw new InvalidOperationException("A 'Converter' class attribute must be declared on the root node for using converters with bindings.");
			}

			return bindings;
		}

		public Bindings Parse(Stream document)
		{
			using(var sr = new StreamReader(document))
			{
				var xs = new XmlSerializer(typeof(Bindings));
				return Validate(xs.Deserialize(sr) as Bindings);
			}
		}
	}
}
