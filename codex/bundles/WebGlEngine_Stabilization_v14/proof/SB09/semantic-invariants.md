# SB09 Semantic Invariants: Source provenance opacity

## SB09-RC

Raw note owned: stabilize and freeze the generic Components WebGL engine without adding consuming-app semantics.

Expected behavior: Source provenance validator keys support opaque provenance and trace-map references without interpreting consumer-specific meaning.

Disallowed shallow implementation: a change that only updates incomplete proof markers, snapshots, or diagnostics text without enforcing the runtime/source contract.

Failing-first or closed-gap proof: bundle://proof/SB09/transcripts/failing-first-or-closed-gap.txt

Passing proof: bundle://proof/SB16/transcripts/dotnet-test-webglrunlib-final.txt

Changed source files and hashes: bundle://proof/SB16/changed-file-hashes.txt

Production assertions: repo://src/CanDoItAll.Components.WebGlRunLib/Documents/WebGlRunDocumentValidator.cs; repo://tests/CanDoItAll.Components.WebGlRunLib.Tests/WebGlRunValidatorTests.cs

Adversarial negative case: Domain-boundary hard gates remain active after provenance opacity expansion.

Semantic positive case: Validator tests prove allowed opaque provenance keys pass while domain-shaped leakage remains covered by audits.

Downstream dependency check: final SB16 build, tests, package proof, browser proof, and domain-boundary audits passed.

## Production Behavior Artifact Matrix

| Artifact | Producer proof | Consumer proof | Lifecycle proof | Negative proof |
|---|---|---|---|---|
| SB09-RC behavior | repo://src/CanDoItAll.Components.WebGlRunLib/Documents/WebGlRunDocumentValidator.cs; repo://tests/CanDoItAll.Components.WebGlRunLib.Tests/WebGlRunValidatorTests.cs | bundle://proof/SB16/transcripts/dotnet-test-webglrunlib-final.txt | bundle://proof/SB09/transcripts/implementation-validation.txt | bundle://proof/SB09/transcripts/failing-first-or-closed-gap.txt |

## Closure

Status: Solved. This invariant is backed by existing transcripts and the final bundle closure report.


