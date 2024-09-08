using System.Collections.Immutable;
using System.Linq;
using BeatSaberMarkupLanguage.Analyzers.Resources;
using BeatSaberMarkupLanguage.Analyzers.Utilities;
using BeatSaberMarkupLanguage.Attributes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BeatSaberMarkupLanguage.Analyzers;

/// <summary>
/// A <see cref="DiagnosticSuppressor"/> for properties and fields marked with various BSML attributes.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class UIAttributeSuppressor : DiagnosticSuppressor
{
    private static readonly SuppressionDescriptor AddReadonlyModifierRule = new(
        id: "BSMLS0001",
        suppressedDiagnosticId: "IDE0044",
        justification: Strings.AddReadonlyModifierUIAttributeSuppressorJustification);

    private static readonly SuppressionDescriptor RemoveUnusedPrivateMemberRule = new(
        id: "BSMLS0002",
        suppressedDiagnosticId: "IDE0051",
        justification: Strings.RemoveUnusedPrivateMemberUIAttributeSuppressorJustification);

    private static readonly SuppressionDescriptor RemoveUnreadPrivateMemberRule = new(
        id: "BSMLS0003",
        suppressedDiagnosticId: "IDE0052",
        justification: Strings.RemoveUnreadPrivateMemberUIAttributeSuppressorJustification);

    private static readonly SuppressionDescriptor PrivateMemberNeverUsedRule = new(
        id: "BSMLS0004",
        suppressedDiagnosticId: "CS0169",
        justification: Strings.PrivateMemberNeverUsedUIAttributeSuppressorJustification);

    private static readonly SuppressionDescriptor PrivateFieldAssignedButNeverUsedRule = new(
        id: "BSMLS0005",
        suppressedDiagnosticId: "CS0414",
        justification: Strings.PrivateFieldAssignedButNeverUsedUIAttributeSuppressorJustification);

    private static readonly SuppressionDescriptor FieldNeverAssignedRule = new(
        id: "BSMLS0006",
        suppressedDiagnosticId: "CS0649",
        justification: Strings.FieldNeverAssignedUIAttributeSuppressorJustification);

    /// <inheritdoc />
    public override ImmutableArray<SuppressionDescriptor> SupportedSuppressions => ImmutableArray.Create(
        AddReadonlyModifierRule,
        RemoveUnusedPrivateMemberRule,
        RemoveUnreadPrivateMemberRule,
        PrivateMemberNeverUsedRule,
        PrivateFieldAssignedButNeverUsedRule,
        FieldNeverAssignedRule);

    /// <inheritdoc />
    public override void ReportSuppressions(SuppressionAnalysisContext context)
    {
        foreach (Diagnostic diagnostic in context.ReportedDiagnostics)
        {
            this.AnalyzeDiagnosticField(diagnostic, context);
        }
    }

    private static bool IsSuppressible(ISymbol fieldSymbol)
    {
        if (fieldSymbol.GetAttributes().Any(a => a.AttributeClass != null && a.AttributeClass.Matches(typeof(UIAction), typeof(UIComponent), typeof(UIObject), typeof(UIParams), typeof(UIValue))))
        {
            return true;
        }

        return false;
    }

    private void AnalyzeDiagnosticField(Diagnostic diagnostic, SuppressionAnalysisContext context)
    {
        SyntaxNode? syntaxNode = context.GetSuppressibleNode<SyntaxNode>(diagnostic, n => n is VariableDeclaratorSyntax or MethodDeclarationSyntax or PropertyDeclarationSyntax);
        if (syntaxNode == null)
        {
            return;
        }

        SyntaxTree? syntaxTree = diagnostic.Location.SourceTree;
        if (syntaxTree == null)
        {
            return;
        }

        SemanticModel model = context.GetSemanticModel(syntaxTree);
        ISymbol? declaredSymbol = model.GetDeclaredSymbol(syntaxNode)!;
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
