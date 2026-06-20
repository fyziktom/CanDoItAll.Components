# SB03 Semantic Invariants: Public C# API freeze baseline

## SB03-RC

Raw note owned: stabilize and freeze the generic Components WebGL engine without adding consuming-app semantics.

Expected behavior: WebGlLib and WebGlRunLib public C# approval snapshots were regenerated and guarded by focused freeze tests.

Disallowed shallow implementation: a change that only updates incomplete proof markers, snapshots, or diagnostics text without enforcing the runtime/source contract.

Failing-first or closed-gap proof: bundle://proof/SB03/transcripts/failing-first-or-closed-gap.txt

Passing proof: bundle://proof/SB16/transcripts/dotnet-test-webgllib-final.txt; bundle://proof/SB16/transcripts/dotnet-test-webglrunlib-final.txt

Changed source files and hashes: bundle://proof/SB16/changed-file-hashes.txt

Production assertions: repo://tests/CanDoItAll.Components.WebGlLib.Tests/fixtures/approvals/webgllib-public-api.approved.txt; repo://tests/CanDoItAll.Components.WebGlRunLib.Tests/fixtures/approvals/webglrunlib-public-api.approved.txt

Adversarial negative case: Approval tests fail if public C# API changes without approved snapshot update.

Semantic positive case: Public API approval tests pass for WebGlLib and WebGlRunLib.

Downstream dependency check: final SB16 build, tests, package proof, browser proof, and domain-boundary audits passed.

## Production Behavior Artifact Matrix

| Artifact | Producer proof | Consumer proof | Lifecycle proof | Negative proof |
|---|---|---|---|---|
| SB03-RC behavior | repo://tests/CanDoItAll.Components.WebGlLib.Tests/fixtures/approvals/webgllib-public-api.approved.txt; repo://tests/CanDoItAll.Components.WebGlRunLib.Tests/fixtures/approvals/webglrunlib-public-api.approved.txt | bundle://proof/SB16/transcripts/dotnet-test-webgllib-final.txt; bundle://proof/SB16/transcripts/dotnet-test-webglrunlib-final.txt | bundle://proof/SB03/transcripts/implementation-validation.txt | bundle://proof/SB03/transcripts/failing-first-or-closed-gap.txt |

## Closure

Status: Solved. This invariant is backed by existing transcripts and the final bundle closure report.


