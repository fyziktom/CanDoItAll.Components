# SB12 Semantic Invariants: Resource ownership and asset-cache stress

## SB12-RC

Raw note owned: stabilize and freeze the generic Components WebGL engine without adding consuming-app semantics.

Expected behavior: Resource ownership audit proves owned instance disposal, shared texture retention, template disposal, and pending-cache cleanup.

Disallowed shallow implementation: a change that only updates incomplete proof markers, snapshots, or diagnostics text without enforcing the runtime/source contract.

Failing-first or closed-gap proof: bundle://proof/SB12/transcripts/failing-first-or-closed-gap.txt

Passing proof: bundle://proof/SB12/transcripts/resource-ownership-final.txt

Changed source files and hashes: bundle://proof/SB16/changed-file-hashes.txt

Production assertions: repo://tools/webgllib/test-resource-ownership.mjs; repo://src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene

Adversarial negative case: Resource audit rejects double disposal and shared texture disposal mistakes.

Semantic positive case: Resource ownership audit returns pass=true for all resource lifecycle cases.

Downstream dependency check: final SB16 build, tests, package proof, browser proof, and domain-boundary audits passed.

## Production Behavior Artifact Matrix

| Artifact | Producer proof | Consumer proof | Lifecycle proof | Negative proof |
|---|---|---|---|---|
| SB12-RC behavior | repo://tools/webgllib/test-resource-ownership.mjs; repo://src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene | bundle://proof/SB12/transcripts/resource-ownership-final.txt | bundle://proof/SB12/transcripts/implementation-validation.txt | bundle://proof/SB12/transcripts/failing-first-or-closed-gap.txt |

## Closure

Status: Solved. This invariant is backed by existing transcripts and the final bundle closure report.


