// Copyright (c) Microsoft Corporation.
// All rights reserved.
//
// This code is licensed under the MIT License.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files(the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and / or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions :
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

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
