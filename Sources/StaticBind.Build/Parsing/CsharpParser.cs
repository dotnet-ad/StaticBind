namespace StaticBind.Build
{
	using System.IO;
	using System.Linq;
	using Microsoft.CodeAnalysis.CSharp;
	using Microsoft.CodeAnalysis.CSharp.Syntax;
	using System;

	public class CsharpParser
	{
		#region Bind

		private bool ParseBindingEvent(Property result, ArgumentSyntax syntax)
		{
			if (syntax?.Expression is InvocationExpressionSyntax invoke)
			{
				if (invoke?.Expression is MemberAccessExpressionSyntax method)
				{
					if (method?.Expression is IdentifierNameSyntax staticClass)
					{
						if(staticClass.Identifier.ToString() == "When" && method.Name.Identifier.ToString() == "Event")
						{
							if (invoke?.ArgumentList.Arguments.FirstOrDefault().Expression is InvocationExpressionSyntax nameofInvoke)
							{
								if (nameofInvoke.Expression is IdentifierNameSyntax identifier && identifier.ToString() == "nameof")
								{
									if (nameofInvoke.ArgumentList.Arguments.FirstOrDefault()?.Expression is MemberAccessExpressionSyntax nameofAccess)
									{
										result.When = "";

										if(method.Name is GenericNameSyntax genericName && genericName.TypeArgumentList.Arguments.Any())
										{
											var typeArg = genericName.TypeArgumentList.Arguments.First();
											result.When += ParseFulltype(typeArg) + ":";
										}

										result.When += nameofAccess.Name.ToString();
										return true;
									}
								}
							}
						}
					}
				}
			}

			return false;
		}

		private Tuple<string,string> ParseAccess(MemberAccessExpressionSyntax access)
		{
			var path = access.Name.ToString();

			while (access.Expression is MemberAccessExpressionSyntax subaccess)
			{
				path = $"{subaccess.Name}.{path}";
				access = subaccess;
			}

			if (access.Expression is IdentifierNameSyntax identifier)
			{
				return new Tuple<string, string>(identifier.ToString(), path);
			}
			else if (access.Expression is ThisExpressionSyntax thise)
			{
				return new Tuple<string, string>("this", path);
			}

			throw new InvalidOperationException("Invalid member access expression");
		}

		private bool ParseBindingConverter(Property result, ArgumentSyntax syntax, string converterName)
		{
			if (syntax?.Expression is InvocationExpressionSyntax invoke)
			{
				if (invoke?.Expression is MemberAccessExpressionSyntax method)
				{
					if (method?.Expression is IdentifierNameSyntax staticClass)
					{
						if (staticClass.Identifier.ToString() == "Conversion" && method.Name.Identifier.ToString() == "Value")
						{
							if (invoke?.ArgumentList.Arguments.FirstOrDefault().Expression is MemberAccessExpressionSyntax access)
							{
								var accessResult = ParseAccess(access);

								if (accessResult.Item1 == converterName)
								{
									result.Converter = accessResult.Item2;
									return true;
								}

								throw new InvalidOperationException($"Converter must be a reference to a '{converterName}' access member.");
							}
						}
					}
				}
			}

			return false;
		}

		private string ParseFulltype(TypeSyntax syntax)
		{
			if (!(syntax is QualifiedNameSyntax))
				throw new InvalidOperationException("All types must be fully qualified (including namespace)");
			
			return $"{syntax.GetText()}";
		}

		private Tuple<Target,string> ParseBindingPath(Bindings result, ArgumentSyntax syntax, string sourceName)
		{
			if(syntax?.Expression is ParenthesizedLambdaExpressionSyntax lambda)
			{
				if(lambda?.Body is MemberAccessExpressionSyntax access)
				{
					var accessResult = this.ParseAccess(access);
				
					if (accessResult.Item1 == sourceName)
					{
						return new Tuple<Target, string>(result.Source, accessResult.Item2);
					}
					else if(accessResult.Item1 == "this")
					{
						return new Tuple<Target, string>(result.Target, accessResult.Item2);
					}
				}
			}

			throw new InvalidOperationException("The path expression should be lambda expression with a complete path from 'source' or 'this'");
		}

		private void ParseBinding(Bindings result, ArgumentListSyntax args, string sourceName, string converterName)
		{
			var fromArg = ParseBindingPath(result, args.Arguments.ElementAtOrDefault(0),sourceName);
			var toArg = ParseBindingPath(result, args.Arguments.ElementAtOrDefault(1),sourceName);

			if (fromArg.Item1 == toArg.Item1)
				throw new InvalidOperationException("The source and target must be different.");

			var bind = new Property { From = fromArg.Item2, To = toArg.Item2 }; 

			if(args.Arguments.Count == 3 && (ParseBindingEvent(bind, args.Arguments.ElementAtOrDefault(2)) || ParseBindingConverter(bind, args.Arguments.ElementAtOrDefault(2),converterName))) {}
			else if (args.Arguments.Count == 4 && ParseBindingEvent(bind, args.Arguments.ElementAtOrDefault(2)) && ParseBindingConverter(bind, args.Arguments.ElementAtOrDefault(3), converterName)) {}
			else if(args.Arguments.Count != 2)
			{
				throw new InvalidOperationException("The binding expression is invalid");
			}

			fromArg.Item1.Bindings.Add(bind);
		}

		#endregion

		private void ParseExpression(Bindings result, ExpressionSyntax expr, string sourceName, string converterName)
		{
			if (expr is InvocationExpressionSyntax inv)
			{
				string name = null;
				ArgumentListSyntax args = null;

				if (inv.Expression is MemberAccessExpressionSyntax access)
				{
					name = access.Name.GetText().ToString();
					args = inv.ArgumentList;
					ParseExpression(result, access.Expression, sourceName, converterName);
				}

				switch (name)
				{
					case "Bind":
						ParseBinding(result, args, sourceName, converterName);
						break;
					default:
						throw new InvalidOperationException("Supported statements is only : 'Bind'.");
				}
			}
		}

		private void ParseStatement(Bindings result, StatementSyntax statement, string sourceName, string converterName)
		{
			if(statement is ExpressionStatementSyntax expr)
			{
				this.ParseExpression(result, expr.Expression, sourceName, converterName);
			}
		}

		private void ParseMethod(Bindings result, MethodDeclarationSyntax method)
		{
			var arg = method.ParameterList.Parameters.FirstOrDefault();

			result.Source = new Target()
			{
				ClassFullname = ParseFulltype(arg.Type),
				Bindings = new System.Collections.Generic.List<Property>(),
			};

			var conv = method.ParameterList.Parameters.ElementAtOrDefault(1);
			var converterName = conv?.Identifier.ToString();

			if(conv != null)
			{
				result.Converter = ParseFulltype(conv.Type);
			}

			var sourceName = arg.Identifier.ToString();

			foreach (var statement in method.Body.Statements)
			{
				this.ParseStatement(result, statement, sourceName, converterName);
			}
		}

		private void ParseNamespace(Bindings result, NamespaceDeclarationSyntax ns)
		{
			var classNode = ns.DescendantNodes()
							  .OfType<ClassDeclarationSyntax>()
							  .FirstOrDefault();

			result.Target = new Target()
			{
				ClassFullname = $"{ns.Name.GetText().ToString().Trim()}.{classNode.Identifier.Text}",
				Bindings = new System.Collections.Generic.List<Property>(),
			};

			var method = classNode.DescendantNodes()
								  .OfType<MethodDeclarationSyntax>()
								  .FirstOrDefault(x => x.AttributeLists.Any(l => l.Attributes.Any(a => a.Name.ToString() == "Bindings")));

			this.ParseMethod(result,method);
		}

		public Bindings Parse(Stream document)
		{
			var result = new Bindings();

			using (var sr = new StreamReader(document))
			{
				var code = sr.ReadToEnd();

				var tree = CSharpSyntaxTree.ParseText(code);

				var root = tree.GetRoot();

				var ns = root.DescendantNodes()
				             .OfType<NamespaceDeclarationSyntax>()
				             .FirstOrDefault();

				this.ParseNamespace(result,ns);

			}

			return result;
		}
	}
}
