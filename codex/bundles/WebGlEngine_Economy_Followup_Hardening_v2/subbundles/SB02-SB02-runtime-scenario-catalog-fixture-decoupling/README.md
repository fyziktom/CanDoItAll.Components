# SB02 — Runtime scenario catalog and fixture decoupling

## Objective

Remove the Economy browser/Node sandbox runtime dependency on test fixture directories and introduce an app-owned scenario catalog/provider boundary.

## Status

Completed 2026-06-02. Gate passed.

## Covered Inputs

- `bundle://inputs/raw-user-request.md`
- `bundle://analysis/02-critical-findings.md`
- `bundle://requirements/01-normalized-requirements.md`

## Prerequisites

Previous dependency subbundles from `bundle://plan/01-phase-plan.md` must be completed or explicitly reopened. Review their proof manifests before editing.

## Exact Source References

- `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Components/Components/SimulationSandbox/EconomySimulationSandboxPage.razor`
- `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Node/Components/Pages/SimulationSandbox.razor`
- `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.SimulationSandbox/`
- `repo://CanDoItAll.Economy/tests/CanDoItAll.Economy.Tests/Fixtures/ExperimentInputs/`

## Deliverables

- Runtime code must not contain hard-coded `tests/CanDoItAll.Economy.Tests/Fixtures` path search.
- DI registrations for sandbox service and scenario catalog.
- Sample scenario files copied/embedded to an app-owned location or included as package content.
- Tests proving test fixtures remain test-only.

## Dependency Impact

Critical foundation. Downstream subbundles rely on this phase to avoid validating against stale or ambiguous behavior.

## Validation Depth

- Failing-first grep/test showing old runtime searches `tests/`.
- Passing unit test for runtime catalog.
- Passing browser proof on Node route using runtime sample provider.
- Package/deployment-like proof with test fixture directory absent or inaccessible.

## Implementation Steps

- Add a scenario catalog abstraction in the Economy simulation sandbox layer.
- Provide a filesystem/content-root or embedded sample implementation for runtime use.
- Keep a test-fixture implementation only in tests.
- Refactor `EconomySimulationSandboxPage` to use DI/parameters instead of direct service construction and internal `tests/` path search.
- Update Node host registration so `/economy/simulation-sandbox` works outside test context.

## Scope Exceptions

No scope exceptions are allowed unless this README is updated with an explicit exception, proof path, and follow-up owner.

## Do Not Do

- Do not introduce Economy, production-line, ledger, market, account, buyer, seller, price, station, machine, or work-order semantics into Components packages.
- Do not close this subbundle with source-grep proof only.
- Do not depend on global NuGet cache when package-mode behavior is under test.
- Do not treat browser screenshots as proof unless route, action, viewport, console status, and assertions are recorded.
- Do not silently widen or shrink this subbundle scope.

## Acceptance Checklist

- [x] Objective for SB02 is implemented or explicitly blocked with a concrete reason.
- [x] Changed files are listed with hashes in the proof manifest.
- [x] Failing-first proof exists for critical behavior changes.
- [x] Passing proof exercises production code paths, not only fixtures/stubs.
- [x] Boundary and anti-stub scans are recorded.
- [x] Downstream reopen triggers are updated if reality differs from this plan.

## Completion Notes

- Added `IEconomySimulationScenarioCatalog` and `FileSystemEconomySimulationScenarioCatalog`.
- Refactored `EconomySimulationSandboxPage` to receive the sandbox session service and scenario catalog through DI.
- Added Node runtime sample content under `SimulationScenarios/EconomySimulationSandbox` and configured build/publish copy.
- Registered the sandbox session service and runtime catalog in the Node service registration.
- Added tests for fixture decoupling, catalog loading, sandbox service validation, traversal rejection, and component rendering with runtime catalog services.
- Captured Node browser proof for `/economy/simulation-sandbox` at 1600x1000 with scenario load and `Apply frame`, screenshots, assertions, and zero console errors/warnings.

## Proof Required

- Failing-first grep/test showing old runtime searches `tests/`.
- Passing unit test for runtime catalog.
- Passing browser proof on Node route using runtime sample provider.
- Package/deployment-like proof with test fixture directory absent or inaccessible.

Critical subbundles must also create/update `proof/SB02/semantic-invariants.md` with shallow-pass trap, negative proof, positive proof, production assertions, and raw requirement closure.

## Browser Validation Logging

Browser proof required. Record route, viewport, Playwright/browser actions, runtime diagnostics JSON, screenshot paths, console log, assertion list, and result.

## Progression Gate

SB02 may close only after proof manifest, semantic invariants when critical, command/browser transcripts, and source assertions agree. If any downstream prerequisite is affected, update `bundle://plan/01-phase-plan.md` and reopen impacted phases.

## Suggested Agent Prompt

You are a senior C# / Blazor / vanilla JavaScript implementation agent. Execute only this subbundle. Read the exact source references, run the entry gate, implement the smallest correct changes, add failing-first and passing semantic proof, update `proof/SB02/manifest.md`, and stop after the progression gate passes. If current repo observations contradict this README, repair the bundle before editing production code.
