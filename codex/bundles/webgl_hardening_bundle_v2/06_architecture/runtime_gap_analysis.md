# Runtime Gap Analysis for Future Runs

## Already present

- Scene DTOs.
- Asset catalog and variants.
- Primitive fallback.
- Optional high-detail model alternatives.
- Selection and hover.
- Drag on ground plane.
- Patch/import/export APIs.
- Basic motion interpolation.
- Proof snapshots and diagnostics.
- Render mode concept.

## Still needed before second-phase run work

### Runtime command quality

Future run layers need command results with correlation ids and errors, not only boolean returns.

### Scene document

Future run layers need to save/restore layouts. Generic scene document serialization belongs in `WebGlLib`; save slots and run persistence do not.

### Model lab

Before using external models in a simulation, each model should be diagnosable individually.

### Idle scheduling

Runs may create many static scenes. Idle render loop must sleep.

### Resource ownership

Long-running run/simulation pages will create/update/dispose many objects. Resource leaks must be fixed before adding run playback.

### Patch/motion parity

C# and JS patch/motion behavior must be aligned, otherwise future server-side snapshot generation and client-side playback will diverge.

