# Provenance And Domain Boundary Architecture

## Current risk

`source.*` is exempted from generic domain term validation. That preserves Economy traceability but can also hide domain-specific execution semantics under generic metadata.

## Target

Use a typed or policy-validated provenance envelope.

Minimum viable target:

```csharp
public sealed class WebGlRunSourceProvenance
{
    public string SourceSystem { get; set; } = string.Empty;
    public string SourceRunId { get; set; } = string.Empty;
    public string SourceFrameId { get; set; } = string.Empty;
    public string SourceActionId { get; set; } = string.Empty;
    public string SourceEventId { get; set; } = string.Empty;
    public string SourceHash { get; set; } = string.Empty;
    public Dictionary<string, string> Tags { get; set; } = [];
}
```

Rules:
- Generic Components can carry source identifiers and hashes.
- Generic Components cannot interpret domain vocabulary.
- Domain bridge can convert domain provenance to generic provenance.
- `source.*` metadata values must be bounded by length and validated for non-executable semantics.
