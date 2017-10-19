namespace StaticBind.Build
{
	using System;
	using System.IO;
	using Microsoft.Build.Framework;
	using Microsoft.Build.Utilities;

	public class StaticBindTask : Task
	{
		#region Properties

		[Required]
		public ITaskItem Source { get; set; }

		[Output]
		public string OutputFile { get; set; }

		#endregion

		#region Private methods

		private Bindings Parse()
		{
			Bindings result = null;

			using (var stream = File.ReadAllText(Source.ItemSpec).ToStream())
			{
				if (Path.GetExtension(Source.ItemSpec) == ".xml")
				{
					Log.LogMessage("Parsing XML binding file ...");
					var parser = new XmlParser();
					result = parser.Parse(stream);
				}
				else
				{
					Log.LogMessage("Parsing Pseudo-CSharp binding file ...");
					var parser = new CsharpParser();
					result = parser.Parse(stream);
				}
			}

			result.File = Source.ItemSpec;

			return result;
		}

		private void Validate(Bindings bindings)
		{
			Log.LogMessage("Generation parsed file ...");
			var validator = new Validator();
			validator.Validate(bindings);
		}

		private void Generate(Bindings bindings)
		{
			Log.LogMessage("Generation C# file ...");
			var generator = new CSharpGenerator();
			var outputCode = generator.Generate(bindings);

			var dir = Path.GetDirectoryName(this.OutputFile);
			if(!Directory.Exists(dir))
			{
				Directory.CreateDirectory(dir);
			}

			File.WriteAllText(this.OutputFile, outputCode);
		}

		#endregion

		#region Execution

		public override bool Execute()
		{
			Log.LogMessage("Source: {0}", Source.ItemSpec);
			Log.LogMessage("OutputFile {0}", OutputFile);

			try
			{
				var bindings = this.Parse();
				this.Validate(bindings);
				this.Generate(bindings);
				return true;
			}
			catch (BindingGenerationException e)
			{
				Log.LogError("StaticBind", "SB002", e.Help, e.File, e.LineNumber, e.ColumnNumber, e.EndLineNumber, e.EndColumnNumber, e.Message);
				Log.LogMessage("StackTrace: {0}", e.StackTrace);
				return false;
			}
			catch (Exception e)
			{
				Log.LogError("StaticBind", "SB001", "", Source.ItemSpec , 0, 0, 0, 0, e.Message);
				Log.LogMessage("StackTrace: {0}", e.StackTrace);
				return false;
			}
		}

		#endregion
	}
}