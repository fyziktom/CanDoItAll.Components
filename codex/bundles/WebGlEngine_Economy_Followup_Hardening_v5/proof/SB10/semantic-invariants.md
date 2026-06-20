# Semantic invariants SB10

Status: completed

## Invariants

- Invariant 1: Completed subbundles must have non-empty proof transcripts; zero-byte or whitespace-only transcript files fail validation.
- Invariant 2: Browser proof screenshots must be paired with valid JSON assertion artifacts that contain an `assertions` object.
- Invariant 3: Completed P0/P1 subbundles must cite failing-first evidence locally or through their completed proof manifest/semantic record.
- Invariant 4: Completed proof must not carry stale package/feed failure markers such as stale feed text, `NU1301`, or service-index load failures.
- Invariant 5: Completed proof must include a source-assertion transcript so changed contracts are proven by artifacts, not prose.
- Invariant 6: Prepared-stage validation must still allow future subbundles to remain incomplete until they are marked completed.

## Proof Mapping

| Invariant | Evidence |
| --- | --- |
| Invariant 1 | `bundle://proof/SB10/transcripts/proof-validator-unit-tests.txt` rejects a blank completed transcript. |
| Invariant 2 | `bundle://proof/SB10/transcripts/proof-validator-unit-tests.txt` rejects browser screenshots without assertion JSON; `bundle://proof/SB10/transcripts/bundle-validator-after-sb10.txt` proves existing browser assertion JSON parses successfully, including BOM-prefixed files. |
| Invariant 3 | `bundle://proof/SB10/transcripts/proof-validator-unit-tests.txt` rejects a critical completed subbundle without failing-first proof; SB02/SB03 manifests now explicitly cite SB01 failing-first artifacts. |
| Invariant 4 | `bundle://proof/SB10/transcripts/proof-validator-unit-tests.txt` rejects stale package/feed markers. |
| Invariant 5 | `bundle://proof/SB10/transcripts/source-assertion-proof-validator-scan.txt` proves the validator checks for source-assertion transcripts in completed proof. |
| Invariant 6 | `bundle://proof/SB10/transcripts/proof-validator-unit-tests.txt` proves prepared-stage validation passes when future subbundles are still placeholders. |

## Production Behavior Artifact Matrix

No production behavior artifacts were added by SB10. The validator changes affect bundle proof closure only.
