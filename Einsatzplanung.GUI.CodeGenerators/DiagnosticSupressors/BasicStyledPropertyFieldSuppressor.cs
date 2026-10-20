namespace Einsatzplanung.GUI.CodeGenerators.DiagnosticSupressors;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

using System.Collections.Immutable;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class BasicStyledPropertyFieldSuppressor : DiagnosticSuppressor {

	private const string AttributePrefix =
		"Einsatzplanung.GUI.CodeGenerators.Attributes.BasicStyledPropertyAttribute";

	private static readonly SuppressionDescriptor UnusedFieldRule = new(
		id: "EPSUP001",
		suppressedDiagnosticId: "CS0414",
		justification: "Field is consumed by the BasicStyledProperty source generator.");

	private static readonly SuppressionDescriptor NonNullableRule = new(
		id: "EPSUP002",
		suppressedDiagnosticId: "CS8618",
		justification: "Field is consumed by the BasicStyledProperty source generator.");


	public override ImmutableArray<SuppressionDescriptor> SupportedSuppressions
		=> ImmutableArray.Create(UnusedFieldRule, NonNullableRule);

	public override void ReportSuppressions(SuppressionAnalysisContext context) {
		foreach (var diagnostic in context.ReportedDiagnostics) {
			var rule = diagnostic.Id switch {
				"CS0414" => UnusedFieldRule,
				"CS8618" => NonNullableRule,
				_ => null
			};
			if (rule is null)
				continue;

			var tree = diagnostic.Location.SourceTree;
			if (tree is null)
				continue;

			var root = tree.GetRoot(context.CancellationToken);
			var node = root.FindNode(diagnostic.Location.SourceSpan);
			var model = context.GetSemanticModel(tree);

			// Case 1: reported directly on the field declarator
			var variable = node.FirstAncestorOrSelf<VariableDeclaratorSyntax>();
			if (variable?.Parent?.Parent is FieldDeclarationSyntax) {
				if (model.GetDeclaredSymbol(variable, context.CancellationToken) is IFieldSymbol f
					&& HasAttribute(f)) {
					context.ReportSuppression(Suppression.Create(rule, diagnostic));
				}
				continue;
			}

			// Case 2 (CS8618 only): reported on a constructor / type.
			// Match the field name quoted in the message, e.g. "field 'name'".
			if (diagnostic.Id == "CS8618") {
				var typeDecl = node.FirstAncestorOrSelf<TypeDeclarationSyntax>();
				if (typeDecl is null)
					continue;
				if (model.GetDeclaredSymbol(typeDecl, context.CancellationToken) is not INamedTypeSymbol type)
					continue;

				string message = diagnostic.GetMessage();
				bool matches = type.GetMembers()
					.OfType<IFieldSymbol>()
					.Any(f => HasAttribute(f) && message.Contains($"'{f.Name}'"));

				if (matches)
					context.ReportSuppression(Suppression.Create(rule, diagnostic));
			}
		}
	}

	private static bool HasAttribute(IFieldSymbol symbol) =>
		symbol.GetAttributes().Any(a =>
			a.AttributeClass?.ConstructedFrom.ToDisplayString()
				.StartsWith(AttributePrefix) == true);
}