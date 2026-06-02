# SB01 — Current-state and proof hygiene audit

## Objective

Establish a fresh cross-repo baseline after Codex execution, inventory changed surfaces, and audit previous proof quality before editing code.

## Status

Completed 2026-06-02. Closure gate passed.

## Covered Inputs

- `bundle://inputs/raw-user-request.md`
- `bundle://analysis/02-critical-findings.md`
- `bundle://requirements/01-normalized-requirements.md`

## Prerequisites

None. This is the first subbundle.

## Exact Source References

- `repo://CanDoItAll.Components/codex/bundles/WebGlEngine_Economy_Hardening_Bundle_v1/`
- `repo://CanDoItAll.Economy/codex/bundles/component-nuget-migration/`
- `repo://CanDoItAll.Components/CanDoItAll.Components.slnx`
- `repo://CanDoItAll.Economy/CanDoItAll.Economy.slnx`
- `repo://CanDoItAll.Components/package.json`
- `repo://CanDoItAll.Economy/scripts/audit-simulation-boundaries.ps1`

## Deliverables

- No code changes unless needed to add a reusable proof-audit script.
- Output `proof/SB01/current-state-inventory.md`.
- Output `proof/SB01/proof-hygiene-audit.md`.
- Output `proof/SB01/changed-file-baseline.md` with SHA-256 hashes for key files.
- Mark noisy generated proof assets that should be pruned or moved in later cleanup.

## Dependency Impact

Critical foundation. Downstream subbundles rely on this phase to avoid validating against stale or ambiguous behavior.

## Validation Depth

- `dotnet build` and focused tests for Components and Economy as baseline.
- `npm run webgllib:audit-scene-runtime-imports`, `npm run webgllib:audit-boundary`, `npm run webglrunlib:audit-boundary` if available.
- Proof manifests must cite actual non-empty transcript files or explicitly mark expected-empty outputs.

## Implementation Steps

- Compare current Components `webgl-engine` and Economy `main` against the last known pre-execution commits.
- Inventory WebGlLib/WebGlRunLib/Economy simulation files, tests, docs, package metadata, and browser proof routes.
- Audit previous proof manifests for missing, empty, stale, or low-semantic transcripts.
- Produce a concise refactor decision log before downstream implementation.

## Scope Exceptions

No scope exceptions are allowed unless this README is updated with an explicit exception, proof path, and follow-up owner.

## Do Not Do

- Do not introduce Economy, production-line, ledger, market, account, buyer, seller, price, station, machine, or work-order semantics into Components packages.
- Do not close this subbundle with source-grep proof only.
- Do not depend on global NuGet cache when package-mode behavior is under test.
- Do not treat browser screenshots as proof unless route, action, viewport, console status, and assertions are recorded.
- Do not silently widen or shrink this subbundle scope.

## Acceptance Checklist

- [x] Objective for SB01 is implemented or explicitly blocked with a concrete reason.
- [x] Changed files are listed with hashes in the proof manifest.
- [x] Failing-first proof exists for the discovered proof-hygiene gate failure.
- [x] Passing proof exercises compiled tests and repo audit scripts, not only fixtures/stubs.
- [x] Boundary and anti-stub scans are recorded.
- [x] Downstream reopen triggers are updated if reality differs from this plan.

## Proof Required

- `dotnet build` and focused tests for Components and Economy as baseline.
- `npm run webgllib:audit-scene-runtime-imports`, `npm run webgllib:audit-boundary`, `npm run webglrunlib:audit-boundary` if available.
- Proof manifests must cite actual non-empty transcript files or explicitly mark expected-empty outputs.

Critical subbundles must also create/update `proof/SB01/semantic-invariants.md` with shallow-pass trap, negative proof, positive proof, production assertions, and raw requirement closure.

## Browser Validation Logging

N/A unless this subbundle changes browser-visible runtime or UI. If browser-visible behavior is touched, record route, viewport, actions, assertions, screenshot paths, console log, and result.

## Progression Gate

SB01 may close only after proof manifest, semantic invariants when critical, command/browser transcripts, and source assertions agree. If any downstream prerequisite is affected, update `bundle://plan/01-phase-plan.md` and reopen impacted phases.

Gate result: Pass. SB02 remains the next dependency because SB01 source assertions confirm the runtime fixture dependency still exists.

## Suggested Agent Prompt

You are a senior C# / Blazor / vanilla JavaScript implementation agent. Execute only this subbundle. Read the exact source references, run the entry gate, implement the smallest correct changes, add failing-first and passing semantic proof, update `proof/SB01/manifest.md`, and stop after the progression gate passes. If current repo observations contradict this README, repair the bundle before editing production code.
