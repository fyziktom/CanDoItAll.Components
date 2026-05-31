# SB08 semantic invariants

| Invariant | Expected behavior | Disallowed shallow pass | Evidence |
| --- | --- | --- | --- |
| SB08-INV-001 | Experiment packs serialize neutral references to scenario, placement, parameters, rules, run plan, visual mapping, and expected invariants. | Hardcoding shared-well values in runtime code. | `ExperimentInputPack_HashesInputDocumentsButIgnoresUiPlaybackSettings` |
| SB08-INV-002 | Deterministic hashes include input document hashes and exclude UI playback settings. | Treating playback speed as experiment input. | `ExperimentInputPack_HashesInputDocumentsButIgnoresUiPlaybackSettings` |

## Production Behavior Artifact Matrix

See `bundle://proof/SB08/manifest.md`.
