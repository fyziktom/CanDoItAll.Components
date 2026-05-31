# SB05 proof manifest

## Scope

Components C# and JS command-batch parity and performance.

## Changed files

- `repo://CanDoItAll.Components/tests/CanDoItAll.Components.WebGlLib.Tests/WebGlSceneCommandBatchTests.cs`
- `repo://CanDoItAll.Components/tools/webgllib/audit-sharedwell-performance.cjs`

## Proof

- Transcript: `bundle://proof/SB02/transcripts/components-validation.txt`
- Performance transcript: `bundle://proof/SB18/transcripts/performance-validation.txt`
- Anti-stub audit: `bundle://proof/SB02/transcripts/anti-stub-audit.txt`
- Semantic invariants: `bundle://proof/SB05/semantic-invariants.md`

## Production Behavior Artifact Matrix

| Producer | Consumer | Lifecycle | Negative proof |
| --- | --- | --- | --- |
| C# `WebGlSceneCommandBatchNormalizer` and JS `normalizeCommandBatch` | runtime command batch execution and audits | same fixtures -> C# test + JS parity audit | Staged repeated motions fixture proves ordered duplicates are not dropped. |
