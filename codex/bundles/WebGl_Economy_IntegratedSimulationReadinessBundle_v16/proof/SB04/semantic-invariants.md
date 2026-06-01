# SB04 Semantic Invariants

| Invariant ID | Assertion |
|---|---|
| SB04-journal-observability | Delayed command stages produce bounded, inspectable journal entries and counters in diagnostics/proof snapshots. |
| SB04-replay-parity | Command batch normalization and replay parity remain stable after journal integration. |

## Shallow-pass trap

A shallow pass could keep an in-memory log that is never exposed or never bounded. The proof requires diagnostics/proof exposure and dropped-entry accounting.

## Adversarial negative proof

`components-stage-journal-audit.txt` exercises more journal entries than the configured cap and proves old entries are dropped while counters stay accurate.

## Semantic positive proof

`components-command-batch-parity-audit.txt`, `components-stage-journal-audit.txt`, and `components-webgllib-tests.txt` pass with exit code 0.

## Anti-stub audit

The journal is connected to the real stage runner and proof snapshot path rather than a test-only fixture.

