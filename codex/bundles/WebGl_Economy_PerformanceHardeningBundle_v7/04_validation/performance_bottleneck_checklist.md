# Performance bottleneck checklist

## Components

- [ ] Link updates use object->link group index.
- [ ] Batch apply is used for frame actions.
- [ ] Sequential actions are not incorrectly coalesced.
- [ ] Asset cache is disposed on scene dispose.
- [ ] Model diagnostics run per loaded template, not every clone.
- [ ] Render scheduler idles after motions and animated symbols complete.
- [ ] Proof snapshot includes batch metrics and asset cache metrics.
- [ ] Large-screen-only proof is documented.

## Economy

- [ ] Scenario definitions are loaded/validated before materialization.
- [ ] Event stream is deterministic and replayable.
- [ ] Frame deltas are minimal.
- [ ] Flow store ids are validated.
- [ ] Flow timestamps match frame or event time.
- [ ] Visual action mapper emits temporal actions.
- [ ] Ledger adapter computes true diffs.
