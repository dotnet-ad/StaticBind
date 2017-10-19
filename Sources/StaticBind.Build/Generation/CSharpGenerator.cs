using System;
using System.Collections.Generic;
using System.Linq;
namespace StaticBind.Build
{
	public class CSharpGenerator : Generator
	{
		public CSharpGenerator()
		{
		}

		private string GetPropertyName(string root, string path, Dictionary<string, string> properties)
		{
			if (properties.TryGetValue(path, out string result))
				return result;

			var splits = path.Split('.');
			var name = splits.Last();
			string id = null;

			if(splits.Length == 1)
			{
				id = $"{root}_{this.NewId()}";
				this.AppendLine($"var {id} = {root}_root.Then(nameof({root}.{path}), x => x.{name}, (x,v) => x.{name} = v);");
			}
			else
			{
				var parentId = GetPropertyName(root, string.Join(".", splits.Take(splits.Length - 1)), properties);
				id = $"{root}_{this.NewId()}";
				this.AppendLine($"var {id} = {parentId}.Then(nameof({root}.{path}), x => x.{name}, (x,v) => x.{name} = v);");
			}

			properties[path] = id;

			return id;
		}

		private void Generate(string fromRoot, string toRoot, Target target, Dictionary<string, string> from, Dictionary<string, string> to)
		{
			foreach (var bind in target.Bindings)
			{
				var fromId = GetPropertyName(fromRoot, bind.From, from);
				var toId = GetPropertyName(toRoot, bind.To, to);

				var v = "v";
				if (!string.IsNullOrEmpty(bind.Converter))
				{
					v = $"converter.{bind.Converter}(v)";
				}

				if(bind.HasEvent)
				{
					var id = $"{fromRoot}_{this.NewId()}";
					this.AppendLine($"{bind.WhenEventHandler} {id} = null;");
					this.AppendLine($"{fromId}.ChangeWhen((x, a) => {{ {id} = (s, e) => a(); x.{bind.WhenEventName} += {id}; }}, (x) => x.{bind.WhenEventName} -= {id});");
				}

				this.AppendLine($"{fromId}.OnChange(v => {toId}.Value = {v});");
			}

			foreach (var command in target.Commands)
			{
				var fromId = GetPropertyName(fromRoot, command.From, from);
				var toId = GetPropertyName(toRoot, command.To, to);
				var enabledId = GetPropertyName(toRoot, command.IsEnabled, to);

				this.AppendLine($"{fromId}.OnCanExecuteChange(v => {enabledId}.Value = v);");
				//TODO when
			}
		}

		public string Generate(Bindings bindings)
		{
			var targetProperties = new Dictionary<string, string>();
			var sourceProperties = new Dictionary<string, string>();

			this.Reset();

			this.AppendLine($"namespace {bindings.Target.Namespace}");
			this.Body(() =>
			{
				this.AppendLine($"using System;");
				this.AppendLine($"using StaticBind;");
				this.AppendLine("");
				this.AppendLine($"public partial class {bindings.Target.ClassName}");
				this.Body(() =>
				{
					this.AppendLine($"public Bindings<{bindings.Source.ClassFullname}, {bindings.Target.ClassFullname}> Bindings {{ get; private set; }}");
					this.AppendLine("");
					this.Append($"public void Bind({bindings.Source.ClassFullname} source");
					if(!string.IsNullOrEmpty(bindings.Converter))
					{
						this.Append($", {bindings.Converter} converter", false);
					}
					this.AppendLine($")", false);
					this.Body(() =>
					{
						this.AppendLine($"var source_root = source.CreateAccessor();");
						this.AppendLine($"var this_root = this.CreateAccessor();");
						this.AppendLine("");
						this.Generate("source", "this", bindings.Source, sourceProperties, targetProperties);
						this.Generate("this", "source", bindings.Target, targetProperties, sourceProperties);
						this.AppendLine("");
						this.AppendLine($"this.Bindings = new Bindings<{bindings.Source.ClassFullname}, {bindings.Target.ClassFullname}>(source_root, this_root);");

					});
				});
			});

			return builder.ToString();
		}
	}
}
