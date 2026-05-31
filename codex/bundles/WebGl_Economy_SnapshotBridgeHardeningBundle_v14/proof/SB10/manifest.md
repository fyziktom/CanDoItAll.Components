# SB10 Proof Manifest

Status: Completed

## Scope

Economy generic-domain leakage audit for simulation abstractions, simple accounts, visualization, and WebGL bridge production code.

## Changed Files

| File | SHA-256 proof |
|---|---|
| No production source changes were required. | `bundle://proof/SB10/source-assertions/strict-generic-domain-term-scan.txt` |

## Command Transcripts

| Command | Result | Path |
|---|---|---|
| `powershell -ExecutionPolicy Bypass -File .\scripts\audit-simulation-boundaries.ps1` | Pass | `bundle://proof/SB10/transcripts/simulation-boundary-audit.txt` |

## Source Assertions

| Assertion | Path |
|---|---|
| Strict scan found no forbidden example-domain terms in generic production files after excluding allowlisted scenario factories/materializers/policies. | `bundle://proof/SB10/source-assertions/strict-generic-domain-term-scan.txt` |
| Full scan captures example-domain terms only in allowlisted factories/tests/docs/probe contexts. | `bundle://proof/SB10/source-assertions/allowlisted-domain-term-scan.txt` |
| Generic model/source scan shows resource/capacity/ownership/transfer/rule/relationship/issue terminology remains the production vocabulary. | `bundle://proof/SB10/source-assertions/generic-resource-concept-source-scan.txt` |

## Behavior Artifacts

| Artifact | Path |
|---|---|
| Boundary audit covering project references, line-count gates, Components/WebGL boundaries, backend boundaries, deterministic clock/randomness guard, and example-domain leakage guard | `bundle://proof/SB10/transcripts/simulation-boundary-audit.txt` |

## Semantic Gate

See `bundle://proof/SB10/semantic-invariants.md`.
