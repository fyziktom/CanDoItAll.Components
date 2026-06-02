# SB01 current-state inventory

Status: Completed 2026-06-02.

## Repository baseline

| Repo | Branch | HEAD | Status |
| --- | --- | --- | --- |
| `repo://CanDoItAll.Components` | `webgl-engine` | `d127dcb2b7203d17e9ad552cbf3f80ac5c2bfdd6` | Baseline build, focused tests, and WebGL audits pass. |
| `repo://CanDoItAll.Economy` | `main` | `38ce002c3fb1afc724eb9b5f35595e8308903f80` | Baseline build/tests pass; boundary audit required a mechanical test split before passing. |

## Command baseline

| Area | Result | Transcript |
| --- | --- | --- |
| Components solution build | Pass, 0 warnings/errors | `bundle://proof/SB01/transcripts/components-dotnet-build.txt` |
| Components WebGlLib tests | Pass, 44 tests | `bundle://proof/SB01/transcripts/components-webgllib-tests.txt` |
| Components WebGlRunLib tests | Pass, 32 tests | `bundle://proof/SB01/transcripts/components-webglrunlib-tests.txt` |
| Components scene runtime import audit | Pass, 36 modules | `bundle://proof/SB01/transcripts/components-npm-audit-scene-runtime-imports.txt` |
| Components WebGlLib boundary audit | Pass | `bundle://proof/SB01/transcripts/components-npm-audit-webgllib-boundary.txt` |
| Components WebGlRunLib boundary audit | Pass | `bundle://proof/SB01/transcripts/components-npm-audit-webglrunlib-boundary.txt` |
| Economy solution build | Pass with existing warnings | `bundle://proof/SB01/transcripts/economy-dotnet-build.txt` |
| Economy tests before SB01 split | Pass, 546 tests | `bundle://proof/SB01/transcripts/economy-tests.txt` |
| Economy simulation boundary audit before split | Fail, one oversized test file | `bundle://proof/SB01/transcripts/economy-audit-simulation-boundaries.txt` |
| Economy simulation boundary audit after split | Pass | `bundle://proof/SB01/transcripts/economy-audit-simulation-boundaries-after-test-split.txt` |
| Economy tests after split | Pass, 546 tests | `bundle://proof/SB01/transcripts/economy-tests-after-test-split.txt` |

## Current source assertions

| Finding | Assertion | Evidence |
| --- | --- | --- |
| Runtime fixture dependency remains | The Economy sandbox page still constructs `EconomySimulationSandboxSessionService` directly and resolves `Fixtures/ExperimentInputs` paths at runtime. SB02 owns removal. | `bundle://proof/SB01/transcripts/source-assertions-current-risks.txt` |
| Mixed direct+staged frame command risk remains | `WebGlRunFrameApplyResult` still emits direct frame patches/motions only when `frame.Stages.Count == 0`. SB03 owns semantic hardening. | `bundle://proof/SB01/transcripts/source-assertions-current-risks.txt` |
| Runtime options reset risk remains | The browser apply adapter reset surface imports the scene document on reset; no source assertion showed runtime option reapplication in that path. SB04 owns policy and tests. | `bundle://proof/SB01/transcripts/source-assertions-current-risks.txt` |
| Generic/domain boundary is active | Components WebGlLib/WebGlRunLib boundary audits pass and scoped first-party anti-stub scan is clean. | `bundle://proof/SB01/transcripts/components-npm-audit-webgllib-boundary.txt`, `bundle://proof/SB01/transcripts/components-npm-audit-webglrunlib-boundary.txt`, `bundle://proof/SB01/transcripts/components-first-party-anti-stub-scan-v2.txt` |
| Economy bridge dynamic object policy risk remains | Economy bridge validator checks command and patch object references against initial scene object ids. SB07 owns static-only vs dynamic policy. | `bundle://proof/SB01/transcripts/source-assertions-current-risks.txt` |

## SB01 implementation note

The only code/test edit in SB01 is a mechanical split of `repo://CanDoItAll.Economy/tests/CanDoItAll.Economy.Tests/EconomyWebGlBridgeStrictMappingTests.cs` into a partial helper file. No assertions or production behavior were changed. This was required because the existing simulation boundary audit rejected the 530-line test file before any downstream implementation could claim a clean baseline.

