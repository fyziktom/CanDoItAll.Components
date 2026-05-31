# Simulation-to-Visualization Bridge Plan

The bridge is the next major missing piece.

## Input

- `SimulationFrame`
- `SimulationFrameDelta`
- `EconomyVisualFrame`
- `EconomyVisualAction`
- optional `visual.mapping.json`

## Output

- `WebGlSceneDocument` for initial scene
- `WebGlRunDocument`
- `WebGlRunFrame`
- `WebGlSceneCommandBatch`
- `WebGlScenePatch`
- `WebGlObjectMotionCommand`

## Required mapping layers

1. **Node-to-object binding**
   - actor node -> character object
   - resource/location node -> object or building
   - relationship/link -> WebGL link or decoration

2. **Action-to-command mapping**
   - move-to-target -> motion command
   - return-to-anchor -> motion command
   - change-pose -> object patch / asset variant
   - show-symbol -> object symbol patch
   - resource-transfer-visual -> staged symbol/link pulse
   - pulse-link -> link patch or effect command

3. **Stage builder**
   - sequence actions produce ordered stages
   - parallel actions produce one stage or parallel child stages
   - wait actions produce stage delay
   - stage IDs are deterministic

4. **Visual state catalog resolver**
   - pose keys map to asset IDs/variants
   - symbol categories map to symbol asset IDs/effects/colors
   - fallback must be explicit and logged

5. **Traceability**
   - every WebGL command carries `simulationEventId`, `visualActionId`, `frameId`, `experimentId`, and source hash in metadata.
   - WebGL metadata must stay generic. Economy-specific fields live in the bridge output metadata namespace, not WebGlLib internals.
