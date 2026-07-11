# CanDoItAll.Components.FileBrowser.Providers.FileSystem

A root-confined local filesystem adapter for `CanDoItAll.Components.FileBrowser.Core`.

The provider exposes one configured directory as an opaque, shallow, paged hierarchy. It normalizes occurrence identity relative to that root, refuses path escape, does not follow directory links by default, and reports only capabilities it can execute safely. Root confinement is a browsing invariant, not an operating-system authorization boundary; run the host under an appropriately restricted identity.

Shallow means it never traverses descendants of the requested directory. To produce deterministic sorted pages and a mutation-sensitive consistency token, each browse request still enumerates all visible direct children and captures required metadata before slicing the page in memory. This adapter therefore avoids eager hierarchy loading but does not claim native filesystem I/O paging for extremely large single directories.

Source descriptions and standard display/copy paths expose absolute server paths. Do not use the default projection where those paths are sensitive; wrap the provider with a host-specific presentation policy.

Reparse-point following is disabled by default. Enabling it assumes a trusted, stable namespace: target validation followed by logical-path reopening is vulnerable to a time-of-check/time-of-use replacement in attacker- or agent-writable roots. A hard race-free policy would require handle-based traversal.

The shipped provider advertises no open, download, or content-read capability and performs no browser download hosting. Consumers must wrap or extend it with an authorized content/action provider, or act on host-owned selection callbacks; those operations do not work automatically.

## Validate

```powershell
dotnet build src/CanDoItAll.Components.FileBrowser.Providers.FileSystem/CanDoItAll.Components.FileBrowser.Providers.FileSystem.csproj
dotnet test tests/CanDoItAll.Components.FileBrowser.Providers.FileSystem.Tests/CanDoItAll.Components.FileBrowser.Providers.FileSystem.Tests.csproj
```
