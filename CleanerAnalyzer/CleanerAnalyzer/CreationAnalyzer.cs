using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace CleanerAnalyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class CreationAnalyzer : DiagnosticAnalyzer
    {
        private const string DiagnosticId = "WordyMethod";
        private const string Title = "Method contains too many words";
        private const string MessageFormat = "Method '{0}' contains {1} words, which is more than the allowed 10 words";
        private const string Description = "Methods should be concise and not contain too many words.";
        private const string Category = "Naming";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            DiagnosticId,
            Title,
            MessageFormat,
            Category,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(AnalyzeMethod, SyntaxKind.MethodDeclaration);
        }

        private void AnalyzeMethod(SyntaxNodeAnalysisContext context)
        {
            var methodDeclaration = (MethodDeclarationSyntax)context.Node;
            var methodBody = methodDeclaration.Body?.ToString() ?? methodDeclaration.ExpressionBody?.ToString() ?? string.Empty;

            var wordCount = CountWords(methodBody);

            if (wordCount > 10)
            {
                var diagnostic = Diagnostic.Create(Rule, methodDeclaration.Identifier.GetLocation(), methodDeclaration.Identifier.Text, wordCount);
                context.ReportDiagnostic(diagnostic);
            }
        }

        private int CountWords(string text)
        {
            return text.Split(new[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).Length;
        }
    }
}
