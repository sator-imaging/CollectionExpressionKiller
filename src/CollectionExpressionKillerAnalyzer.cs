// (c) 2026 Sator Imaging
// https://github.com/sator-imaging
// Licensed under the MIT License

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace CollectionExpressionKiller;

#pragma warning disable RS2008  // Enable analyzer release tracking

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class CollectionExpressionKillerAnalyzer : DiagnosticAnalyzer
{
    public const string DisallowAllDiagnosticId = "CEK001";
    public const string DisallowLessThanFourDiagnosticId = "CEK002";
    public const string DisallowLongTextDiagnosticId = "CEK003";
    private const int MinElementsForSecondDiagnostic = 4;
    private const int MaxAllowedExpressionTextLength = 12;

    private static readonly DiagnosticDescriptor DisallowAllRule = new(
        id: DisallowAllDiagnosticId,
        title: "Collection expressions are disallowed",
        messageFormat: "Collection expressions are not allowed",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Disallows any collection expression syntax.");

    private static readonly DiagnosticDescriptor DisallowLessThanFourRule = new(
        id: DisallowLessThanFourDiagnosticId,
        title: "Collection expressions with 4 or more elements are disallowed",
        messageFormat: "Collection expressions must have fewer than 4 elements",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Disallows collection expressions when they have 4 or more elements.");

    private static readonly DiagnosticDescriptor DisallowLongTextRule = new(
        id: DisallowLongTextDiagnosticId,
        title: "Long collection expression text is disallowed",
        messageFormat: "Collection expression text length must be 12 or fewer characters",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Disallows collection expressions whose string representation is longer than 12 characters.");

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(DisallowAllRule, DisallowLessThanFourRule, DisallowLongTextRule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(
            static syntaxContext => AnalyzeCollectionExpression(syntaxContext),
            SyntaxKind.CollectionExpression);
    }

    private static void AnalyzeCollectionExpression(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not CollectionExpressionSyntax expression)
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(
            DisallowAllRule,
            expression.GetLocation()));

        if (expression.Elements.Count >= MinElementsForSecondDiagnostic)
        {
            context.ReportDiagnostic(Diagnostic.Create(
                DisallowLessThanFourRule,
                expression.GetLocation()));
        }

        if (expression.Span.Length > MaxAllowedExpressionTextLength)
        {
            context.ReportDiagnostic(Diagnostic.Create(
                DisallowLongTextRule,
                expression.GetLocation()));
        }
    }
}
