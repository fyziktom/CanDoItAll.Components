# CanDoItAll WebGlEngine + Economy Follow-up Hardening Bundle v10

Prepared: 2026-06-05T10:59:49Z

## Purpose

This bundle is a cross-repository follow-up for `CanDoItAll.Components` (`webgl-engine`) and `CanDoItAll.Economy` (`main`) after Codex completed the previous hardening bundle.

The focus is no longer only making the demos run. The goal is to make economic simulation results trustworthy enough that failures can be attributed to the economic model, not hidden simulator/runtime/projection defects.

## Current verdict

Economic simulations are usable now for exploratory analysis, scenario design, headless pipeline checks, and visualization smoke tests. They are not automatically research-ready. A run should be treated as claim-grade only when it has:

- ResearchStrict policy and zero non-allowlisted model-adjacent warnings.
- Artifact-derived readiness evidence, not caller-asserted flags.
- External golden oracle or explicit `no-oracle` label for weaker headless-valid runs.
- Deterministic headless manifest and frame hash chain.
- Browser observer proof only as observer evidence, not economic truth.
- Domain leakage audit showing generic Components packages are not shaped by Economy examples.

## Key findings

- **F01 [High] Generic WebGlRunLib still contains domain-shaped action kind ResourceTransferVisual** — Replace with generic ValueFlowVisual/LinkFlowVisual/DirectedFlowVisual or move the domain action behind a domain driver in Economy bridge.
- **F02 [Medium] Forbidden domain terms moved to configurable boundary options, but enforcement must be CI-backed** — Add CI/domain-leakage scan that checks WebGlLib/WebGlRunLib source, tests, public constants, samples, and package metadata; domain lists live in driver packages.
- **F03 [Medium] Pause ordering is improved through stop coordinator, but must be proven by first-stop observable UX gate** — Add Playwright proof that captures state immediately after Pause and after drain, asserts no active motions/stages, stable frame, no stale UI rewrite.
- **F04 [High] Observer proof must use actual browser-exported run/scene state, not self-comparison or derived fallback positions** — Require runtime-exported scene/document hash and object positions from browser; no fallback positions in proof; mismatch = observer failure only.
- **F05 [High] Readiness flags must be artifact-derived, not caller asserted** — Introduce EvidenceBundle inputs with hashes/citations; booleans become computed outputs only.
- **F06 [Medium] multi-goods-elite exists but should be hardened as a genericity canary, not only a demo fixture** — Promote it to canary suite with external oracle, metamorphic checks, factor matrix, scenario docs, and generic visual flow driver.
- **F07 [Medium] Economy bridge contains mapping logic but generic extension/driver boundary is not explicit enough** — Define a formal visual/run domain driver pattern: driver-owned action vocabulary, mapping, validation, boundary terms, oracle tests.
- **F08 [Medium] Design matrix factor materialization improved but needs factor binding coverage for third scenario** — Add binding registry and no-effect hard fail for every factor type used by multi-goods-elite.
- **F09 [Medium] Historic proof transcripts include zero-length artifacts; v10 must enforce non-empty meaningful proof** — Proof validator must fail zero-byte transcripts except explicitly allowlisted binary screenshots and intentional placeholder files.

## Subbundles

- **SB01 — Current-state and v9 closure audit**: Audit current push in both repositories, map v9 requirements to implemented files/proofs, and identify incomplete gates.
- **SB02 — Generic Components domain-leakage hardening**: Remove economy-shaped action vocabulary from WebGlRunLib public API or mark as deprecated and move to driver-owned extension.
- **SB03 — Domain driver contract for run/visual mappings**: Introduce a formal domain-driver pattern so necessary domain vocabulary lives outside generic Components packages.
- **SB04 — Economy bridge migration to driver-owned visual flow action**: Move ResourceTransferVisual handling from generic action kinds to Economy driver mapping or generic DirectedValueFlow action.
- **SB05 — Pause and immediate runtime stop proof v2**: Harden pause UX: immediate stop before C# drain, stale callback suppression, first-observable-stop assertion.
- **SB06 — Settled runtime command lifecycle as hard contract**: Ensure applyCommandBatchAndWait and WebGlRunBrowserApplyAdapter distinguish scheduled/settled/cancelled/failed states.
- **SB07 — Browser observer proof must use exported runtime state**: Replace self-comparison/fallback observer proof with actual browser-exported scene/run state hashes and object positions.
- **SB08 — Readiness evidence contract v3**: Make readiness bands artifact-derived: browser/UI/oracle flags computed from evidence bundle, not caller-provided booleans.
- **SB09 — multi-goods-elite scenario canary hardening**: Promote third scenario into canary suite for exchange, multi-good holdings, capital claims, fee shocks, and concentration.
- **SB10 — Exchange/investment semantics driver**: Add generic economic simulation driver semantics for contribution, claim issue, return payment, obligations, and concentration measurement without hardcoding elite-specific behavior.
- **SB11 — Golden oracle corpus expansion**: Externalize oracle cases and add multi-goods-elite oracle expectations including final stores, flows, claims, issues, metrics, frame hash chain.
- **SB12 — Metamorphic and conservation properties**: Add property tests for conservation, monotonic fee effects, investment size effects, concentration bounds, and no hidden negative stores.
- **SB13 — Design matrix factor binding registry v2**: Extend factor materialization to capacity, policy parameter, event enable/disable, fee rate, investment size, return rate, relationship shock.
- **SB14 — Generic visualization canary for non-economy domains**: Add a non-economy sample/run document to Components to prove generic action lifecycle is not economy-shaped.
- **SB15 — Proof integrity and CI gates**: Harden proof validator and CI workflows so empty transcripts, stale screenshots, and skipped scans fail the bundle.
- **SB16 — Performance and comparability budgets for third scenario**: Add headless and observer budgets for multi-goods-elite; classify not-comparable vs economic failure.
- **SB17 — Docs/operator workflow: how to run credible experiments**: Update docs with exact workflow: exploratory, headless-valid, oracle-valid, browser-observer-valid, research-ready.
- **SB18 — Final cross-repo red-team closure**: Independent QA pass over genericity, scenario trust, proof evidence, browser idle, and package boundaries.

## Required repositories

- `fyziktom/CanDoItAll.Components`, branch `webgl-engine`
- `fyziktom/CanDoItAll.Economy`, branch `main`

## Execution rule

Execute subbundles sequentially. After each subbundle, run the specified proof and update the manifest before continuing. If a subbundle introduces architectural drift, stop and refactor before moving forward.
