# SB01 proof manifest

Status: Completed.

Owned requirements: R12 proof artifacts must be non-empty, assertion-backed, and tied to changed-source hashes.

Raw notes: `bundle://inputs/raw-user-request.md`, `bundle://analysis/01-current-state-after-v3.md`, `bundle://analysis/02-main-weaknesses-and-repair-strategy.md`.

Semantic invariant contract: `bundle://proof/SB01/semantic-invariants.md`.

## Changed file hashes

| File | Before SHA-256 | After SHA-256 | Evidence |
| --- | --- | --- | --- |
| `bundle://scripts/audit_proof_integrity.py` | `absent` | `0fc876bb465a528abab5c52dd718bf6fb1d4c3625da78959b97248148bcdc004` | `bundle://proof/SB01/changed-file-hashes.md` |

## Source baseline hashes

Likely downstream source files are baselined in `bundle://proof/SB01/source-baseline-hashes.md`.

## Command transcripts

| Command / action | Result | Transcript |
| --- | --- | --- |
| Failing-first proof hygiene audit against v2 completed proof tree | Fails with empty transcript/log/browser-proof artifacts | `bundle://proof/SB01/transcripts/failing-first.txt` |
| Passing proof hygiene audit against v4 prepared proof tree | Passes because no subbundle is marked completed with empty required proof | `bundle://proof/SB01/transcripts/passing-tests.txt` |
| Source assertion scan with CodeAnalytics snapshot ids and `rg` line references | Passes and records the exact current weaknesses in Components and Economy | `bundle://proof/SB01/transcripts/source-assertions.txt` |

## Inventory artifacts

| Artifact | Result |
| --- | --- |
| `bundle://proof/SB01/current-state-inventory.md` | Non-empty cross-repo commit/package/proof-artifact inventory |
| `bundle://proof/SB01/source-baseline-hashes.md` | Non-empty baseline hash matrix for likely downstream changed files |

## Source assertions

| Assertion | Evidence |
| --- | --- |
| Empty proof artifacts exist in previous completed proof trees and must be rejected. | `bundle://proof/SB01/transcripts/failing-first.txt` |
| The new v4 bundle can pass completed-only proof hygiene before completed subbundles exist. | `bundle://proof/SB01/transcripts/passing-tests.txt` |
| Current code still contains the weaknesses assigned to SB02-SB11, including single-frame playback apply, input-order validation, path-centric session fields, and scene key lifecycle risk. | `bundle://proof/SB01/transcripts/source-assertions.txt` |

## Anti-stub audit

SB01 added only `scripts/audit_proof_integrity.py`; the passing transcript proves the script performs real manifest/proof-tree traversal and reports concrete empty artifacts instead of returning a fixed success string.

## Browser artifacts

None. SB01 does not claim browser behavior.

## Production Behavior Artifact Matrix

No production runtime signal, state, record, or event was introduced by SB01.
