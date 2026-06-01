# SB06 - Economy real scenario artifact runner hardening

## Status

Completed. Closure gate passed.

## Goal

Make real headless runs safe, deterministic where needed, and useful for analysis.

## Tasks

- Keep `EconomyRealScenarioRunner`.
- Add CLI/tool entry if not already practical.
- Separate canonical artifacts from volatile reports.
- Do not put `DateTimeOffset.UtcNow` in canonical artifacts.
- Make `readyForLargeScreenBrowserExecution` wording precise:
  - use `readyForLargeScreenBrowserSmokeInput` until browser runtime is actually exercised.
- Add optional output cleanup before export.
- Ensure test artifact outputs do not create untracked noise unless explicitly enabled.

## Acceptance

- Real scenario runner exports all expected files for shared-resource and finite-resource probes.
- Readiness report clearly states headless vs browser-smoke readiness.

## Prerequisites

- SB05 page state understood so readiness wording can distinguish headless input from browser runtime proof.

## Owned Requirements

- R06 Real scenario artifacts.

## Dependency Impact

This proof feeds SB07 strict inputs, SB11 smoke artifacts, SB12 performance, and SB14 readiness closure.

## Validation Depth

Exporter tests must inspect generated artifact content, canonical/volatile split, readiness wording, and cleanup behavior. CLI/tool entry is required only if current runner is impractical.

## Proof Required

- Real probe exporter test transcript.
- Generated artifact inventory/assertions.
- `bundle://proof/SB06/manifest.md`
- `bundle://proof/SB06/semantic-invariants.md`

## Browser Validation Logging

N/A for headless artifacts. Browser readiness must explicitly say browser runtime proof is not complete until SB11.

## Semantic Adequacy Gate

- Shallow-pass trap: readiness report says browser-ready because files exist headlessly.
- Adversarial negative proof: canonical artifacts reject volatile timestamp pollution or overclaiming `readyForLargeScreenBrowserExecution`.
- Semantic positive proof: shared-resource and finite-resource exports contain expected stable files and precise readiness mode.
- Anti-stub audit: runner/exporter does real file export and does not create untracked noise by default.

## Progression Gate

Pass only when readiness language cannot be confused with actual browser execution.
