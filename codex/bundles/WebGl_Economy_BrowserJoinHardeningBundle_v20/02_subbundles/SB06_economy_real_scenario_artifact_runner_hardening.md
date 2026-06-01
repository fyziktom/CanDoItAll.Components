# SB06 - Economy real scenario artifact runner hardening

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
