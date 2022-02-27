using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;

namespace ConfigureAwaitAnalyzer
{
    /// <summary>
    /// This analyzer is partially based on https://github.com/edumserrano/roslyn-analyzers
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ConfigureAwaitAnalyzerAnalyzer : DiagnosticAnalyzer
    {
        private const string Category = "Async";
        public const string DiagnosticId = "AsyncConfigureAwait";

        // You can change these strings in the Resources.resx file. If you do not want your analyzer to be localize-able, you can use regular strings for Title and MessageFormat.
        // See https://github.com/dotnet/roslyn/blob/main/docs/analyzers/Localizing%20Analyzers.md for more on localization
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.AnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.AnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.AnalyzerDescription), Resources.ResourceManager, typeof(Resources));

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(AnalyzeAwaitExpression, SyntaxKind.AwaitExpression);
        }

        private static void AnalyzeAwaitExpression(SyntaxNodeAnalysisContext context)
        {
            var awaitExpression = context.TryGetSyntaxNode<AwaitExpressionSyntax>();
            if (awaitExpression == null)
            {
                return;
            }

            var semanticModel = context.SemanticModel;
            if (semanticModel == null)
            {
                return;
            }

            var expression = awaitExpression.Expression;
            if (expression == null)
            {
                return;
            }

            var symbol = semanticModel.GetSymbolInfo(expression, context.CancellationToken).Symbol;
            if (symbol is IMethodSymbol methodSymbol)
            {
                if (methodSymbol.ReturnsAwaitableTask())
                {
                    return;
                }

                context.ReportDiagnostic(Diagnostic.Create(Rule, awaitExpression.GetLocation()));
            }
            else if (symbol is IPropertySymbol propertySymbol)
            {
                if (propertySymbol.ReturnsAwaitableTask())
                {
                    return;
                }

                context.ReportDiagnostic(Diagnostic.Create(Rule, awaitExpression.GetLocation()));
            }
        }
    }
}
