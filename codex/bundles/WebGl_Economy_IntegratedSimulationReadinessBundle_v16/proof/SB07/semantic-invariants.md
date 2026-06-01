# SB07 Semantic Invariants

| Invariant ID | Assertion |
|---|---|
| SB07-snapshot-hashes | Snapshot provenance separates stable data hash, visual-state hash, and full snapshot hash. |
| SB07-diff-coverage | Snapshot diffs include relationships, visual state, provenance hashes, metadata, and existing economic frame collections. |

## Shallow-pass trap

A shallow implementation could add hash keys without keeping data hash stable across runtime noise or diff only frame hash changes.

## Adversarial negative proof

`economy-snapshot-builder-store-tests.txt` mutates visual state and relationships and expects targeted diff entries.

## Semantic positive proof

`economy-snapshot-builder-store-tests.txt` passes and proves stable `snapshot.data` behavior when runtime diagnostics change.

## Anti-stub audit

Hashing and diffing are implemented in reusable snapshot infrastructure, not in a probe-only helper.

