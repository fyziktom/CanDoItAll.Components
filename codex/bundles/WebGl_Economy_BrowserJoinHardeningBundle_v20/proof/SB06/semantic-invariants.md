# SB06 Semantic Invariants

## Invariant ID

SB06-real-scenario-artifacts

## Shallow-pass trap

The runner can export files and call them browser-ready even though no browser runtime has consumed the WebGL document.

## Adversarial negative proof

`EconomyRealProbeArtifactExporterTests` asserts the volatile readiness report omits `readyForLargeScreenBrowserExecution`, sets `browserRuntimeExercised` to false, and exposes only `readyForLargeScreenBrowserSmokeInput` for headless output.

## Semantic positive proof

`bundle://proof/SB06/transcripts/artifact-inventory-assertions.txt` proves shared-well and farmer-land exports each contain 13 files, 3 snapshots, 3 snapshot analyses, a canonical artifact manifest, and one volatile readiness report.

## Anti-stub audit

`bundle://proof/SB06/transcripts/anti-stub-audit.txt` confirms the runner/exporter/readiness code and focused tests contain no placeholder/stub markers.

## Raw-note literal closure

- Keep `EconomyRealScenarioRunner`: kept and extended with run options.
- Optional cleanup before export: implemented through `CleanOutputDirectory`.
- Canonical vs volatile split: `artifact-manifest.json` separates canonical artifacts from `volatile-reports/readiness-report.json`.
- No `DateTimeOffset.UtcNow` in canonical input validation: proved by artifact assertions.
- Headless vs browser wording: report uses `readyForLargeScreenBrowserSmokeInput` and does not claim browser execution.
- Tests avoid repo artifact noise by default: focused tests use temp output roots.
