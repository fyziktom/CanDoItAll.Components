# Missing Capabilities Before Simulation + Visualization Connection

Before building the actual bridge/demo, we still need:

1. **Economy visual mapping document**
   - Defines how economy visual categories map to neutral visual keys.
   - Does not contain WebGL GLB asset IDs.

2. **Bridge project**
   - `CanDoItAll.Economy.Simulation.WebGlBridge`
   - References `Economy.Visualization` and `Components.WebGlRunLib`.

3. **Scene projection**
   - Economy nodes -> WebGL scene objects.
   - Economy links -> WebGL links.
   - Location coordinates -> WebGL object positions.
   - Stable node/object IDs.

4. **Action projection**
   - Economy visual action -> WebGlRunAction.
   - Sequence -> staged batch.
   - Move -> motion command.
   - Pose/symbol -> patch command.
   - Transfer/pulse -> visual effect/link action.

5. **Traceability**
   - source event id
   - visual action id
   - frame id
   - scenario hash
   - input pack hash

6. **Strict deterministic run loader**
   - Loads pack.
   - Verifies hashes.
   - Builds frames.
   - Builds visual frames.
   - Optionally passes to bridge.

7. **Proof**
   - shared-well readiness
   - farmer-land readiness
   - no domain leakage in generic layers
   - no WebGL mobile optimization drift
