# SB10 Semantic Invariants

| Invariant ID | Shallow-pass trap | Adversarial negative proof | Semantic positive proof | Anti-stub audit |
| --- | --- | --- | --- | --- |
| SB10-INV-01 | Loading/projecting without executing runtime commands would not prove executable readiness. | The headless probe converts the selected run frame to a command batch and asserts staged commands exist. | `WebGlRunPlaybackController.ApplyDetailedAsync` and `WebGlRunFrameApplyResult.FromFrame` run against production documents. | Both fixtures use real input packs and strict loader hash validation. |
| SB10-INV-02 | Snapshot analysis could be detached from playback state. | The test seeks a selected step before snapshot and analysis. | Runtime snapshot current frame index matches the session run frame and includes current stage ids. | The assertions require visual-stage-pressure findings from the production analyzer. |
