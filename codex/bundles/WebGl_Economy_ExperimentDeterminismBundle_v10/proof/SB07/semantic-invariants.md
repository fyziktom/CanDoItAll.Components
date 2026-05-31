# SB07 semantic invariants

| Invariant | Expected behavior | Disallowed shallow pass | Evidence |
| --- | --- | --- | --- |
| SB07-INV-001 | WebGL validation remains large-screen-only. | Adding small/medium/mobile/tablet optimization work. | `npm run webgllib:audit-scene-runtime` passed with no forbidden optimization drift. |

## Production Behavior Artifact Matrix

No production signal, state, record, or event is introduced by SB07.
