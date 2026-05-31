# SB15 Semantic Invariants

## INV-SB15-001 No Mobile Drift

- Expected behavior: WebGL work remains desktop/large-screen only.
- Positive proof: `bundle://proof/SB19/transcripts/components-runtime-audit.txt`.

## Production Behavior Artifact Matrix

| Signal/state | Producer | Consumer | Lifecycle | Negative proof |
|---|---|---|---|---|
| Mobile drift audit | `repo://tools/webgllib/audit-scene-runtime.cjs` | Final audit | Scan docs and bundle prompts | `bundle://proof/SB19/transcripts/components-runtime-audit.txt` |
