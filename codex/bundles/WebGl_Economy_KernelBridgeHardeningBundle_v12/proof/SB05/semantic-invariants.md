# SB05 Semantic Invariants

## INV-SB05-001 C# And JS Summaries Match Fixtures

- Expected behavior: both normalizers agree on command counts, stage counts, coalescing counts, duplicate-motion handling, and stage summaries.
- Shallow-pass trap: only asserting fixture file existence.
- Positive proof: `bundle://proof/SB05/transcripts/command-batch-parity-audit.txt` and `bundle://proof/SB05/transcripts/webgllib-tests.txt`.

## INV-SB05-002 Ordered Semantics Resist Coalescing

- Expected behavior: metadata flags and patch+motion pose transitions prevent unsafe coalescing.
- Negative proof: `ordered-patch-motion-pose.json` expects two patches and one motion after normalization, not one merged patch.

