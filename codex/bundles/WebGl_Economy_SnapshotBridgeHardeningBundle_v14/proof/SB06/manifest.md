# SB06 Proof Manifest

Status: Completed

## Scope

Economy bridge dependency strategy.

## Changed Files

No code changes were required for SB06. The existing bridge project already has conditional local project/package reference support.

## Command Transcripts

| Command | Result | Path |
|---|---|---|
| `powershell -ExecutionPolicy Bypass -File .\scripts\audit-simulation-boundaries.ps1` | Pass | `bundle://proof/SB06/transcripts/economy-boundary-audit.txt` |

## Source Assertions

| Assertion | Path |
|---|---|
| Bridge csproj contains `ComponentsRepoRoot`, conditional `ProjectReference`, package fallback, and no SimpleAccounts/Ledger backend reference. | `bundle://proof/SB06/source-assertions/dependency-strategy-source-assertions.txt` |

## Downstream Decision

SB15 may rely on the bridge dependency strategy: Components references remain isolated to `Simulation.WebGlBridge`, and the bridge has a CI/package path that is not solely a developer-specific checkout.
