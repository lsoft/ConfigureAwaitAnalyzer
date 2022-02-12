using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConfigureAwaitAnalyzer
{
    internal static class AnalyzerHelper
    {
        internal static bool ReturnsAwaitableTask(this IPropertySymbol propertySymbol)
        {
            var propertyType = propertySymbol.Type;

            if (propertyType == null)
            {
                return false;
            }

            return propertyType?.Name == "ConfiguredTaskAwaitable";
        }

        internal static bool ReturnsTask(this IPropertySymbol propertySymbol)
        {
            var propertyType = propertySymbol.Type;

            if (propertyType == null)
            {
                return false;
            }

            if (propertyType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) == "global::System.Threading.Tasks.Task")
            {
                return true;
            }
            if (propertyType.BaseType != null && propertyType.BaseType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) == "global::System.Threading.Tasks.Task")
            {
                return true;
            }

            return false;
        }

        internal static bool ReturnsTask(this IMethodSymbol methodSymbol)
        {
            var returnType = methodSymbol.ReturnType;

            if (returnType == null)
            {
                return false;
            }

            if (returnType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) == "global::System.Threading.Tasks.Task")
            {
                return true;
            }
            if (returnType.BaseType != null && returnType.BaseType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) == "global::System.Threading.Tasks.Task")
            {
                return true;
            }

            return false;
        }

        internal static bool ReturnsAwaitableTask(this IMethodSymbol methodSymbol)
        {
            var returnType = methodSymbol.ReturnType;

            if (returnType == null)
            {
                return false;
            }

            //if (returnType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) == "global::System.Threading.Tasks.Task")
            //{
            //    return true;
            //}
            //if (returnType.BaseType != null && returnType.BaseType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) == "global::System.Threading.Tasks.Task")
            //{
            //    return true;
            //}

            return returnType?.Name == "ConfiguredTaskAwaitable";
        }

        public static T TryGetSyntaxNode<T>(this SyntaxNodeAnalysisContext context) where T : SyntaxNode
        {
            if (!(context.Node is T node))
            {
                return null;
            }

            if (node.GetDiagnostics().Any(x => x.Severity == DiagnosticSeverity.Error))
            {
                return null;
            }

            return node;
        }
    }
}
