# SB16 Semantic Invariants

## INV-SB16-001 New Foundation Stays Split

- Expected behavior: newly added foundations stay under size thresholds; existing broad files are explicitly monitored.
- Positive proof: `bundle://proof/SB19/transcripts/components-runtime-audit.txt` and `bundle://proof/SB19/transcripts/economy-boundary-audit.txt`.

## Production Behavior Artifact Matrix

| Signal/state | Producer | Consumer | Lifecycle | Negative proof |
|---|---|---|---|---|
| Production/test line-count gates | Audit scripts | Final closure | Scan source and tests | `bundle://proof/SB19/transcripts/economy-boundary-audit.txt` |
