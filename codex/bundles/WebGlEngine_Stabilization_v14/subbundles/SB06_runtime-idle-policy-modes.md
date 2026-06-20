# SB06: Runtime idle policy modes

## Purpose

Make semantic/visual/final-render-drain modes explicit in JS and C# options. Strict proof must not silently accept visual blockers.

## Scope

Repository: `fyziktom/CanDoItAll.Components` branch `webgl-engine` only.  
Do not modify `CanDoItAll.Economy` in this subbundle.

## Required implementation tasks

1. Start with a failing-first audit or test proving the current gap exists or confirming that the gap is already closed.
2. Implement the smallest safe change that satisfies the purpose.
3. Preserve existing behavior unless the subbundle explicitly requires a stricter gate.
4. Update docs/proof artifacts under `codex/bundles/components-webgl-engine-stabilization-followup-v14/proof/SB06`.
5. Record changed files and explain why each change belongs in Components rather than Economy.

## Acceptance criteria

- All relevant tests pass.
- Proof manifest exists and references non-empty transcripts.
- Domain-boundary audit still passes.
- No Economy project is modified.
- Public API changes are either absent or explicitly approved in the API baseline.

## Required proof artifacts

- `proof/SB06/manifest.md`
- `proof/SB06/semantic-invariants.md`
- `proof/SB06/transcripts/failing-first-or-closed-gap.txt`
- `proof/SB06/transcripts/implementation-validation.txt`
- `proof/SB06/transcripts/domain-boundary-audit.txt`
- `proof/SB06/transcripts/changed-files.txt`

## Mandatory QA gate

No

## Notes for Codex

Do not simplify this subbundle by adding a domain-specific shortcut to generic Components. If a test needs a domain vocabulary, create a synthetic non-domain fixture or use the domain-driver options as input without embedding the domain in the generic package.

## Execution status

Status: Completed on 2026-06-05.

Proof manifest: proof/SB06/manifest.md.
Semantic invariant contract: proof/SB06/semantic-invariants.md.
Closure result: Pass; final validation is summarized in eviews/01-execution-report.md.
