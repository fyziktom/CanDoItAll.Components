# Execution Plan

## Phase A — Baseline and contract freeze preparation

- SB01: Current-state and proof integrity audit
- SB02: Package scope and packable boundary
- SB03: Public C# API freeze baseline
- SB04: JavaScript runtime API freeze baseline

## Phase B — Runtime and component boundary stabilization

- SB05: WebGlSceneView facade refactor
- SB06: Runtime idle policy modes
- SB07: Command batch and stage lifecycle contract
- SB08: Domain driver contract freeze

**Mandatory senior QA/refactor gate after SB08:**

- Re-read all public API diffs.
- Check that no domain assumptions were added.
- Check that WebGlLib still does not reference WebGlRunLib.
- Check that WebGlRunLib still does not reference Economy or future domain packages.
- Check that every proof transcript is non-empty or explicitly justified.

## Phase C — Boundary, package and stress proofs

- SB09: Source provenance opacity
- SB10: Domain boundary audit v3
- SB11: Package-mode samples and static assets
- SB12: Resource ownership and asset-cache stress
- SB13: Large scene and compact lifecycle proof
- SB14: Browser observer generic proof

## Phase D — Freeze and handoff

- SB15: Docs and consumer migration guide
- SB16: Final Components release-candidate freeze

After SB16, future work should move primarily to Economy and other domain drivers. Components may receive bugfixes and explicitly approved generic API changes only.
