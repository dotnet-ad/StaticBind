namespace StaticBind.Build
{
	using System;

	public class BindingGenerationException : Exception
	{
		public BindingGenerationException(string file, int line, int column, string help, string message) : base(message)
		{
			this.File = file;
			this.LineNumber = line;
			this.ColumnNumber = column;
			this.Help = help;
		}

		public string File { get; }

		public string Help { get; }

		public int LineNumber { get; }

		public int ColumnNumber { get; }

		public int EndLineNumber { get; }

		public int EndColumnNumber { get; }
	}
}
