# Assumptions And Risks

## Critical Path Risks

| Risk | Impact | Mitigation |
|---|---|---|
| Snapshot model grows WebGL-specific fields into the canonical contract. | Simulation snapshots would not remain renderer-neutral. | Keep visual state optional and require source assertions in SB07/SB09. |
| Bridge projection can pass tests while silently dropping unsupported actions. | Visual playback would be incomplete but appear successful. | Require diagnostics, adversarial tests, and non-empty executable stage proof in SB05. |
| Example terms leak into generic abstractions while tests remain green. | Future demos overfit shared-resource or finite-resource probes. | Run forbidden-term scans and boundary audit in SB10 and final closure. |
| JS runtime module hardening expands warning-sized files. | Runtime maintainability degrades. | Run module audit before and after Components edits; split before adding behavior to warning-sized modules. |

## Validation Risks

| Risk | Validation response |
|---|---|
| Baseline tests fail before implementation. | Record failing-first transcript and decide whether the failure is in-scope before editing. |
| `pwsh` is unavailable for Economy audit. | Use Windows PowerShell only if the script is compatible, and record the shell used in the transcript. |
| UI proof is requested accidentally. | This bundle adds foundations, not a finished demo route; browser proof is only required if a UI route is changed. |
| Performance numbers vary by machine. | Record probe JSON and use bounded smoke thresholds rather than absolute benchmark claims. |

## Reopen Triggers

| Trigger | Reopen |
|---|---|
| A later bridge/snapshot proof reveals dropped stages or missing diagnostics. | Reopen SB05 and any dependent SB11/SB14 proof. |
| Snapshot hash changes after JSON roundtrip. | Reopen SB07/SB08. |
| Boundary audit finds Components -> Economy or bridge -> backend references. | Reopen SB06/SB10. |
| Final validation has missing transcripts, hashes, or semantic proof for a critical subbundle. | Reopen the affected subbundle before SB15 closure. |
