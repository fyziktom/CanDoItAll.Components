# CanDoItAll WebGL Engine + Economy Follow-up Hardening Bundle v13

Prepared: 2026-06-05

## Purpose

This bundle is a cross-repository follow-up for `CanDoItAll.Components` (`webgl-engine`) and
`CanDoItAll.Economy` (`main`). It is intentionally different from the previous broad hardening
waves: its primary goal is to **stabilize the generic WebGL / WebGlRun foundation** so that future
work can move mostly into Economy and domain drivers.

The bundle should be executed as a controlled release-candidate wave:

1. Finish the remaining shared hardening needed to treat `WebGlLib` and `WebGlRunLib` as a stable
   generic substrate.
2. Freeze generic Components APIs and action vocabulary behind explicit approval tests.
3. Move all remaining economy-specific behavior into domain drivers and Economy-side bridge code.
4. After the Components freeze gate passes, continue primarily in Economy for research-readiness,
   simulation semantics, and economic scenario development.

## Core judgement

The current direction is good, but not ready for a freeze yet. Codex has addressed several major
findings from v12:

- `ResourceTransferVisual` has been generalized to `DirectedFlowVisual`.
- `WebGlRunLib` now owns a generic domain mapping driver contract and manifest/hash validation.
- Economy now provides an economy-specific mapping driver.
- Domain leakage scanning exists in CI.
- `multi-goods-elite` exists as a third canary scenario.
- Readiness reporting is much more evidence-aware.

The remaining work is mostly about **stability gates, API freeze, evidence verification, and
proof quality**.

## Repositories

- `fyziktom/CanDoItAll.Components`, branch `webgl-engine`
- `fyziktom/CanDoItAll.Economy`, branch `main`

## Non-negotiable constraints

- Do not expand generic Components feature scope unless a subbundle explicitly allows it.
- Generic `WebGlLib` must stay domain-neutral and reusable without `WebGlRunLib`.
- Generic `WebGlRunLib` may contain generic driver contracts, but must not contain Economy,
  production-line, market, investor, claim, credit, water/well, or scenario-specific behavior.
- Economy-specific semantics must live in Economy domain packages and drivers.
- Browser/WebGL output is observer proof only. Economic truth comes from headless scenario →
  backend → frames/deltas → metrics/invariants → oracle → manifest → readiness artifacts.
- After SB08, Components should enter a release-candidate freeze: only bug fixes, CI, docs, and
  compatibility shims are allowed unless a new explicit architecture bundle reopens the generic layer.

## Execution model

Execute subbundles in order. Do not skip freeze gates. Every subbundle must produce proof artifacts
with non-empty transcripts and explicit command lines. If a proof cannot be produced, mark the gate
as failed and stop before the next freeze gate.
