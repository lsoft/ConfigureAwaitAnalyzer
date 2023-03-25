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

            if (propertyType.Name.Contains("Task")) //fast and dirty check for better performace and lesser allocations
            {
                if (propertyType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) == "global::System.Threading.Tasks.Task")
                {
                    return true;
                }
            }

            if (propertyType.BaseType != null)
            {
                if (propertyType.BaseType.Name.Contains("Task")) //fast and dirty check for better performace and lesser allocations
                {
                    if (propertyType.BaseType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) == "global::System.Threading.Tasks.Task")
                    {
                        return true;
                    }
                }
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

            if (returnType.Name.Contains("Task")) //fast and dirty check for better performace and lesser allocations
            {
                if (returnType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) == "global::System.Threading.Tasks.Task")
                {
                    return true;
                }
            }

            if (returnType.BaseType != null)
            {
                if (returnType.BaseType.Name.Contains("Task")) //fast and dirty check for better performace and lesser allocations
                {
                    if (returnType.BaseType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) == "global::System.Threading.Tasks.Task")
                    {
                        return true;
                    }
                }
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

            if (returnType.Name == "ConfiguredTaskAwaitable"
                || returnType.Name == "ConfiguredValueTaskAwaitable"
                )
            {
                return true;
            }

            if (returnType.ToDisplayString() == "System.Runtime.CompilerServices.YieldAwaitable")
            {
                //no need to guard this clause:
                //await Task.Yield()
                return true;
            }

            return false;
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
