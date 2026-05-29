# SB04 Components Render Scheduler And Disposal Hardening

## Status

- Status: Completed

## Objective

- Make WebGL render scheduling idle-aware and improve resource disposal auditability.

## Covered Inputs

- `bundle://02_subbundles/SB04_components_render_scheduler_and_disposal.md`
- WebGL screenshot validation request.

## Prerequisites

- SB03 scene document behavior is stable.

## Exact Source References

- `bundle://02_subbundles/SB04_components_render_scheduler_and_disposal.md`
- `repo://src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/15-webgl-scene-render-loop.js`
- `repo://src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/17-webgl-scene-resources.js`

## Deliverables

- Idle-aware scheduler diagnostics and resource audit helpers.

## Dependency Impact

- Unlocks reliable browser proof for static and model-heavy scenes.

## Validation Depth

- JS audit and browser proof JSON/screenshot must show static scenes reach idle.

## Implementation Steps

- Track loop activity, idle timestamp, render reason, motion count, animated symbol count, and disposal diagnostics.

## Do Not Do

- Do not force continuous rendering for static scenes in auto/on-demand modes.

## Acceptance Checklist

- Continuous, auto, and on-demand scheduling are distinct.
- Static scene browser evidence reaches idle.

## Proof Required

- Audit transcript, browser screenshot, browser proof JSON, and execution-report analytics row.

## Browser Validation Logging

- Capture route, viewport, screenshot path, scheduler diagnostics, and pass/fail result.

## Progression Gate

- Proceed to SB05 only after browser proof is nonblank and idle state is verified.

## Suggested Agent Prompt

- Implement idle-aware WebGL scheduling and prove it with a real browser screenshot.

