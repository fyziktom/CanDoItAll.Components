# SB01 Proof Manifest

Subbundle: SB01 - Cross-repo inventory and branch guard

Status: Completed

Owned requirements: R01 Branch And Boundary Guard

Raw notes:

- Work in the currently checked-out branch in both repositories.
- Do not create a new branch.
- Record current branch and latest commit SHA for both repositories.
- Confirm the Components commit message typo does not affect branch selection.
- Verify dependency direction across Components, Economy WebGlBridge, and Economy SimulationSandbox.

Semantic invariant contract: `bundle://proof/SB01/semantic-invariants.md`

## Changed-File Manifest

Hashes for bundle files touched by the repair and SB01 proof are recorded in `bundle://proof/SB01/transcripts/changed-file-hashes.txt`.

## Command Transcripts

| Command | Transcript | Result |
|---|---|---|
| Prepared-stage validator | `bundle://proof/SB00/transcripts/prepared-validator.txt` | Passed |
| Branch, commit, status, and dependency scans | `bundle://proof/SB01/transcripts/branch-inventory-dependencies.txt` | Passed with tracked downstream findings |
| Source assertions | `bundle://proof/SB01/transcripts/source-assertions.txt` | Passed |
| Anti-stub audit | `bundle://proof/SB01/transcripts/anti-stub-audit.txt` | Passed |

## Failing-First And Passing Proof

No production behavior changed in SB01. The adversarial check is the boundary scan in `bundle://proof/SB01/transcripts/branch-inventory-dependencies.txt`, which surfaced BF-SB01-001 and BF-SB01-002 rather than silently treating the baseline as clean.

## Source-Level Assertions

- Components branch: `webgl-engine`; Components HEAD: `cad0c85512b4a12d46678dbbdff1d8f15beefde0`.
- Economy branch: `main`; Economy HEAD: `ee37d966bcb3082c45da9a572549ce5da473c105`.
- Components source scan found no `CanDoItAll.Economy` text in WebGlLib/WebGlRunLib source.
- CodeAnalytics snapshots used: Components `snap-20260601142416-32ec9612`; Economy `snap-20260601142523-389ba89a`.

## Downstream Findings

- BF-SB01-001 maps to SB06/SB07: Economy WebGlBridge source imports `CanDoItAll.Components.WebGlLib` types while the project/package dependency points at WebGlRunLib.
- BF-SB01-002 maps to SB11: SimulationSandbox composes SimpleAccounts in the composition layer and must prove generic contracts stay backend-neutral.

## Browser Or Host Proof

Not applicable. SB01 is a branch and source-boundary guard.
