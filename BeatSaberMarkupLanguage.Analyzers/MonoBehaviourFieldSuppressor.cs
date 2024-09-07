using System;
using System.Collections.Immutable;
using System.Linq;
using BeatSaberMarkupLanguage.Analyzers.Resources;
using BeatSaberMarkupLanguage.Analyzers.Utilities;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using UnityEngine;

namespace BeatSaberMarkupLanguage.Analyzers;

/// <summary>
/// A <see cref="DiagnosticSuppressor"/> for Harmony patch methods and associated parameters.
/// </summary>
// TODO: This really belongs in its own project/package. Nothing currently available on NuGet seems to do this, so I'm leaving it here for now.
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class MonoBehaviourFieldSuppressor : DiagnosticSuppressor
{
    private static readonly SuppressionDescriptor StyleCopParameterNamingRule = new(
        id: "BSMLS0010",
        suppressedDiagnosticId: "SA1401",
        justification: Strings.StyleCopParameterNamingHarmonyPatchSuppressorJustification);

    /// <inheritdoc />
    public override ImmutableArray<SuppressionDescriptor> SupportedSuppressions => ImmutableArray.Create(StyleCopParameterNamingRule);

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
        if (!symbol.ContainingType.Extends(typeof(MonoBehaviour)))
        {
            return false;
        }

        return symbol is IFieldSymbol fieldSymbol && !fieldSymbol.IsReadOnly && !fieldSymbol.IsStatic;
    }

    private void AnalyzeDiagnosticField(Diagnostic diagnostic, SuppressionAnalysisContext context)
    {
        SyntaxNode? fieldDeclarationSyntax = context.GetSuppressibleNode<VariableDeclaratorSyntax>(diagnostic);
        if (fieldDeclarationSyntax == null)
        {
            return;
        }

        SyntaxTree? syntaxTree = diagnostic.Location.SourceTree;
        if (syntaxTree == null)
        {
            return;
        }

        SemanticModel model = context.GetSemanticModel(syntaxTree);
        ISymbol? declaredSymbol = model.GetDeclaredSymbol(fieldDeclarationSyntax)!;
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
