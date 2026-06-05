# SB06 Semantic Invariants

## Invariants

- Generic Components production code must not embed economy-specific forbidden terms as default validation policy.
- Domain-specific rejection remains available only when a caller supplies `WebGlRunGenericBoundaryOptions.ForbiddenDomainTerms`.
- Source provenance metadata is traceability-only: keys are allowlisted, executable policy-like source keys are rejected, and values remain opaque.
- Domain leakage CI proof is scoped to generic production C# files under WebGlLib and WebGlRunLib, excluding build output.
- Tests may carry economy vocabulary only as fixture-driven negative coverage.

## Proof Links

- `bundle://proof/SB06/domain-leakage-report.json`
- `bundle://proof/SB06/transcripts/domain-leakage-scan.txt`
- `bundle://proof/SB06/transcripts/webglrunlib-tests.txt`
- `bundle://proof/SB06/transcripts/source-assertions.txt`
- `bundle://proof/SB06/transcripts/changed-file-hashes.txt`
