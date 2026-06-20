# CanDoItAll WebGL Engine + Economy Follow-up Hardening Bundle v5

Prepared date: 2026-06-03
Stage: completed
Profile: initiative / post-v4 implementation hardening review
Target repositories:

- `fyziktom/CanDoItAll.Components` — branch/ref: `webgl-engine`
- `fyziktom/CanDoItAll.Economy` — branch/ref: `main`

## Purpose

Codex implemented the previous follow-up bundle and pushed both repositories. The architecture moved forward: multi-frame browser apply exists, scenario packs gained manifests, Economy has a scenario selector, and WebGlRun validation is stronger. However, the user found a concrete runtime bug: pressing **Pause** during a performance/playback test does not stop the scene. This bundle focuses on turning playback/cancellation into a robust, testable, generic contract instead of ad-hoc UI state.

## Current review verdict

Completed. The playback lifecycle now treats C# cancellation and browser/JS runtime cancellation as one host contract. Final SB12 proof shows `/run-playback` Pause clears queued runtime work, records runtime stop diagnostics, keeps C# state paused, preserves stable frame/stage/motion counters after the pause deadline, and passes package-mode and performance-budget release-readiness checks.

## Main weaknesses to fix

| Finding | Severity | Summary |
| --- | --- | --- |
| F01 | P0 | RunPlayback pause is not a real runtime stop |
| F02 | P0 | Playback UI event handler can monopolize component command flow |
| F03 | P0 | No public stop-all WebGL runtime operation |
| F04 | P1 | MotionCompleted callback can overwrite paused status |
| F05 | P1 | ApplyPlaybackAsync lacks playback transaction/cancellation summary |
| F06 | P1 | Economy UI deterministic replay is O(n) per step and O(n²) across long playback |
| F07 | P1 | Scenario API is improved but still path-biased |
| F08 | P1 | Scenario manifest locks only part of pack semantics |
| F09 | P1 | Proof hygiene still needs machine enforcement |
| F10 | P2 | Large simulation performance budgets are implicit |
| F11 | P2 | WebGlRun runner state lacks first-class playback lifecycle |
| F12 | P2 | Documentation needs a user-facing playback troubleshooting section |

## Critical instructions for Codex

- Work one subbundle at a time.
- Reproduce the pause bug first. Do not implement blindly.
- Do not mark pause/cancel fixed unless browser proof shows active motions and queued stages stop.
- Do not add Economy or production-line semantics into Components packages.
- Do not close critical subbundles with screenshots only; include JSON assertions and console logs.
- Do not allow zero-byte or empty proof transcripts in completed bundles.
- Preserve WebGlLib as the lightweight rendering layer; optional playback/run concerns stay in WebGlRunLib.

## Execution order

```mermaid
flowchart TD
    SB01[SB01 Baseline and pause reproducer]
    SB02[SB02 WebGlLib runtime stop API]
    SB03[SB03 RunPlayback pause fix]
    SB04[SB04 WebGlRun runner lifecycle]
    SB05[SB05 Multi-frame transaction semantics]
    SB06[SB06 Economy replay performance]
    SB07[SB07 Scenario source cleanup]
    SB08[SB08 Scenario pack hashes]
    SB09[SB09 Performance/resource budgets]
    SB10[SB10 Proof integrity validator]
    SB11[SB11 Docs and troubleshooting]
    SB12[SB12 Final red-team]

    SB01 --> SB02 --> SB03
    SB03 --> SB04 --> SB05
    SB05 --> SB06
    SB01 --> SB07 --> SB08
    SB03 --> SB09
    SB10 --> SB12
    SB06 --> SB12
    SB08 --> SB12
    SB09 --> SB12
    SB11 --> SB12
```

## Prepared-stage validation

Run from the bundle root:

```powershell
python scripts/validate_bundle.py --stage prepared --profile initiative
```

Expected:

```text
Bundle validation passed for stage=prepared, profile=initiative, subbundles=12
```

## Completed-stage validation

Run from the bundle root:

```powershell
python scripts/validate_bundle.py --stage completed --profile initiative
```

Expected:

```text
Bundle validation passed for stage=completed, profile=initiative, subbundles=12
```

## Execution status

Last updated: 2026-06-03

| Subbundle | Status | Gate note |
| --- | --- | --- |
| SB01 | Completed | Failing-first pause proof and proof-hygiene inventory captured. |
| SB02 | Completed | Public runtime stop and stage-cancel API added and proven. |
| SB03 | Completed | RunPlayback Pause now cancels C# playback and stops WebGL runtime work; browser proof passed. |
| SB04 | Completed | WebGlRun runner Pause/Cancel/Stop lifecycle contracts added and proven. |
| SB05 | Completed | Multi-frame ApplyPlayback transaction failure/cancellation reporting added and proven. |
| SB06 | Completed | Economy sandbox replay strategy proven: contiguous forward Step applies one delta frame without reset; manual/seek use full reset replay. |
| SB07 | Completed | Runtime scenario catalog/source flows are pathless; legacy experiment paths remain compatibility-only. |
| SB08 | Completed | Scenario manifests now verify deterministic pack hashes and every required file hash; tampered required companions invalidate catalog descriptors. |
| SB09 | Completed | Generic WebGlRun performance budget harness emits JSON metrics and fails xUnit on threshold regressions. |
| SB10 | Completed | Bundle validator now rejects blank completed transcripts, screenshot-only browser proof, missing critical failing-first evidence, stale package/feed markers, and missing source-assertion scans. |
| SB11 | Completed | Docs now cover playback host integration, Pause troubleshooting, package-mode proof, scenario-pack hashes, and deterministic replay. |
| SB12 | Completed | Final cross-repo red-team closure passed: builds/tests/audits/package proof/browser pause proof/performance budget/proof hygiene all recorded; no open P0/P1 issue remains. |
