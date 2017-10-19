using System;
namespace StaticBind.Build
{
	using System.Linq;
	public class Validator
	{
		public Validator()
		{
		}

		public void Validate(Bindings bindings)
		{
			if (bindings == null)
				throw new BindingGenerationException(bindings.File, 0, 0, "Insert minimum tags", "Empty root element");

			if (string.IsNullOrEmpty(bindings.Source?.ClassFullname))
				throw new InvalidOperationException("The 'Source' element must be declared and have a 'Class' property.");

			if (string.IsNullOrEmpty(bindings.Target?.ClassFullname))
				throw new InvalidOperationException("The 'Target' element must be declared and have a 'Class' property.");

			if (string.IsNullOrEmpty(bindings.Converter) && (bindings.Source.Bindings.Concat(bindings.Target.Bindings).Any(x => !string.IsNullOrEmpty(x.Converter))))
			{
				throw new InvalidOperationException("A 'Converter' class attribute must be declared on the root node for using converters with bindings.");
			}
		}
	}
}
