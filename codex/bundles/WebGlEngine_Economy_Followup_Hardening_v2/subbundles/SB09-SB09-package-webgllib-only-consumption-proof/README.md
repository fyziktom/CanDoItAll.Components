# SB09 — Package and WebGlLib-only consumption proof

## Objective

Prove light WebGlLib-only consumption and WebGlRunLib package consumption through isolated fresh packages, without stale cache or accidental heavy dependencies.

## Status

Prepared. Not implemented.

## Covered Inputs

- `bundle://inputs/raw-user-request.md`
- `bundle://analysis/02-critical-findings.md`
- `bundle://requirements/01-normalized-requirements.md`

## Prerequisites

Previous dependency subbundles from `bundle://plan/01-phase-plan.md` must be completed or explicitly reopened. Review their proof manifests before editing.

## Exact Source References

- `repo://CanDoItAll.Components/Directory.Build.props`
- `repo://CanDoItAll.Components/samples/CanDoItAll.Components.WebGlLibOnlyViewer`
- `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Simulation.WebGlBridge/CanDoItAll.Economy.Simulation.WebGlBridge.csproj`
- `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Components/CanDoItAll.Economy.Components.csproj`
- `repo://CanDoItAll.Economy/NuGet.config`

## Deliverables

- WebGlLib-only sample builds with only WebGlLib package.
- Economy bridge package-mode build passes with fresh package feed and isolated package cache.
- Package content audit does not include codex proof artifacts or unexpected huge files.
- Versioning/stale-cache strategy is documented.

## Dependency Impact

Critical foundation. Downstream subbundles rely on this phase to avoid validating against stale or ambiguous behavior.

## Validation Depth

- Transcript of `dotnet pack`.
- Transcript of isolated restore/build.
- nupkg content listing.
- Dependency graph scan.

## Implementation Steps

- Pack Components in Release to an isolated artifacts feed.
- Restore consumers with isolated `NUGET_PACKAGES` and a local NuGet.config that prioritizes fresh package output.
- Build a WebGlLib-only sample and prove it does not reference WebGlRunLib.
- Build Economy WebGlBridge and Economy Components in package mode.
- Audit nupkg contents and approximate size, especially GLB/static assets and proof artifacts.

## Scope Exceptions

No scope exceptions are allowed unless this README is updated with an explicit exception, proof path, and follow-up owner.

## Do Not Do

- Do not introduce Economy, production-line, ledger, market, account, buyer, seller, price, station, machine, or work-order semantics into Components packages.
- Do not close this subbundle with source-grep proof only.
- Do not depend on global NuGet cache when package-mode behavior is under test.
- Do not treat browser screenshots as proof unless route, action, viewport, console status, and assertions are recorded.
- Do not silently widen or shrink this subbundle scope.

## Acceptance Checklist

- [ ] Objective for SB09 is implemented or explicitly blocked with a concrete reason.
- [ ] Changed files are listed with hashes in the proof manifest.
- [ ] Failing-first proof exists for critical behavior changes.
- [ ] Passing proof exercises production code paths, not only fixtures/stubs.
- [ ] Boundary and anti-stub scans are recorded.
- [ ] Downstream reopen triggers are updated if reality differs from this plan.

## Proof Required

- Transcript of `dotnet pack`.
- Transcript of isolated restore/build.
- nupkg content listing.
- Dependency graph scan.

Critical subbundles must also create/update `proof/SB09/semantic-invariants.md` with shallow-pass trap, negative proof, positive proof, production assertions, and raw requirement closure.

## Browser Validation Logging

Browser proof required. Record route, viewport, Playwright/browser actions, runtime diagnostics JSON, screenshot paths, console log, assertion list, and result.

## Progression Gate

SB09 may close only after proof manifest, semantic invariants when critical, command/browser transcripts, and source assertions agree. If any downstream prerequisite is affected, update `bundle://plan/01-phase-plan.md` and reopen impacted phases.

## Suggested Agent Prompt

You are a senior C# / Blazor / vanilla JavaScript implementation agent. Execute only this subbundle. Read the exact source references, run the entry gate, implement the smallest correct changes, add failing-first and passing semantic proof, update `proof/SB09/manifest.md`, and stop after the progression gate passes. If current repo observations contradict this README, repair the bundle before editing production code.
