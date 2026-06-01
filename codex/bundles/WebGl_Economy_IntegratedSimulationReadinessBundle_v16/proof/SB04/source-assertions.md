# SB04 Source Assertions

- Command stage journal is bounded, drops old entries when over capacity, and exposes counters and recent results through diagnostics/proof snapshots.
- Delayed stages still leave observable started/applied/completed journal evidence.
- Command batch parity and WebGlLib tests pass after journal integration.

