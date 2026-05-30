# CanDoItAll WebGL + Economy Shared-Well Readiness Follow-up Bundle v9

This bundle is a cross-repository follow-up for:

- `fyziktom/CanDoItAll.Components`, branch currently checked out by Codex, expected `webgl-engine`
- `fyziktom/CanDoItAll.Economy`, branch currently checked out by Codex, currently `main` in the last observed state

## Non-negotiable rules

1. Do not create a new branch. Work in the branch that is already checked out in each repository.
2. Do not connect `CanDoItAll.Economy` to `CanDoItAll.Components` in this wave.
3. Do not add WebGL, Components, UI, or rendering references to Economy `Simulation.*` projects.
4. Do not add economy, ledger, water, well, account, business, tax, or domain-specific concepts to `CanDoItAll.Components.WebGlLib` or `CanDoItAll.Components.WebGlRunLib`.
5. WebGL is large-screen/desktop only. Do not spend time on small/medium/mobile/tablet optimization, responsive redesign, mobile screenshot gates, or touch-first UX. Use 1440x900+ proof viewports.
6. Keep source-code comments in English.

## Goal of this follow-up

Use the shared-well scenario only as a readiness probe. Do not implement the final demo yet.

The bundle asks Codex to harden generic primitives needed for that demo:

- scenario definitions must compile into events without hardcoded scenario IDs;
- event streams must materialize into frames/deltas through reusable state transitions;
- visual actions must be ordered, grouped, and convertible to generic WebGlRun actions without losing sequence semantics;
- WebGL run/action batching must preserve ordered movement/action stages for the same object;
- performance bottlenecks must be measured and bounded before the first real demo.
