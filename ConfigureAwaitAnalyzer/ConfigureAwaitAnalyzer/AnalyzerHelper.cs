using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConfigureAwaitAnalyzer
{
    internal static class AnalyzerHelper
    {
        internal static bool CheckIfPropertyOK(this IPropertySymbol propertySymbol)
        {
            if (CheckIfPropertyReturnsAwaitableTask(propertySymbol))
            {
                return true;
            }
            if (CheckIfSymbolSuppressed(propertySymbol))
            {
                return true;
            }

            return false;
        }

        private static bool CheckIfPropertyReturnsAwaitableTask(IPropertySymbol propertySymbol)
        {
            var propertyType = propertySymbol.Type;
            if (propertyType == null)
            {
                return false;
            }

            if (propertyType.Name.Contains("TaskScheduler")) //fast and dirty check for better performance and lesser allocations
            {
                var fqName = propertyType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

                if (fqName == "global::System.Threading.Tasks.TaskScheduler") //awaiting TaskScheduler does not require ConfigureAwait of any sort
                {
                    return true;
                }
            }

            return propertyType?.Name == "ConfiguredTaskAwaitable";
        }

        internal static bool CheckIfMethodOK(this IMethodSymbol methodSymbol)
        {
            if (CheckIfMethodReturnsAwaitableTask(methodSymbol))
            {
                return true;
            }
            if (CheckIfSymbolSuppressed(methodSymbol))
            {
                return true;
            }

            return false;
        }

        

        private static bool CheckIfSymbolSuppressed(ISymbol methodSymbol)
        {
            for (var dsri = 0; dsri < methodSymbol.DeclaringSyntaxReferences.Length; dsri++)
            {
                var dsr = methodSymbol.DeclaringSyntaxReferences[dsri];
                var s = dsr.GetSyntax();
                var leadingTrivia = s.GetLeadingTrivia();
                foreach (var trivia in leadingTrivia)
                {
                    if (trivia.IsKind(SyntaxKind.SingleLineCommentTrivia))
                    {
                        if (trivia.ToString() == ConfigureAwaitAnalyzerAnalyzer.SuppressComment)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private static bool CheckIfMethodReturnsAwaitableTask(IMethodSymbol methodSymbol)
        {
            //check if method is returning awaitable task
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
