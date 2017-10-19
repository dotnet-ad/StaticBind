namespace StaticBind.Build
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	public class CSharpGenerator : Generator
	{
		public CSharpGenerator()
		{
		}

		private string GetPropertyName(string root, Target toTarget, string path, Dictionary<string, string> properties)
		{
			if (properties.TryGetValue(path, out string result))
				return result;

			var splits = path.Split('.');
			var name = splits.Last();
			string id = null;

			var needsSetter = toTarget.Bindings.Any(x => x.To == path) || toTarget.Commands.Any(x => x.IsEnabled == path);
			var setter = needsSetter ? $", (x,v) => x.{name} = v" : "";

			if(splits.Length == 1)
			{
				id = $"{root}_{this.NewId()}";
				this.AppendLine($"var {id} = {root}_root.Property(nameof({root}.{path}), x => x.{name}{setter});");
			}
			else
			{
				var parentId = GetPropertyName(root, toTarget, string.Join(".", splits.Take(splits.Length - 1)),  properties);
				id = $"{root}_{this.NewId()}";
				this.AppendLine($"var {id} = {parentId}.Property(nameof({root}.{path}), x => x.{name}{setter});");
			}

			properties[path] = id;

			return id;
		}

		private void Generate(string fromRoot, string toRoot, Target fromTarget, Target toTarget, Dictionary<string, string> from, Dictionary<string, string> to)
		{
			foreach (var bind in fromTarget.Bindings)
			{
				var fromId = GetPropertyName(fromRoot, toTarget, bind.From, from);
				var toId = GetPropertyName(toRoot, fromTarget, bind.To, to);

				var v = "v";
				if (!string.IsNullOrEmpty(bind.Converter))
				{
					v = $"converter.{bind.Converter}(v)";
				}

				if(bind.HasEvent)
				{
					var id = $"{fromRoot}_{this.NewId()}";
					this.AppendLine($"{bind.WhenEventHandler} {id} = null;");
					this.AppendLine($"{fromId}.ChangeWhen(" +
					                $"(x, a) => {{ {id} = (s, e) => a(); x.{bind.WhenEventName} += {id}; }}, " +
					                $"(x) => x.{bind.WhenEventName} -= {id});");
				}

				this.AppendLine($"{fromId}.OnChange(v => {toId}.Value = {v});");
			}

			foreach (var command in fromTarget.Commands)
			{
				var fromId = GetPropertyName(fromRoot, toTarget, command.From, from);
				var toId = GetPropertyName(toRoot, fromTarget, command.To, to);
				var enabledId = GetPropertyName(toRoot, fromTarget, $"{command.To}.{command.IsEnabled}", to);


				var command_id = $"{fromRoot}_command_{this.NewId()}";
				this.AppendLine($"var {command_id} = {fromId}.Command({toId});");

				this.AppendLine($"{command_id}.OnCanExecuteChange(v => {enabledId}.Value = v);");

				var id = $"{fromRoot}_{this.NewId()}";
				this.AppendLine($"{command.ExecuteWhenEventHandler} {id} = null;");
				this.AppendLine($"{command_id}.ExecuteWhen(" +
				                $"(x,a) => {{ {id} = (s, e) => a(); x.{command.ExecuteWhenEventName} += {id}; }}, " +
				                $"(x) => x.{command.ExecuteWhenEventName} -= {id}" +
				                ");");
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
						this.Generate("this", "source", bindings.Target, bindings.Source, targetProperties, sourceProperties);
						this.Generate("source", "this", bindings.Source, bindings.Target, sourceProperties, targetProperties);
						this.AppendLine("");
						this.AppendLine($"this.Bindings = new Bindings<{bindings.Source.ClassFullname}, {bindings.Target.ClassFullname}>(source_root, this_root);");

					});
				});
			});

			return builder.ToString();
		}
	}
}
