# SB03 semantic invariants

Status: completed

## Invariants

- Semantic idle is true only when browser runtime work that can change simulation state has drained: active motions, queued motions, queued command stages, barriers, automatic stage work, and pending asset disposal must be absent.
- Visual idle is tracked separately from semantic idle. A runtime can be semantically idle while a final render frame is still scheduled.
- A scheduled-only final render may be treated as drained only after an explicit final-render drain marker or two consecutive semantic-idle probes.
- Active motion is never hidden by final-render drain logic.
- Continuous render mode remains a visual blocker and does not pass the scheduled-only final render drain rule.
- Runtime diagnostics and idle result interop expose `semanticIdle`, `visualIdle`, and `finalRenderDrained` to browser proof and Blazor consumers.

## Proof hooks

- `bundle://proof/SB03/js/runtime-idle-final-render-assertions.json`
- `bundle://proof/SB03/browser/runtime-idle-semantics-assertions.json`
- `bundle://proof/SB03/transcripts/webgllib-tests.txt`
