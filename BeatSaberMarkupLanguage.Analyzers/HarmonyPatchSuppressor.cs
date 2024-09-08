using System.Collections.Immutable;
using System.Linq;
using BeatSaberMarkupLanguage.Analyzers.Resources;
using BeatSaberMarkupLanguage.Analyzers.Utilities;
using HarmonyLib;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BeatSaberMarkupLanguage.Analyzers;

/// <summary>
/// A <see cref="DiagnosticSuppressor"/> for Harmony patch methods and associated parameters.
/// </summary>
// TODO: This really belongs in its own project/package. Nothing currently available on NuGet seems to do this, so I'm leaving it here for now.
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class HarmonyPatchSuppressor : DiagnosticSuppressor
{
    private static readonly SuppressionDescriptor RemoveUnusedPrivateMemberRule = new(
        id: "BSMLS0007",
        suppressedDiagnosticId: "IDE0051",
        justification: Strings.RemoveUnusedPrivateMemberHarmonyPatchSuppressorJustification);

    private static readonly SuppressionDescriptor PrivateMemberNeverUsedRule = new(
        id: "BSMLS0008",
        suppressedDiagnosticId: "CS0169",
        justification: Strings.PrivateMemberNeverUsedHarmonyPatchSuppressorJustification);

    private static readonly SuppressionDescriptor StyleCopParameterNamingRule = new(
        id: "BSMLS0009",
        suppressedDiagnosticId: "SA1313",
        justification: Strings.StyleCopParameterNamingHarmonyPatchSuppressorJustification);

    private static readonly string[] HarmonyDefaultMethodNames = new[] { "Cleanup", "Finalizer", "Postfix", "Prefix", "Prepare", "TargetMethod", "TargetMethods", "Transpiler" };

    /// <inheritdoc />
    public override ImmutableArray<SuppressionDescriptor> SupportedSuppressions => ImmutableArray.Create(
        RemoveUnusedPrivateMemberRule,
        PrivateMemberNeverUsedRule,
        StyleCopParameterNamingRule);

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
        if (!symbol.ContainingType.GetAttributes().Any(a => a.AttributeClass != null && a.AttributeClass.Matches(typeof(HarmonyPatch))))
        {
            return false;
        }

        if (symbol is IMethodSymbol &&
            !HarmonyDefaultMethodNames.Contains(symbol.Name) &&
            !symbol.GetAttributes().Any(a => a.AttributeClass != null && a.AttributeClass.Matches(
                typeof(HarmonyCleanup),
                typeof(HarmonyFinalizer),
                typeof(HarmonyPostfix),
                typeof(HarmonyPrefix),
                typeof(HarmonyPrepare),
                typeof(HarmonyReversePatch),
                typeof(HarmonyTargetMethod),
                typeof(HarmonyTargetMethods),
                typeof(HarmonyTranspiler))))
        {
            return false;
        }

        return true;
    }

    private void AnalyzeDiagnosticField(Diagnostic diagnostic, SuppressionAnalysisContext context)
    {
        SyntaxNode? syntaxNode = context.GetSuppressibleNode<SyntaxNode>(diagnostic, n => n is MethodDeclarationSyntax or ParameterSyntax);
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
