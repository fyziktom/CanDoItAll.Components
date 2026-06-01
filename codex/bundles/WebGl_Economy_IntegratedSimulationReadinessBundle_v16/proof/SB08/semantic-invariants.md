# SB08 Semantic Invariants

| Invariant ID | Assertion |
|---|---|
| SB08-descriptor-index | File snapshot storage writes a descriptor index separate from full snapshot payloads. |
| SB08-integrity | Full payload loads validate deterministic hash and reject tampering. |

## Shallow-pass trap

A shallow pass could list by deserializing every full snapshot or skip tamper validation. The file-store test reopens the store from the descriptor index and then tampers with a payload.

## Adversarial negative proof

`economy-file-snapshot-store-tests.txt` expects tampered payload load to throw.

## Semantic positive proof

`economy-file-snapshot-store-tests.txt` passes for save, indexed list, filtered list, reopen, load, delete, and tamper detection.

## Anti-stub audit

The store writes real JSON payloads and a real `descriptors.index.json`, not an in-memory-only fake.

