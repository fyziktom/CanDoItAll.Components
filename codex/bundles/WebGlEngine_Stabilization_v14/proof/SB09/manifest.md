# SB09 Proof Manifest: Source provenance opacity

Status: Completed
Invariant contract: bundle://proof/SB09/semantic-invariants.md
Changed-file hashes: bundle://proof/SB16/changed-file-hashes.txt

## Owned Requirements

- Components-only implementation scope.
- Source provenance opacity.
- Preserve generic WebGlLib/WebGlRunLib boundaries and public API approval discipline.

## Proof Artifacts

| Evidence | Path |
|---|---|
| Failing-first or closed-gap | bundle://proof/SB09/transcripts/failing-first-or-closed-gap.txt |
| Implementation validation | bundle://proof/SB09/transcripts/implementation-validation.txt |
| Domain-boundary audit | bundle://proof/SB09/transcripts/domain-boundary-audit.txt |
| Changed files | bundle://proof/SB09/transcripts/changed-files.txt |
| Source assertions | bundle://proof/SB09/transcripts/source-assertions.txt |
| Anti-stub audit | bundle://proof/SB09/transcripts/anti-stub-audit.txt |
| Primary passing proof | bundle://proof/SB16/transcripts/dotnet-test-webglrunlib-final.txt |

## Source-Level Assertions

repo://src/CanDoItAll.Components.WebGlRunLib/Documents/WebGlRunDocumentValidator.cs; repo://tests/CanDoItAll.Components.WebGlRunLib.Tests/WebGlRunValidatorTests.cs

## Semantic Adequacy Gate

- Invariant id: SB09-RC
- Shallow-pass trap: accepting file existence, approval snapshot churn, or panel counters without proving the runtime/source behavior.
- Adversarial negative proof: Domain-boundary hard gates remain active after provenance opacity expansion.
- Semantic positive proof: Validator tests prove allowed opaque provenance keys pass while domain-shaped leakage remains covered by audits.
- Anti-stub audit: bundle://proof/SB16/transcripts/anti-stub-audit-owned-final.txt
- Raw-note closure: Solved for SB09; no scope words were narrowed and no consuming-app semantics were added to Components.

## Production Behavior Artifact Matrix

| Artifact | Producer proof | Consumer proof | Lifecycle proof | Negative proof |
|---|---|---|---|---|
| SB09-RC behavior | repo://src/CanDoItAll.Components.WebGlRunLib/Documents/WebGlRunDocumentValidator.cs; repo://tests/CanDoItAll.Components.WebGlRunLib.Tests/WebGlRunValidatorTests.cs | bundle://proof/SB16/transcripts/dotnet-test-webglrunlib-final.txt | bundle://proof/SB09/transcripts/implementation-validation.txt | bundle://proof/SB09/transcripts/failing-first-or-closed-gap.txt |

## Closure Decision

Pass. Required proof artifacts are present, source references are portable, domain-boundary gates pass, and final validation is cited from the transcripts above.


