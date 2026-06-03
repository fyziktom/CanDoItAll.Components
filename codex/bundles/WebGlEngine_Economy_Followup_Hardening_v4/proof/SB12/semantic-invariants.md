# SB12 semantic invariants

Status: Completed.

## Invariants

| Id | Invariant | Status | Evidence |
| --- | --- | --- | --- |
| SB12-I01 | The implementation preserves Components genericity and does not introduce Economy semantics into WebGlLib or WebGlRunLib production behavior. | Pass | Boundary/source audits: `bundle://proof/SB12/transcripts/boundary-audits.txt`, `bundle://proof/SB12/transcripts/source-assertions.txt`. |
| SB12-I02 | Runtime proof demonstrates behavior, not only compilation. | Pass | Browser artifact audit over SB04/SB10/SB11: `bundle://proof/SB12/transcripts/browser-proof.txt`. |
| SB12-I03 | Final proof artifacts are non-empty and assertion-backed. | Pass | Red-team review and final validators: `bundle://proof/SB12/transcripts/red-team-review.txt`, `bundle://proof/SB12/transcripts/validator-audits.txt`. |
| SB12-I04 | Package-mode and project-reference consumers both remain viable. | Pass | SB12 package-mode proof restores/builds package mode and restores back to project-reference mode: `bundle://proof/SB12/transcripts/package-mode-proof.txt`. |
| SB12-I05 | Final red-team findings are fixed before closure. | Pass | Failing-first file-size audit and post-refactor tests/audits: `bundle://proof/SB12/transcripts/failing-first.txt`, `bundle://proof/SB12/transcripts/passing-tests.txt`, `bundle://proof/SB12/transcripts/boundary-audits.txt`. |

## Production Behavior Artifact Matrix

| Production signal / state | Producer | Consumer | Lifecycle | Proof |
| --- | --- | --- | --- | --- |
| Explicit replay application | WebGlRunLib adapter and Economy sandbox replay bridge | Browser route proof and focused tests | Initial scene reset and ordered frames are applied to target frame | `bundle://proof/SB04/browser/runtime-diagnostics.json`, `bundle://proof/SB12/transcripts/economy-tests.txt` |
| Compact large-scene lifecycle key and budget profiles | WebGlSceneView and WebGlRuntimeBudgetProfiles | Sandbox performance route and WebGlLib tests | Large-scene render avoids full payload keying and exposes budget counters | `bundle://proof/SB11/browser/performance-proof-diagnostics.json`, `bundle://proof/SB12/transcripts/components-tests.txt` |
| Generic package boundaries | WebGlLib/WebGlRunLib project and source layouts | Package consumers and Economy bridge | Components packages expose generic contracts; Economy owns domain mapping | `bundle://proof/SB12/transcripts/boundary-audits.txt`, `bundle://proof/SB12/transcripts/package-mode-proof.txt` |
| Artifact-backed closure | SB12 proof manifest and execution report | Bundle validators and proof audit | Completed manifests cite concrete files and validators confirm bundle structure/proof | `bundle://proof/SB12/transcripts/validator-audits.txt` |
