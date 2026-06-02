# SB07 Semantic Invariants

Subbundle: `SB07-forced-webgllib-boundary-refactor-gate`
Status: `Completed`

## Invariants

| Invariant | Claim | Evidence | Status |
| --- | --- | --- | --- |
| SB07-BOUNDARY-001 | WebGlLib has no direct dependency on WebGlRunLib or Economy/domain packages. | `repo://CanDoItAll.Components/tools/webgllib/audit-webgllib-boundary.cjs`, `bundle://proof/SB07/transcripts/passing-webgllib-boundary-audit.txt` | Passed |
| SB07-BOUNDARY-002 | The dependency direction remains WebGlRunLib to WebGlLib, never WebGlLib to WebGlRunLib. | `repo://CanDoItAll.Components/docs/webgl/run-layer-boundary.md`, `bundle://proof/SB07/transcripts/sb07-source-assertions.txt` | Passed |
| SB07-SAMPLE-001 | A consumer can build a minimal WebGlLib-only viewer without referencing WebGlRunLib. | `repo://CanDoItAll.Components/samples/CanDoItAll.Components.WebGlLibOnlyViewer`, `bundle://proof/SB07/transcripts/passing-webgllib-only-sample-build.txt` | Passed |
| SB07-FENCE-001 | WebGlLib command batches and command stages are fenced as render-command transport, not run/replay/action semantics. | `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/README.md`, `bundle://proof/SB07/boundary-audit.md` | Passed |
| SB07-NEGATIVE-001 | The static audit fails when a forbidden WebGlRunLib reference is introduced through the probe. | `bundle://proof/SB07/transcripts/failing-boundary-audit-probe.txt` | Passed |

## Shallow-Pass Trap

A shallow SB07 implementation could pass a one-line csproj grep while leaving domain terms in first-party source, failing to prove independent WebGlLib consumption, or leaving ambiguous docs around command stages. SB07 rejects that trap by adding a reusable audit script, an adversarial probe, a buildable WebGlLib-only sample, and docs that name allowed and forbidden layer ownership.

## Production Behavior Artifact Matrix

| Artifact | Producer | Consumer | Lifecycle | Negative proof |
| --- | --- | --- | --- | --- |
| Boundary audit script | `tools/webgllib/audit-webgllib-boundary.cjs` | Local/CI command `npm run webgllib:audit-boundary` | Scans project references and first-party source on demand, failing nonzero for forbidden dependency/domain terms. | `bundle://proof/SB07/transcripts/failing-boundary-audit-probe.txt` |
| WebGlLib-only sample | `samples/CanDoItAll.Components.WebGlLibOnlyViewer` | Maintainers and consumers proving lightweight usage | Compiles independently as a Razor class library sample. | `bundle://proof/SB07/transcripts/passing-webgllib-boundary-audit.txt` |

## Closure

SB07 closes Gate F: WebGlLib remains an ultra-light generic render substrate and SB08 may begin WebGlRunLib contract stabilization.
