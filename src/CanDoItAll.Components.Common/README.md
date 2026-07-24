# CanDoItAll.Components.Common

## Purpose

Common is the small, dependency-light foundation beneath the rendering libraries. It contains CSS class helpers, component attribute helpers, and typed layout primitives such as orientation, alignment, justification, and wrapping. It is useful when a shared feature needs UI vocabulary without depending on Razor rendering or a browser runtime.

Most Blazor applications should reference `BaseLib` rather than Common directly. Reference Common when authoring a reusable library that needs these neutral contracts but should remain independent of the visual component package.

## Project Type

- SDK: `Microsoft.NET.Sdk`
- Target framework(s): `net10.0`
- Validation command:

```powershell
dotnet build src/CanDoItAll.Components.Common/CanDoItAll.Components.Common.csproj
```

## References

Project references:

- None

Framework references:

- None

Direct package references:

- None

## Architecture Notes

Common deliberately contains no components, JavaScript, or Tailwind output. Keep it generic and framework-light; move rendered UI to BaseLib or a specialised library. See the [main README](../../README.md) for the library map.

## Related Docs

- [Repository overview](../../README.md)
