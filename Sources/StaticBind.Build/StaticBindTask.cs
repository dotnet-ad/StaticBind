using System; 
using System.IO;
namespace StaticBind.Build
{
	using Microsoft.Build.Framework;
	using Microsoft.Build.Utilities;

	public class StaticBindTask : Task
	{
		[Required]
		public ITaskItem Source { get; set; }

		public string AssemblyName { get; set; }

		[Output]
		public string OutputFile { get; set; }

		private Bindings Parse()
		{
			using (var stream = File.ReadAllText(Source.ItemSpec).ToStream())
			{
				if (Path.GetExtension(Source.ItemSpec) == ".xml")
				{
					Log.LogMessage("Parsing XML binding file ...");
					var parser = new XmlParser();
					return parser.Parse(stream);


				}
				else 
				{
					Log.LogMessage("Parsing Pseudo-CSharp binding file ...");
					var parser = new CsharpParser();
					return parser.Parse(stream);

				}
			}
		}

		private void Generate(Bindings bindings)
		{
			var generator = new CSharpGenerator();
			var outputCode = generator.Generate(bindings);
		}

		public override bool Execute()
		{
			Log.LogMessage("Source: {0}", Source.ItemSpec);
			Log.LogMessage("AssemblyName: {0}", AssemblyName);
			Log.LogMessage("OutputFile {0}", OutputFile);

			try
			{
				var bindings = this.Parse();

				return true;
			}
			catch (Exception e)
			{
				Log.LogMessage("Error: {0}", e.Message);
				Log.LogMessage("StackTrace: {0}", e.StackTrace);
				Log.LogError(null, null, null, Source, 0, 0, 0, 0, $"{e.Message}");
				return false;
			}
		}
	}
}