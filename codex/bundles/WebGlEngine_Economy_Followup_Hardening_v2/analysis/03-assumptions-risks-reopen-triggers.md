# Assumptions, risks, and reopen triggers

## Assumptions

- Codex has both repositories checked out locally and can use project references between them during local development.
- `CanDoItAll.Components` work happens on `webgl-engine` unless the execution agent confirms a newer target branch.
- `CanDoItAll.Economy` work happens on `main` unless the execution agent confirms a newer target branch.
- The previous bundle is considered implemented but not final-quality complete.
- WebGlLib should remain useful for light visual-only consumers; WebGlRunLib is optional.
- Economy is a first domain consumer but must not force economy semantics into Components packages.

## Critical path risks

1. Runtime fixtures from `tests/` make the Node route unusable outside a repo checkout.
2. Mixed frame-level and staged commands can silently lose commands.
3. Generic validation can become too strict and reject legitimate domain provenance, or too weak and allow domain vocabulary into generic contracts.
4. Package version `0.1.0` is reused heavily and can hide stale package-cache problems.
5. More browser proof without deployment-like scenario loading may falsely pass.

## Validation risks

- Build/test success alone is insufficient because the risky behavior is semantic: command loss, package-mode source selection, runtime fixture loading, and replay reset policy.
- Browser proof against a repo checkout is not deployment proof.
- Source assertions that only grep for symbols are supporting evidence, not semantic proof.
- Proof manifests must cite non-empty transcript files and actual changed file hashes.

## Reopen triggers

Reopen the relevant earlier subbundle if any later phase finds:

- a runtime path under `tests/` used by product or Node UI;
- a frame with both direct and staged commands where direct commands are silently ignored;
- `Scene.Revision` and `Scene.UiState.Revision` diverging after import/export/normalize/commit;
- a browser reset that uses stale runtime options or loses expected runtime profile;
- dynamic object references rejected without an explicit static-scene policy;
- package-mode build succeeding only because of stale global NuGet cache;
- proof manifests citing empty or missing transcripts.
