using System;
using System.IO;

namespace StaticBind.Build.Tests
{
	public static class Utils
	{
		public static Stream GenerateStream(string s)
		{
			var stream = new MemoryStream();
			var writer = new StreamWriter(stream);
			writer.Write(s);
			writer.Flush();
			stream.Position = 0;
			return stream;
		}
	}
}
