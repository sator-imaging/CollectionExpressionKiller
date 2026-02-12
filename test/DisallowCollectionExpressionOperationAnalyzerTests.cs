using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using Xunit;

namespace CollectionExpressionKiller.Tests;

public sealed class CollectionExpressionKillerTests
{
    [Fact]
    public async Task ReportsBothDiagnosticsForCollectionExpressionWithFourElements()
    {
        const string source = """
            using System.Collections.Generic;

            public static class C
            {
                public static IReadOnlyList<int> M()
                {
                    return [1, 2, 3, 4];
                }
            }
            """;

        var diagnostics = await GetDiagnosticsAsync(source);

        AssertDiagnosticIds(
            diagnostics,
            CollectionExpressionKillerAnalyzer.DisallowAllDiagnosticId,
            CollectionExpressionKillerAnalyzer.DisallowLessThanFourDiagnosticId);
    }

    [Fact]
    public async Task ReportsOnlyDisallowAllDiagnosticForCollectionExpressionWithThreeElements()
    {
        const string source = """
            using System.Collections.Generic;

            public static class C
            {
                public static IReadOnlyList<int> M()
                {
                    return [1, 2, 3];
                }
            }
            """;

        var diagnostics = await GetDiagnosticsAsync(source);

        var diagnostic = Assert.Single(diagnostics);
        Assert.Equal(CollectionExpressionKillerAnalyzer.DisallowAllDiagnosticId, diagnostic.Id);
        Assert.Equal(DiagnosticSeverity.Error, diagnostic.Severity);
    }

    [Fact]
    public async Task ReportsBothDiagnosticsForCollectionExpressionWithSpreadAndFourElements()
    {
        const string source = """
            using System.Collections.Generic;

            public static class C
            {
                public static IReadOnlyList<int> M()
                {
                    int[] otherArray = new[] { 1, 2 };
                    return [..otherArray, 3, 4, 5];
                }
            }
            """;

        var diagnostics = await GetDiagnosticsAsync(source);

        AssertDiagnosticIds(
            diagnostics,
            CollectionExpressionKillerAnalyzer.DisallowAllDiagnosticId,
            CollectionExpressionKillerAnalyzer.DisallowLessThanFourDiagnosticId,
            CollectionExpressionKillerAnalyzer.DisallowLongTextDiagnosticId);
    }

    [Fact]
    public async Task ReportsOnlyDisallowAllDiagnosticForCollectionExpressionWithSpreadAndThreeElements()
    {
        const string source = """
            using System.Collections.Generic;

            public static class C
            {
                public static IReadOnlyList<int> M()
                {
                    int[] otherArray = new[] { 1, 2 };
                    return [..otherArray, 3, 4];
                }
            }
            """;

        var diagnostics = await GetDiagnosticsAsync(source);

        AssertDiagnosticIds(
            diagnostics,
            CollectionExpressionKillerAnalyzer.DisallowAllDiagnosticId,
            CollectionExpressionKillerAnalyzer.DisallowLongTextDiagnosticId);
    }

    [Fact]
    public async Task ReportsDisallowAllAndLongTextDiagnosticsForLongSingleElementCollectionExpression()
    {
        const string source = """
            using System.Collections.Generic;

            public static class C
            {
                public static IReadOnlyList<long> M()
                {
                    return [1234567890123];
                }
            }
            """;

        var diagnostics = await GetDiagnosticsAsync(source);

        AssertDiagnosticIds(
            diagnostics,
            CollectionExpressionKillerAnalyzer.DisallowAllDiagnosticId,
            CollectionExpressionKillerAnalyzer.DisallowLongTextDiagnosticId);
    }

    [Fact]
    public async Task ReportsAllDiagnosticsForLongCollectionExpressionWithFourElements()
    {
        const string source = """
            using System.Collections.Generic;

            public static class C
            {
                public static IReadOnlyList<int> M()
                {
                    return [10, 20, 30, 40];
                }
            }
            """;

        var diagnostics = await GetDiagnosticsAsync(source);

        AssertDiagnosticIds(
            diagnostics,
            CollectionExpressionKillerAnalyzer.DisallowAllDiagnosticId,
            CollectionExpressionKillerAnalyzer.DisallowLessThanFourDiagnosticId,
            CollectionExpressionKillerAnalyzer.DisallowLongTextDiagnosticId);
    }

    [Fact]
    public async Task DoesNotReportDiagnosticForNonCollectionExpression()
    {
        const string source = """
            using System.Collections.Generic;

            public static class C
            {
                public static IReadOnlyList<int> M()
                {
                    return new[] { 1, 2, 3 };
                }
            }
            """;

        var diagnostics = await GetDiagnosticsAsync(source);

        Assert.Empty(diagnostics);
    }

    private static async Task<ImmutableArray<Diagnostic>> GetDiagnosticsAsync(string source)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(
            source,
            new CSharpParseOptions(LanguageVersion.Preview));

        var compilation = CSharpCompilation.Create(
            assemblyName: "AnalyzerTests",
            syntaxTrees: new[] { syntaxTree },
            references: GetMetadataReferences(),
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var analyzer = new CollectionExpressionKillerAnalyzer();
        var withAnalyzers = compilation.WithAnalyzers(ImmutableArray.Create<DiagnosticAnalyzer>(analyzer));
        return await withAnalyzers.GetAnalyzerDiagnosticsAsync();
    }

    private static IEnumerable<MetadataReference> GetMetadataReferences()
    {
        return AppDomain.CurrentDomain.GetAssemblies()
            .Where(static assembly => !assembly.IsDynamic && !string.IsNullOrWhiteSpace(assembly.Location))
            .Select(static assembly => MetadataReference.CreateFromFile(assembly.Location))
            .GroupBy(static reference => reference.Display, StringComparer.Ordinal)
            .Select(static group => group.First());
    }

    private static void AssertDiagnosticIds(ImmutableArray<Diagnostic> diagnostics, params string[] expectedIds)
    {
        Assert.Equal(expectedIds.Length, diagnostics.Length);
        var actualIds = diagnostics.Select(static d => d.Id).OrderBy(static id => id).ToArray();
        var sortedExpected = expectedIds.OrderBy(static id => id).ToArray();
        Assert.Equal(sortedExpected, actualIds);
        Assert.All(diagnostics, static diagnostic => Assert.Equal(DiagnosticSeverity.Error, diagnostic.Severity));
    }
}
