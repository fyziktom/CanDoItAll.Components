# Performance risk register

1. **Bridge projection cost**
   - Risk: action-stage projection creates too many objects/patches.
   - Probe: 500 visual actions, 1000 scene objects.

2. **Snapshot size**
   - Risk: snapshots include full frames and runtime diagnostics for every step.
   - Probe: snapshot size and serialize/deserialize time.

3. **Stage runner**
   - Risk: pending barriers keep render loop active forever.
   - Probe: timeout/idle diagnostics.

4. **Motion queue**
   - Risk: many queued motions per object create long playback time and memory growth.
   - Probe: max queue length and bounded journal.

5. **GLB assets**
   - Risk: high polygon assets slow real browser proof.
   - Probe: asset diagnostics and fallback profile.

6. **Analyzer cost**
   - Risk: snapshot analysis scans large frames repeatedly.
   - Probe: index-based analyzers for stores/relationships/events.
