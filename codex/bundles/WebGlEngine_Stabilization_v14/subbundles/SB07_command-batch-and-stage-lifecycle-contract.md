# SB07: Command batch and stage lifecycle contract

## Purpose

Freeze accepted/scheduled/settled/failed lifecycle semantics, applyCommandBatch vs applyCommandBatchAndWait behavior, and command-stage runner diagnostics.

## Scope

Repository: `fyziktom/CanDoItAll.Components` branch `webgl-engine` only.  
Do not modify `CanDoItAll.Economy` in this subbundle.

## Required implementation tasks

1. Start with a failing-first audit or test proving the current gap exists or confirming that the gap is already closed.
2. Implement the smallest safe change that satisfies the purpose.
3. Preserve existing behavior unless the subbundle explicitly requires a stricter gate.
4. Update docs/proof artifacts under `codex/bundles/components-webgl-engine-stabilization-followup-v14/proof/SB07`.
5. Record changed files and explain why each change belongs in Components rather than Economy.

## Acceptance criteria

- All relevant tests pass.
- Proof manifest exists and references non-empty transcripts.
- Domain-boundary audit still passes.
- No Economy project is modified.
- Public API changes are either absent or explicitly approved in the API baseline.

## Required proof artifacts

- `proof/SB07/manifest.md`
- `proof/SB07/semantic-invariants.md`
- `proof/SB07/transcripts/failing-first-or-closed-gap.txt`
- `proof/SB07/transcripts/implementation-validation.txt`
- `proof/SB07/transcripts/domain-boundary-audit.txt`
- `proof/SB07/transcripts/changed-files.txt`

## Mandatory QA gate

No

## Notes for Codex

Do not simplify this subbundle by adding a domain-specific shortcut to generic Components. If a test needs a domain vocabulary, create a synthetic non-domain fixture or use the domain-driver options as input without embedding the domain in the generic package.
