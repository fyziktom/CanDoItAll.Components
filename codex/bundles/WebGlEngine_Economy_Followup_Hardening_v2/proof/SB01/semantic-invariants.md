# SB01 semantic invariants

Status: Completed.

| Invariant ID | Source raw note | Expected behavior | Disallowed shallow implementation | Failing-first / negative proof | Passing / positive proof | Source files | Downstream dependency |
| --- | --- | --- | --- | --- | --- | --- | --- |
| SB01-INV-001 | `bundle://inputs/raw-user-request.md` | The follow-up bundle executes from fresh current-state proof across both repos, not stale preparation prose. | Claim readiness from the prepared README or saved `validation-result.txt` only. | Source assertions show remaining runtime fixture, command preservation, reset-policy, and dynamic-object risks still exist: `bundle://proof/SB01/transcripts/source-assertions-current-risks.txt`. | Components build/tests/audits and Economy build/tests are captured as command transcripts in `bundle://proof/SB01/manifest.md`. | `repo://CanDoItAll.Components/`, `repo://CanDoItAll.Economy/` | All later subbundles rely on this baseline. |
| SB01-INV-002 | R13 | Proof-hygiene gates must fail on real hygiene defects and pass only after durable correction. | Treat the Economy simulation boundary audit as optional because tests already pass. | `bundle://proof/SB01/transcripts/economy-audit-simulation-boundaries.txt` fails on the oversized strict mapping test file. | `bundle://proof/SB01/transcripts/economy-audit-simulation-boundaries-after-test-split.txt` passes after helper split; `bundle://proof/SB01/transcripts/economy-tests-after-test-split.txt` proves tests still pass. | `repo://CanDoItAll.Economy/tests/CanDoItAll.Economy.Tests/EconomyWebGlBridgeStrictMappingTests.cs`, `repo://CanDoItAll.Economy/tests/CanDoItAll.Economy.Tests/EconomyWebGlBridgeStrictMappingTests.Helpers.cs` | SB02-SB12 can rely on a passing boundary audit baseline. |
| SB01-INV-003 | R14 | Genericity and runtime-risk boundaries are explicitly carried forward. | A source grep that only proves files exist while narrowing the raw request's architectural concerns. | Anti-stub and source assertion scans record the remaining Economy fixture path and targeted WebGlRun/WebGlLib risks: `bundle://proof/SB01/transcripts/economy-anti-stub-fixture-scan.txt`, `bundle://proof/SB01/transcripts/source-assertions-current-risks.txt`. | Components generic boundary audits pass and first-party anti-stub scan is clean: `bundle://proof/SB01/transcripts/components-npm-audit-webgllib-boundary.txt`, `bundle://proof/SB01/transcripts/components-npm-audit-webglrunlib-boundary.txt`, `bundle://proof/SB01/transcripts/components-first-party-anti-stub-scan-v2.txt`. | Source-reference hashes in `bundle://proof/SB01/changed-file-baseline.md` | SB02, SB03, SB04, SB06, SB07, SB08, SB09, SB11. |

## Production assertions

- No production code behavior changed in SB01.
- The only code/test edit is the Economy test helper split described in `bundle://proof/SB01/changed-file-baseline.md`.
- Remaining production risks are preserved as downstream source assertions, not marked solved in SB01.

## Anti-stub audit

See `bundle://proof/SB01/transcripts/components-first-party-anti-stub-scan-v2.txt` and `bundle://proof/SB01/transcripts/economy-anti-stub-fixture-scan.txt`.

## Production Behavior Artifact Matrix

SB01 introduced no new production signal, state, record, or event.

| Artifact / signal | Producer | Consumer | Lifecycle | Negative test |
| --- | --- | --- | --- | --- |
| Boundary-audit test layout | `repo://CanDoItAll.Economy/tests/CanDoItAll.Economy.Tests/EconomyWebGlBridgeStrictMappingTests.cs`, `repo://CanDoItAll.Economy/tests/CanDoItAll.Economy.Tests/EconomyWebGlBridgeStrictMappingTests.Helpers.cs` | `repo://CanDoItAll.Economy/scripts/audit-simulation-boundaries.ps1` | Proof command runs the script before downstream implementation continues. | `bundle://proof/SB01/transcripts/economy-audit-simulation-boundaries.txt` fails before the split; `bundle://proof/SB01/transcripts/economy-audit-simulation-boundaries-after-test-split.txt` passes after. |
