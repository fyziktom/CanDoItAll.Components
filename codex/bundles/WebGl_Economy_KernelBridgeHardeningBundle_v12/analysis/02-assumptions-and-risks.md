# Assumptions And Risks

## Assumptions

- The active branches are intentional: `CanDoItAll.Components` on `webgl-engine` and `CanDoItAll.Economy` on `main`.
- The bundle's existing numbered subbundles are the authoritative work order.
- Existing v11/v10 work may already satisfy some v12 requirements; execution still needs fresh proof against the current working tree.
- This wave prepares foundations and bridge contracts only. It does not ship a final shared-well or farmer-land WebGL demo.

## Critical Path Risks

| Risk | Impact | Reopen trigger |
|---|---|---|
| Staged JS execution remains immediate | Bridge-ready sequence semantics are false. | Any staged batch proof shows wait stages are metadata-only or commands from later stages apply before the wait. |
| Motion append remains multi-active per object | Sequential visual movement is nondeterministic. | Any proof shows two same-object motions active at once by default. |
| Strict hashes are only shape-checked | Experiment determinism can be spoofed. | A stale referenced document still validates in strict mode. |
| Loader only checks fixture presence | Readiness proof becomes happy-path scaffolding. | Shared-well or farmer-land loader output lacks loaded documents, hashes, event stream, or diagnostics. |
| Event handler registry is decorative | Genericity proof still depends on core hardcoded branches. | Core transition engine still switches/branches on example-specific IDs or scenario IDs. |
| Bridge skeleton introduces wrong references | Repo boundary is compromised. | Components references Economy, or low-level Economy abstractions reference Components/WebGL. |

## Validation Risks

| Risk | Mitigation |
|---|---|
| Existing tests are broad and may take time. | Run targeted tests during subbundles and the full required commands at final closure. |
| JS runtime proof can pass from structure only. | Add fixture/audit assertions for command order, waits, and motion completion order. |
| Large file gates may fail on pre-existing files. | Record explicit allowed exceptions or split files only when the current subbundle grows them materially. |
| Readiness probe may overfit example data. | Include genericity scans and negative assertions against scenario/resource/actor ID branching. |

## Reopen Triggers

- A downstream subbundle depends on a proof file that does not exist or contains only prose.
- Any cross-repo scan finds an unapproved reference across the bridge boundary.
- Any generic source file contains forbidden shared-well or farmer-land example terms.
- Any required command transcript is missing from the relevant `proof/SBxx/transcripts/` folder.
- Any behavior-changing critical subbundle lacks a semantic positive proof and an adversarial negative proof.

