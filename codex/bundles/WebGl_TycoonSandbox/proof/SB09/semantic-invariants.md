# SB09 Semantic Invariants

| ID | Behavior | Shallow-pass trap | failing-first proof | passing proof |
|---|---|---|---|---|
| SB09-I1 | Required builds and asset checks pass after implementation. | Only targeted early build passes. | Bundle checklist required full validation. | `bundle://proof/SB09/transcripts/dotnet-build-solution.txt` and npm asset transcripts. |
| SB09-I2 | Browser proof verifies real rendered behavior with screenshots. | Claim visual success from code inspection. | No sandbox route existed in SB01 inventory. | `bundle://proof/SB09/transcripts/browser-final-desktop.png`; `bundle://proof/SB09/transcripts/browser-final-proof.json` |
| SB09-I3 | Hard boundaries remain intact. | Add hidden project references or domain terms. | Bundle hard constraints forbid them. | `bundle://proof/SB09/transcripts/dependency-scan-webgllib.txt`; `bundle://proof/SB09/transcripts/dependency-scan-webglsandbox.txt`; `bundle://proof/SB09/transcripts/forbidden-domain-scan.txt` |

## Semantic Adequacy

- Adversarial negative case: fake proof with static counts would fail canvas image length, runtime namespace, and pointer-selection evidence.
- Semantic positive case: final proof JSON and screenshots are produced from the running Blazor app after a clean solution build.
- anti-stub audit: `bundle://proof/SB09/transcripts/anti-stub-audit.txt`

## Production Behavior Artifact Matrix

See `bundle://proof/SB09/manifest.md#production-behavior-artifact-matrix`.

