# SB01 semantic invariants

## SB01-INV-001 Proof hygiene is semantic, not file-existence only

- Source raw note: R12 and W07 require proof artifacts to be non-empty, assertion-backed, and tied to changed-source hashes.
- Expected behavior: completed bundle proof fails when required transcript, log, JSON, Markdown, or browser assertion artifacts are empty.
- Disallowed shallow implementation: a validator that only checks that `manifest.md` files exist.
- Failing-first proof: `bundle://proof/SB01/transcripts/failing-first.txt`.
- Passing proof: `bundle://proof/SB01/transcripts/passing-tests.txt`.
- Changed files and hashes: `bundle://proof/SB01/changed-file-hashes.md`.
- Production assertions: no production code was changed; SB01 adds only bundle-local proof tooling.
- Red-team negative case: v2 completed proof tree with empty artifacts is rejected by name in the failing-first transcript.
- Downstream dependency check: later completed subbundles must run `scripts/audit_proof_integrity.py` before closure.

## SB01-INV-002 Current-state weaknesses are source-backed

- Source raw note: the bundle asks Codex to inspect the previous implementation and harden the next risk layer.
- Expected behavior: source assertions cite concrete Components and Economy paths/lines for each later subbundle weakness.
- Disallowed shallow implementation: carrying the weakness list forward as prose without re-reading the repos.
- Failing-first proof: `bundle://proof/SB01/transcripts/source-assertions.txt` records unresolved source locations before later feature changes.
- Passing proof: `bundle://proof/SB01/current-state-inventory.md` and `bundle://proof/SB01/source-baseline-hashes.md` record cross-repo inventory and hashes.
- Changed files and hashes: `bundle://proof/SB01/changed-file-hashes.md`.
- Production assertions: Components genericity remains unchanged because no production Components source file was modified in SB01.
- Red-team negative case: missing or empty proof artifacts are explicitly classified by `scripts/audit_proof_integrity.py`.
- Downstream dependency check: SB02 starts from the baselined runner/browser-apply source files listed in `bundle://proof/SB01/source-baseline-hashes.md`.

## Production Behavior Artifact Matrix

No production signal, state, record, or event was introduced by SB01.
