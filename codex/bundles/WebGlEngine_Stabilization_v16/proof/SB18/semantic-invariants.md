# Semantic invariants - SB18

- Diagnostics are generic: command counts, render count, object count, motion counts, cache counts, patch classifications, link rebuilds, and canvas dimensions.
- The dashboard must not introduce Economy or production-line domain statistics.
- Large-scene proof must render real canvas pixels, not only DOM text.
- Command batch payloads returned to Blazor remain compact enough for browser proof to complete.

## Production Behavior Artifact Matrix

| Behavior | Producer | Consumer | Lifecycle | Proof |
|---|---|---|---|---|
| Large scene dashboard metrics | `PerformanceProof.razor.cs` | browser proof JSON | route initialization and batch apply | `bundle://proof/SB18/browser/performance-proof-browser.json` |
| Canvas rendering | WebGlLib runtime | pixel sampler | after route settles | `bundle://proof/SB18/browser/performance-proof-pixels.json` |
