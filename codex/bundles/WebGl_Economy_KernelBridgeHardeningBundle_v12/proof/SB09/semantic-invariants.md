# SB09 Semantic Invariants

## INV-SB09-001 Generic Code Has No Example Leakage

- Expected behavior: example-specific terms are allowed only in fixtures, tests, and scenario factories.
- Positive proof: `bundle://proof/SB19/transcripts/economy-boundary-audit.txt`.

## Production Behavior Artifact Matrix

| Signal/state | Producer | Consumer | Lifecycle | Negative proof |
|---|---|---|---|---|
| Forbidden-term scan | `repo://CanDoItAll.Economy/scripts/audit-simulation-boundaries.ps1` | Final closure | Source scan during audit | `bundle://proof/SB19/transcripts/economy-boundary-audit.txt` |
