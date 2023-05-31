using System;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;

namespace BeatSaberMarkupLanguage.Analyzers.Utilities;

/// <summary>
/// Extensions for <see cref="ITypeSymbol" />.
/// </summary>
internal static class TypeSymbolExtensions
{
    /// <summary>
    /// Checks that the <paramref name="symbol"/> matches or is a subclass of <paramref name="type"/>.
    /// </summary>
    /// <param name="symbol">The symbol to check against.</param>
    /// <param name="type">The type to match.</param>
    /// <returns>Whether or not the symbol matches or is a subclass of the type.</returns>
    public static bool Extends(this ITypeSymbol? symbol, Type? type)
    {
        if (symbol == null || type == null)
        {
            return false;
        }

        while (symbol != null)
        {
            if (symbol.Matches(type))
            {
                return true;
            }

            symbol = symbol.BaseType;
        }

        return false;
    }

    /// <summary>
    /// Checks that the <paramref name="symbol"/> matches any one of the <paramref name="types"/>.
    /// </summary>
    /// <param name="symbol">The symbol to check against.</param>
    /// <param name="types">The types to match.</param>
    /// <returns>Whether or not the symbol matches any one of the types.</returns>
    public static bool Matches(this ITypeSymbol symbol, params Type[] types) => types.Any(type => symbol.Matches(type));

    /// <summary>
    /// Checks that the <paramref name="symbol"/> matches the <paramref name="type"/>.
    /// </summary>
    /// <param name="symbol">The symbol to check against.</param>
    /// <param name="type">The type to match.</param>
    /// <returns>Whether or not the symbol matches the type.</returns>
    public static bool Matches(this ITypeSymbol symbol, Type type)
    {
        switch (symbol.SpecialType)
        {
            case SpecialType.System_Void:
                return type == typeof(void);
            case SpecialType.System_Boolean:
                return type == typeof(bool);
            case SpecialType.System_Int32:
                return type == typeof(int);
            case SpecialType.System_Single:
                return type == typeof(float);
        }

        if (type.IsArray)
        {
            return symbol is IArrayTypeSymbol array && array.ElementType.Matches(type.GetElementType()!);
        }

        if (symbol is not INamedTypeSymbol named)
        {
            return false;
        }

        if (type.IsConstructedGenericType)
        {
            var args = type.GetTypeInfo().GenericTypeArguments;
            if (args.Length != named.TypeArguments.Length)
            {
                return false;
            }

            for (var i = 0; i < args.Length; i++)
            {
                if (!named.TypeArguments[i].Matches(args[i]))
                {
                    return false;
                }
            }

            return named.ConstructedFrom.Matches(type.GetGenericTypeDefinition());
        }

        return named.Name == type.Name
               && named.ContainingNamespace?.ToDisplayString() == type.Namespace;
    }
}
