<div align="center">

# Collection Expression Killer for C#

Ensures Explicit Coding Style for Code Review

[![nuget](https://img.shields.io/nuget/vpre/CollectionExpressionKiller)](https://www.nuget.org/packages/CollectionExpressionKiller)
[![test](https://github.com/sator-imaging/CollectionExpressionKiller/actions/workflows/test.yml/badge.svg)](https://github.com/sator-imaging/CollectionExpressionKiller/actions/workflows/test.yml)
&nbsp;
[![DeepWiki](https://deepwiki.com/badge.svg)](https://deepwiki.com/sator-imaging/CollectionExpressionKiller)

</div>


&nbsp;


# Why?

Collection expression is useful for small scripts, demonstrations or other non-critical use cases but it is unclear what is actually created behind the scenes.

The source code should be explicitly expressing itself and eliminating "Something working" from the repository is important especially on code review on GitHub (no IDE assist).



## Installation

```sh
dotnet add package CollectionExpressionKiller
```


## `.editorconfig` Recommendation

Disable the suggestions relating to collection expressions.

```ini
dotnet_diagnostic.IDE0300.severity = silent   # Use collection expression for array
dotnet_diagnostic.IDE0301.severity = silent   # Use collection expression for empty
dotnet_diagnostic.IDE0305.severity = silent   # Use collection expression for fluent
dotnet_diagnostic.IDE0028.severity = silent   # Use collection initializers
```





&nbsp;

# Diagnostics

Ability to disallow all expressions or complicated only.

- `CEK001`: Disallow all collection expressions.
- `CEK002`: Disallow expressions with 4 or more elements. (e.g., `[1, 2, ..other, 4]`)
- `CEK003`: Disallow expressions whose string representation is longer than 12 characters (including `[` and `]`).
- `CEK004`: Disallow multiline expressions.


## How to Disable Analyzer

Disable for entire `.cs` file with `#pragma`:

```csharp
#pragma warning disable CEK001
#pragma warning disable CEK002
#pragma warning disable CEK003
#pragma warning disable CEK004

int[] values = [1, 2, 3, 4, ..otherCollection];
```


Disable for entire assembly:

```csharp
using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage(
    "Usage",
    "CEK001:Collection expressions are not allowed",
    Justification = "Approved for this assembly")]

[assembly: SuppressMessage(
    "Usage",
    "CEK002:Collection expressions must have fewer than 4 elements",
    Justification = "Approved for this assembly")]

[assembly: SuppressMessage(
    "Usage",
    "CEK003:Collection expression text length must be 12 or fewer characters",
    Justification = "Approved for this assembly")]

[assembly: SuppressMessage(
    "Usage",
    "CEK004:Collection expressions must be on a single line",
    Justification = "Approved for this assembly")]
```


Disable for entire project with `.editorconfig`:

```ini
dotnet_diagnostic.CEK001.severity = none
dotnet_diagnostic.CEK002.severity = none
dotnet_diagnostic.CEK003.severity = none
dotnet_diagnostic.CEK004.severity = none
```
