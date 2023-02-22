using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace WalletWasabi.Fluent.Generators;

[Generator]
public class UIContextConstructorGenerator : IIncrementalGenerator
{
	private static string[] Exclusions =
		new[]
		{
			"RoutableViewModel"
		};

	public void Initialize(IncrementalGeneratorInitializationContext context)
	{
		var ctorsProvider =
			context.SyntaxProvider.CreateSyntaxProvider(static (node, _) =>
			node is ConstructorDeclarationSyntax &&
			node.Parent is ClassDeclarationSyntax c &&
			c.Identifier.Text.EndsWith("ViewModel") &&
			!Exclusions.Contains(c.Identifier.Text),
			static (ctx, _) => ctx)
			.Where(static ctx => ViewModelReferencesUIContext(ctx.Node));

		var combined =
			context.CompilationProvider.Combine(ctorsProvider.Collect());

		context.RegisterSourceOutput(combined, Generate);
	}

	private static bool ViewModelReferencesUIContext(SyntaxNode node)
	{
		return
			node.Parent is ClassDeclarationSyntax c &&
			c.DescendantNodes()
			 .OfType<IdentifierNameSyntax>()
			 .Where(x => x.Identifier.ValueText == "UIContext")
			 .Any();
	}

	private static void Generate(SourceProductionContext context, (Compilation, ImmutableArray<GeneratorSyntaxContext>) args)
	{
		var (compilation, nodes) = args;

		var toGenerate =
			from g in nodes
			let n = g.Node
			let c = n.Parent as ClassDeclarationSyntax
			where c != null
			group g by c.Identifier.ValueText into g
			select g.First();

		foreach (var c in toGenerate)
		{
			var (node, model) = (c.Node, c.SemanticModel);
			var classDeclaration = ((ClassDeclarationSyntax)node.Parent);
			var fileName = classDeclaration.Identifier.ValueText + "_UIContext.cs";

			var classSymbol = model.GetDeclaredSymbol(node.Parent) as INamedTypeSymbol;
			var className = classSymbol.Name;
			var namespaceName = classSymbol.ContainingNamespace.ToDisplayString();
			var ctor = node as ConstructorDeclarationSyntax;
			var ctorArgs =
				ctor.ParameterList.Parameters
								  .Select(x => x.Identifier.ValueText);
			var hasCtorArgs = ctorArgs.Any();
			var ctorArgsString = string.Join(",", ctorArgs);
			var ctorString =
				hasCtorArgs
				? $": this({ctorArgsString})"
				: "";

			var parameterNamespaces =
				from p in ctor.ParameterList.Parameters
				let t = model.GetTypeInfo(p.Type)
				select $"using {t.Type.ContainingNamespace.ToDisplayString()};";

			var parametersString =
				ctor.ParameterList.Parameters.ToFullString();

			if (hasCtorArgs)
			{
				parametersString += ", ";
			}

			parametersString += "UIContext uiContext";

			var usings = string.Join(Environment.NewLine, parameterNamespaces.Distinct().OrderBy(x => x));

			var sourceText =
$$"""
{{usings}}
using WalletWasabi.Fluent.UIServices;

namespace {{namespaceName}};

partial class {{className}}
{
    public {{className}}({{parametersString}}){{ctorString}}
    {
	    UIContext = uiContext;
    }
}
""";
			context.AddSource(fileName, SourceText.From(sourceText, Encoding.UTF8));

			ValidateNotInConstructor(context, node);
		}
	}

	/// <summary>
	/// Report an error if UIContext is referenced in the constructor directly without being closed on by a lambda expression.
	/// UIContext cannot be referenced in constructor because it hasn't been initialized yet.
	/// </summary>
	/// <param name="context"></param>
	/// <param name="node"></param>
	private static void ValidateNotInConstructor(SourceProductionContext context, SyntaxNode node)
	{
		var invalidReference =
		 	((ClassDeclarationSyntax)node.Parent)
			.DescendantNodes()
			.OfType<IdentifierNameSyntax>()
			.Where(static x => x.Identifier.ValueText == "UIContext")
			.Where(static x => x.FirstAncestorOrSelf<ConstructorDeclarationSyntax>() != null)
			.Where(static x => x.FirstAncestorOrSelf<LambdaExpressionSyntax>() == null)
			.FirstOrDefault();

		if (invalidReference != null)
		{
			var msg = $"UIContext cannot be referenced in a ViewModel's constructor because it hasn't been initialized yet when constructor runs. Move this code to OnNavigatedTo().";
			var location = invalidReference.GetLocation();
			var diagnostic = Diagnostic.Create(new DiagnosticDescriptor("WW501", msg, msg, "UI", DiagnosticSeverity.Error, true), location);
			context.ReportDiagnostic(diagnostic);
		}
	}
}
