# Assumptions And Risks

## Working Assumptions

- `C:\repositories\CanDoItAll.Components` and `C:\repositories\CanDoItAll.Economy` are both available locally.
- Components remains on branch `webgl-engine`; Economy remains on branch `main`.
- Existing tests are valid baseline proof unless a subbundle requires new semantic negative and positive tests.
- WebGL browser proof is required only when rendered behavior or an actual browser route changes.

## Critical Path Risks

- Stage runner proof can pass shallowly if it only checks timers and not motion-completion barriers.
- Bridge proof can pass shallowly if unresolved mappings silently fall back to generic objects.
- Snapshot proof can pass shallowly if tests build snapshots with local helpers instead of reusable production services.
- Probe proof can overfit to well/farmer examples unless generic code is scanned for forbidden terms.

## Validation Risks

- Cross-repo proof needs both Components and Economy command transcripts.
- Some existing package warnings may be unrelated to this bundle and should be recorded separately from failures.
- Large-screen WebGL policy must not be diluted into responsive/mobile work.

## Reopen Triggers

- Components gains Economy references.
- Economy bridge or sandbox dependencies bypass the intended layering.
- A critical proof manifest lacks negative proof, positive proof, source assertions, changed-file hashes, or anti-stub output.
- Any final raw-note closure row remains pending.

