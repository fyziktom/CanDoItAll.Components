# SB08 Semantic Invariants

Subbundle: `SB08-webglrunlib-contract-stabilization`
Status: `Completed`

## Invariants

| Invariant | Claim | Evidence | Status |
| --- | --- | --- | --- |
| SB08-DOC-001 | Run documents have a public validator that rejects unsupported schema, missing run id, invalid timeline, invalid frame/stage shape, invalid barriers, and obvious domain leakage. | `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlRunLib/Documents/WebGlRunDocumentValidator.cs`, `bundle://proof/SB08/transcripts/passing-webglrun-validators-and-compiler.txt` | Passed |
| SB08-PLAN-001 | Action plans have a public validator that catches normalization errors, structural action errors, invalid policies, direct command errors, and domain leakage before compilation. | `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlRunLib/Actions/WebGlRunActionPlanValidator.cs`, `bundle://proof/SB08/transcripts/passing-webglrun-validators-and-compiler.txt` | Passed |
| SB08-COMPILE-001 | Compiler/batch contracts preserve sequential stages, parallel coalescing, wait/event barriers, object-motion barriers, and direct scene patch batches. | `repo://CanDoItAll.Components/tests/CanDoItAll.Components.WebGlRunLib.Tests/WebGlRunActionCompilerTests.cs`, `bundle://proof/SB08/transcripts/passing-webglrun-validators-and-compiler.txt` | Passed |
| SB08-BOUNDARY-001 | WebGlRunLib depends on WebGlLib only and contains no forbidden Economy/ledger/market/production domain terms in first-party source. | `repo://CanDoItAll.Components/tools/webgllib/audit-webglrunlib-boundary.cjs`, `bundle://proof/SB08/transcripts/passing-webglrunlib-boundary-audit.txt` | Passed |
| SB08-DOCS-001 | WebGlRunLib contracts, validators, and boundary are documented before runtime integration starts. | `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlRunLib/README.md`, `repo://CanDoItAll.Components/docs/webgl/run-layer-boundary.md` | Passed |

## Shallow-Pass Trap

A weak SB08 could add a README for run contracts and rely on existing planner tests, while leaving invalid run documents or domain-specific action plans able to reach compilation. SB08 rejects that trap with failing-first validator tests, public validators, adversarial boundary audit proof, compile parity coverage, and full WebGlRunLib test proof.

## Production Behavior Artifact Matrix

| Artifact | Producer | Consumer | Lifecycle | Negative proof |
| --- | --- | --- | --- | --- |
| Run document validation result | `WebGlRunDocumentValidator` | Playback setup and future bridge mappers | Computed per validation call from document/timeline/scene/frame/stage state. | `bundle://proof/SB08/transcripts/failing-first-webglrun-validators.txt` |
| Action plan validation result | `WebGlRunActionPlanValidator` | Planners, compilers, playback integration, future bridge mappers | Computed per validation call from normalized action trees and direct commands. | `bundle://proof/SB08/transcripts/failing-first-webglrun-validators.txt` |
| Boundary audit result | `tools/webgllib/audit-webglrunlib-boundary.cjs` | Local/CI gate runners | Scans project references and first-party source on demand. | `bundle://proof/SB08/transcripts/failing-webglrunlib-boundary-probe.txt` |

## Closure

SB08 closes WebGlRunLib contract stabilization for REQ-010 and the contract/validator slice of REQ-011. SB09 remains responsible for browser-visible runtime playback integration over these contracts.
