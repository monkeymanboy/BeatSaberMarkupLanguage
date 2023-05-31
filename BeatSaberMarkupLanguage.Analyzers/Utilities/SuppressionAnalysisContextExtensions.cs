/*--------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *-------------------------------------------------------------------------------------------*/

using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BeatSaberMarkupLanguage.Analyzers.Utilities;

/// <summary>
/// Extensions for <see cref="SuppressionAnalysisContext"/>.
/// </summary>
internal static class SuppressionAnalysisContextExtensions
{
    /// <summary>
    /// Finds the <see cref="SyntaxNode"/> of type <typeparamref name="T"/> in the descendants of the diagnostic's node.
    /// </summary>
    /// <typeparam name="T">The type of <see cref="SyntaxNode"/> to search for.</typeparam>
    /// <param name="context">The context passed from the <see cref="DiagnosticSuppressor"/>.</param>
    /// <param name="diagnostic">The <see cref="Diagnostic"/> under which to search.</param>
    /// <returns>The first descendant node that matches the type <typeparamref name="T"/>.</returns>
    public static T? GetSuppressibleNode<T>(this SuppressionAnalysisContext context, Diagnostic diagnostic)
        where T : SyntaxNode
    {
        return context.GetSuppressibleNode<T>(diagnostic, _ => true);
    }

    /// <summary>
    /// Finds the <see cref="SyntaxNode"/> of type <typeparamref name="T"/> in the descendants of the diagnostic's node.
    /// </summary>
    /// <typeparam name="T">The type of <see cref="SyntaxNode"/> to search for.</typeparam>
    /// <param name="context">The context passed from the <see cref="DiagnosticSuppressor"/>.</param>
    /// <param name="diagnostic">The <see cref="Diagnostic"/> under which to search.</param>
    /// <param name="predicate">A function to perform additional filtering of found nodes.</param>
    /// <returns>The first descendant node that matches the type <typeparamref name="T"/> and the <paramref name="predicate"/>.</returns>
    public static T? GetSuppressibleNode<T>(this SuppressionAnalysisContext context, Diagnostic diagnostic, Func<T, bool> predicate)
        where T : SyntaxNode
    {
        var location = diagnostic.Location;
        var sourceTree = location.SourceTree;
        var root = sourceTree?.GetRoot(context.CancellationToken);

        return root?
            .FindNode(location.SourceSpan)
            .DescendantNodesAndSelf()
            .OfType<T>()
            .FirstOrDefault(predicate);
    }
}
