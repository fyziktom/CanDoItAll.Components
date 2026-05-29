# 07 - Definition of done

## Architecture

- `WebGlLib` owns generic WebGL contracts and runtime.
- `WebGlSandbox` owns generic demos/proofs.
- No economy/process/domain logic added.
- Existing `WebGlWorkbench` remains stable.

## Code

- Scene contracts implemented.
- Asset catalog contracts/services implemented.
- Status symbol contracts/services implemented.
- Interaction contracts implemented.
- Generic `WebGlSceneView` implemented.
- Generic JS runtime implemented.
- Asset inclusion updated safely.
- Standalone sandbox project created.
- Village scene demo created.

## Validation

- `dotnet build CanDoItAll.Components.slnx` passes.
- `npm run webgllib:build-assets` passes.
- `npm run webgllib:verify-assets` passes.
- WebGL sandbox runs.
- Village page renders and is interactable.
- No forbidden dependencies.
- Documentation updated.

## Report

Codex must create:

```text
artifacts/webgl-symbolic-tycoon-sandbox/IMPLEMENTATION_REPORT.md
```

The report must include:

- What was implemented.
- What was intentionally not implemented.
- How assets were discovered.
- Which GLB assets are used.
- What fallback behavior exists.
- Build outputs.
- Browser proof results.
- Remaining risks.
- Follow-up tasks for future economy visualization.
