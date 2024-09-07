using System.Collections.Immutable;
using System.Linq;
using BeatSaberMarkupLanguage.Analyzers.Resources;
using BeatSaberMarkupLanguage.Analyzers.Utilities;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Zenject;

namespace BeatSaberMarkupLanguage.Analyzers;

/// <summary>
/// A <see cref="DiagnosticSuppressor"/> for Harmony patch methods and associated parameters.
/// </summary>
// TODO: This really belongs in its own project/package. Nothing currently available on NuGet seems to do this, so I'm leaving it here for now.
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class ZenjectInjectSuppressor : DiagnosticSuppressor
{
    private static readonly SuppressionDescriptor RemoveUnusedPrivateMemberRule = new(
        id: "BSMLS0007",
        suppressedDiagnosticId: "IDE0051",
        justification: Strings.RemoveUnusedPrivateMemberHarmonyPatchSuppressorJustification);

    /// <inheritdoc />
    public override ImmutableArray<SuppressionDescriptor> SupportedSuppressions => ImmutableArray.Create(RemoveUnusedPrivateMemberRule);

    /// <inheritdoc />
    public override void ReportSuppressions(SuppressionAnalysisContext context)
    {
        foreach (Diagnostic diagnostic in context.ReportedDiagnostics)
        {
            this.AnalyzeDiagnosticField(diagnostic, context);
        }
    }

    private static bool IsSuppressible(ISymbol symbol)
    {
        return symbol.GetAttributes().Any(a => a.AttributeClass != null && a.AttributeClass.Matches(typeof(InjectAttribute)));
    }

    private void AnalyzeDiagnosticField(Diagnostic diagnostic, SuppressionAnalysisContext context)
    {
        SyntaxNode? syntax = context.GetSuppressibleNode<SyntaxNode>(diagnostic, n => n is MethodDeclarationSyntax or ConstructorDeclarationSyntax);
        if (syntax == null)
        {
            return;
        }

        SyntaxTree? syntaxTree = diagnostic.Location.SourceTree;
        if (syntaxTree == null)
        {
            return;
        }

        SemanticModel model = context.GetSemanticModel(syntaxTree);
        ISymbol? declaredSymbol = model.GetDeclaredSymbol(syntax)!;
        if (!IsSuppressible(declaredSymbol))
        {
            return;
        }

        foreach (SuppressionDescriptor descriptor in this.SupportedSuppressions.Where(d => d.SuppressedDiagnosticId == diagnostic.Id))
        {
            context.ReportSuppression(Suppression.Create(descriptor, diagnostic));
        }
    }
}
