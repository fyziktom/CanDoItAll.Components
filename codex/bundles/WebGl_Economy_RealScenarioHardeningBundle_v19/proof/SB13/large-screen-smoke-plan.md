# SB13 Large-Screen Browser Smoke Plan

Status: Prepared for execution

## Scope

This is a large-screen-only smoke path for a generated `WebGlRunDocument`. It is not a polished demo, not a mobile/tablet pass, and not a small-screen optimization pass.

Required viewport: `1440x900` or larger.

Primary document candidate:

- `repo://CanDoItAll.Economy/artifacts/economy/real-scenario-runs/shared-well/webgl.run-document.json`

Candidate stageful frame:

- Frame index: `1`
- Expected stage ids:
  - `action.event.near.collect.1.travel.primary`
  - `action.event.near.collect.1.use.sequence`
  - `action.event.near.collect.1.return.primary`

## Browser Proof Decision

Browser proof is intentionally deferred for this bundle. The existing Components sandbox route `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlSandbox/Components/Pages/RunPlayback.razor` renders an in-code generic sample document, but it does not load the generated Economy `webgl.run-document.json` artifact or expose an expected/completed stage-id comparator. Adding that route behavior now would be a new browser harness, which crosses this subbundle's "prepare, but do not overbuild" boundary.

## Smoke Steps

1. Start the existing WebGL sandbox at a large-screen viewport of `1440x900` or larger.
2. Load exactly one generated `WebGlRunDocument` from the candidate artifact path.
3. Render the document's initial scene and verify the canvas is nonblank.
4. Confirm initial scene object count is greater than zero; the current candidate has `13` initial objects.
5. Seek to frame `1`.
6. Play only frame `1` through the existing generic run-document runner.
7. Wait for object-motion barriers declared on the frame stages.
8. Export runtime diagnostics from the browser runtime snapshot.
9. Compare completed stage ids to the expected frame `1` stage ids listed above.
10. Capture a screenshot and diagnostics JSON as proof artifacts.

## Required Diagnostics

The browser proof must capture:

- viewport width and height,
- route or harness URL,
- source run document path,
- run id,
- initial scene id,
- initial object count,
- selected frame index,
- expected stage ids,
- completed stage ids,
- current stage ids,
- active motion count,
- command journal tail or equivalent stage command trace,
- barrier policy and blockers for the active/completed motion stages,
- render diagnostics summary,
- pass/fail result.

## Pass Criteria

- Viewport is at least `1440x900`.
- The loaded document path is the generated artifact path, not an in-code sample.
- Initial scene renders with a nonblank canvas.
- Frame `1` is applied once.
- Object-motion barriers complete without timeout.
- Completed stage ids equal the expected stage ids in order.
- Runtime diagnostics are exported.
- No mobile, tablet, or small-screen CSS/runtime changes are introduced.
- No final UI demo or marketing/demo page is introduced.

## Fail Criteria

- The viewport is below `1440x900`.
- The route loads only the in-code sample document.
- Initial scene is blank or has zero objects.
- Stage ids are missing, reordered unexpectedly, or only partially completed.
- Object-motion barrier waits time out.
- Runtime diagnostics cannot be exported.
- Proof requires adding a polished UI/demo or responsive/mobile behavior.

## Existing Capability References

- `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlRunLib/Playback/WebGlRunDocumentRunner.cs`
- `repo://CanDoItAll.Components/tests/CanDoItAll.Components.WebGlRunLib.Tests/WebGlRunDocumentRunnerTests.cs`
- `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlSandbox/Components/Pages/RunPlayback.razor`
- `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlSandbox/Components/Pages/RunPlayback.razor.cs`
- `repo://CanDoItAll.Economy/artifacts/economy/readiness/real-scenario-readiness-report.json`

## Deferred Implementation Boundary

The later browser harness may add a narrow generated-document loader and diagnostics export surface, but it must stay generic in Components and keep Economy-specific artifact production in `CanDoItAll.Economy`.
