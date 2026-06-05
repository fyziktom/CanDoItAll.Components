# SB17 Final Red-Team Report

Status: completed

## Executive Decision

The v9 bundle closes without an implementation blocker. The final matrix is conservative: `multi-goods-elite` is headless-valid and browser-observer-valid, but it is not research-ready because the final catalog run used `--no-oracle`. The legacy `shared-well` and `farmer-land` packs remain exploratory/not research-ready because strict scenario validation fails.

No `REOPEN.md` is required.

## Validation Performed

| Area | Result | Evidence |
| --- | --- | --- |
| Economy focused tests | Passed: 14/14 | `bundle://proof/SB17/transcripts/economy-final-focused-tests.txt` |
| Components WebGlRun focused tests | Passed: 20/20 | `bundle://proof/SB17/transcripts/components-final-focused-tests.txt` |
| Components WebGlLib focused tests | Passed: 25/25 | `bundle://proof/SB17/transcripts/webgllib-final-focused-tests.txt` |
| Browser proof verification | Passed: 5/5 | `bundle://proof/SB17/browser-proof-summary.json` |
| Domain leakage scan | Passed: 0 blocking matches | `bundle://proof/SB17/domain-leakage-scan.txt` |
| Headless all-scenario catalog run | Completed: 3 manifests, expected exit 1 because two legacy scenarios fail strict readiness | `bundle://proof/SB17/transcripts/headless-all-three-scenarios.txt` |
| Artifact inventory | Passed: zero empty proof files | `bundle://proof/SB17/artifact-inventory.json` |
| Anti-stub audit | Passed | `bundle://proof/SB17/transcripts/anti-stub-audit.txt` |

## Final Decision Matrix

| Scenario | Exploratory | Headless valid | Oracle valid | Browser observer valid | Research ready | Final status |
| --- | --- | --- | --- | --- | --- | --- |
| `farmer-land` | True | False | False | False | False | `failed` |
| `multi-goods-elite` | True | True | False | True | False | `headless-valid` |
| `shared-well` | True | False | False | False | False | `failed` |

Machine-readable matrix: `bundle://proof/SB17/final-decision-matrix.json`.

## Red-Team Findings

- Simulator noise is reduced, not merely documented: pause/stop ordering, runtime idle semantics, command-batch settlement, observer evidence, oracle corpus checks, manifest diffs, metamorphic tests, and performance comparability now have focused proof.
- Components genericity is preserved for the affected WebGlLib/WebGlRunLib production surface: the SB17 scan found zero blocking source matches. Raw matches are classified as vendor/generated/README noise.
- Headless economic truth remains separate from browser observer evidence. SB12 proves `multi-goods-elite` can render through the generic browser path, but SB17 does not convert browser success into oracle or research readiness.
- No scenario passes because of fallback/default behavior in the final matrix. The two legacy scenarios remain failed under strict checks; `multi-goods-elite` is only `headless-valid`, not `researchReady`.

## Legacy Scenario Failure Classification

- `shared-well`: strict scenario validation fails because strict mode requires an explicit behavior expansion profile.
- `farmer-land`: strict scenario validation fails because strict mode requires an explicit behavior expansion profile; simulation also reports missing actor/resource references and unsupported event handlers for the legacy pack.

## Closure

SB17 closes the bundle. The final state is suitable for exploratory work and strict headless validation of `multi-goods-elite`, with browser observer proof for that generated run. Research-ready claims still require oracle-enabled final catalog proof.
