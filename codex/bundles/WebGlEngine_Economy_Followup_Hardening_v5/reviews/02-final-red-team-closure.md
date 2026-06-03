# Final red-team closure matrix

Date: 2026-06-03
Status: Completed

## Verdict

No P0/P1 issue remains open. The original Pause failure has failing-first proof from SB01 and final passing proof from SB12. Performance, package-mode, proof-hygiene, and documentation closure are artifact-backed.

## Closure matrix

| Finding | Severity | Verdict | Primary evidence | Residual risk |
| --- | --- | --- | --- | --- |
| F01 RunPlayback pause is not a real runtime stop | P0 | Solved | `bundle://proof/SB01/browser/failing-first-pause-assertions.json`; `bundle://proof/SB03/browser/runplayback-pause-assertions.json`; `bundle://proof/SB12/browser/runplayback-pause-final-assertions.json` | None for `/run-playback`; host integrations must follow documented runtime-stop recipe. |
| F02 Playback UI event handler can monopolize command flow | P0 | Solved | `bundle://proof/SB03/transcripts/source-assertion-runplayback-pause-scan.txt`; `bundle://proof/SB12/browser/runplayback-pause-final-assertions.json` | None observed in final browser proof; future hosts should not await long-running playback loops in UI handlers. |
| F03 No public stop-all WebGL runtime operation | P0 | Solved | `bundle://proof/SB02/browser/runtime-stop-assertions.json`; `bundle://proof/SB12/transcripts/source-assertion-final-contract-scan.txt` | None. Public C# and JS runtime stop paths are present and covered. |
| F04 MotionCompleted can overwrite paused status | P1 | Solved | `bundle://proof/SB03/browser/runplayback-pause-assertions.json`; `bundle://proof/SB12/browser/runplayback-pause-final-assertions.json` | None observed; final proof shows status remains `Paused.` after the pause deadline. |
| F05 ApplyPlaybackAsync lacks transaction/cancellation summary | P1 | Solved | `bundle://proof/SB05/transcripts/webglrunlib-focused-tests.txt`; `bundle://proof/SB12/transcripts/components-webglrunlib-final-tests.txt` | None. Transaction and cancellation fields are part of the WebGlRun contract. |
| F06 Economy deterministic replay is too expensive for forward Step | P1 | Solved | `bundle://proof/SB06/browser/economy-replay-mode-assertions.json`; `bundle://proof/SB12/transcripts/economy-focused-final-tests.txt` | Solved for the Economy simulation sandbox path; other hosts should adopt the same replay-mode split. |
| F07 Scenario API remains path-biased | P1 | Solved | `bundle://proof/SB07/browser/simulation-sandbox-pathless-catalog-assertions.json`; `bundle://proof/SB12/transcripts/economy-focused-final-tests.txt` | Legacy path APIs remain compatibility-only and are documented as such. |
| F08 Scenario manifest locks only part of pack semantics | P1 | Solved | `bundle://proof/SB08/transcripts/failing-first-required-companion-tamper-test.txt`; `bundle://proof/SB12/transcripts/economy-focused-final-tests.txt` | None for manifest-required companion files; future manifest fields need the same hash discipline. |
| F09 Proof hygiene needs machine enforcement | P1 | Solved | `bundle://proof/SB10/transcripts/proof-validator-unit-tests.txt`; `bundle://proof/SB12/transcripts/proof-hygiene-inventory.txt`; `bundle://proof/SB12/transcripts/bundle-validator-completed-final.txt` | None. The final validator checks completed proof shape and browser assertion JSON. |
| F10 Large simulation performance budgets are implicit | P2 | Solved | `bundle://proof/SB09/metrics/webglrun-performance-budget-metrics.json`; `bundle://proof/SB12/metrics/webglrun-performance-budget-final-metrics.json` | Thresholds are test-enforced; they may need tuning when hardware or scenario scale changes. |
| F11 WebGlRun runner state lacks lifecycle | P2 | Solved | `bundle://proof/SB04/transcripts/webglrunlib-focused-tests.txt`; `bundle://proof/SB12/transcripts/components-webglrunlib-final-tests.txt` | None. Pause/Cancel/Stop lifecycle state is first-class. |
| F12 Documentation needs playback troubleshooting | P2 | Solved | `bundle://proof/SB11/transcripts/source-assertion-docs-coverage-scan.txt`; `repo://components/docs/webgl/playback-hosting-and-troubleshooting.md`; `repo://economy/docs/SCENARIOS_AND_SIMULATION.md` | None. Docs now separate generic host lifecycle from Economy replay ownership. |

## Final release-readiness checks

| Check | Result | Evidence |
| --- | --- | --- |
| Components build | Passed | `bundle://proof/SB12/transcripts/components-final-build-rerun.txt` |
| Economy build | Passed with existing dependency warnings | `bundle://proof/SB12/transcripts/economy-final-build.txt` |
| Components tests | Passed | `bundle://proof/SB12/transcripts/components-webgllib-final-tests.txt`; `bundle://proof/SB12/transcripts/components-webglrunlib-final-tests.txt` |
| Economy focused tests | Passed | `bundle://proof/SB12/transcripts/economy-focused-final-tests.txt` |
| Browser Pause proof | Passed | `bundle://proof/SB12/browser/runplayback-pause-final-assertions.json`; `bundle://proof/SB12/browser/runplayback-pause-final-after.png` |
| Performance budget | Passed | `bundle://proof/SB12/metrics/webglrun-performance-budget-final-metrics.json` |
| Package and package-mode proof | Passed | `bundle://proof/SB12/transcripts/components-final-pack.txt`; `bundle://proof/SB12/transcripts/economy-webglbridge-package-mode-proof.txt` |
| Proof hygiene | Passed | `bundle://proof/SB12/transcripts/proof-hygiene-inventory.txt`; `bundle://proof/SB12/transcripts/bundle-validator-completed-final.txt` |

## Residual warnings

- Economy final build still reports existing dependency warnings (`NU1701`, `NU1510`, `NU1902`) unrelated to the WebGL runtime hardening acceptance gate.
- Economy WebGlBridge package-mode build still reports two existing nullable warnings in `SimulationEventNormalizer.cs`; package consumption and build completion passed.
- The WebGlLib runtime audit reports existing line-count warnings for JS runtime modules; the audit itself passed after SB12 parser hardening.
