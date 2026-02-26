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
    public const string DisallowManyElementsDiagnosticId = "CEK002";
    public const string DisallowLongExpressionDiagnosticId = "CEK003";
    public const string DisallowMultilineDiagnosticId = "CEK004";
    public const string DisallowAnyElementsDiagnosticId = "CEK005";
    public const string DiagnosticCategory = "CollectionExprKiller";
    private const int ManyElementsThreshold = 3;
    private const int LongExpressionThreshold = 12;

    private static readonly DiagnosticDescriptor DisallowAllRule = new(
        id: DisallowAllDiagnosticId,
        title: "Collection expressions are disallowed",
        messageFormat: "Collection expressions are not allowed",
        category: DiagnosticCategory,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Disallows any collection expression syntax.");

    private static readonly DiagnosticDescriptor DisallowManyElementsRule = new(
        id: DisallowManyElementsDiagnosticId,
        title: $"Collection expressions with more than {ManyElementsThreshold} elements are disallowed",
        messageFormat: $"Collection expressions must have {ManyElementsThreshold} or fewer elements",
        category: DiagnosticCategory,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: $"Disallows collection expressions when they have more than {ManyElementsThreshold} elements.");

    private static readonly DiagnosticDescriptor DisallowLongExpressionRule = new(
        id: DisallowLongExpressionDiagnosticId,
        title: "Long collection expression text is disallowed",
        messageFormat: $"Collection expression text length must be {LongExpressionThreshold} or fewer characters",
        category: DiagnosticCategory,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: $"Disallows collection expressions whose string representation is longer than {LongExpressionThreshold} characters.");

    private static readonly DiagnosticDescriptor DisallowMultilineRule = new(
        id: DisallowMultilineDiagnosticId,
        title: "Multiline collection expressions are disallowed",
        messageFormat: "Collection expressions must be on a single line",
        category: DiagnosticCategory,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Disallows collection expressions that span multiple lines.");

    private static readonly DiagnosticDescriptor DisallowAnyElementsRule = new(
        id: DisallowAnyElementsDiagnosticId,
        title: "Collection expressions with elements are disallowed",
        messageFormat: "Collection expressions must be empty",
        category: DiagnosticCategory,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Disallows collection expressions when they have one or more elements.");

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(DisallowAllRule, DisallowManyElementsRule, DisallowLongExpressionRule, DisallowMultilineRule, DisallowAnyElementsRule);

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

        if (expression.Elements.Count > 0)
        {
            context.ReportDiagnostic(Diagnostic.Create(
                DisallowAnyElementsRule,
                expression.GetLocation()));
        }

        if (expression.Elements.Count > ManyElementsThreshold)
        {
            context.ReportDiagnostic(Diagnostic.Create(
                DisallowManyElementsRule,
                expression.GetLocation()));
        }

        if (expression.Span.Length > LongExpressionThreshold)
        {
            context.ReportDiagnostic(Diagnostic.Create(
                DisallowLongExpressionRule,
                expression.GetLocation()));
        }

        var lineSpan = expression.GetLocation().GetLineSpan();
        if (lineSpan.StartLinePosition.Line != lineSpan.EndLinePosition.Line)
        {
            context.ReportDiagnostic(Diagnostic.Create(
                DisallowMultilineRule,
                expression.GetLocation()));
        }
    }
}
