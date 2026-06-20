# SB13 — Docs and troubleshooting

## Purpose

Prevent users from over-trusting exploratory runs.

## Scope

Repositories:

- `fyziktom/CanDoItAll.Components` / branch `webgl-engine`
- `fyziktom/CanDoItAll.Economy` / branch `main`

## Implementation tasks

- Document confidence levels and readiness bands.
- Add troubleshooting for Pause/Play, runtime idle, scenario pack hash failures and strict mode errors.
- Add guidance for designing valid economic experiments.
- Add examples of exploratory vs research-grade workflow.

## Required proof

- Docs link to commands and artifact examples.
- Docs state that UI proof is not economic proof.
- Docs include strict mode setup.

## Refactor gate

Before closing this subbundle, Codex must add a short self-review covering:

- API compatibility,
- generic/domain boundary,
- deterministic behavior,
- performance risk,
- proof adequacy,
- remaining open risks.

## Stop conditions

Do not continue to the next subbundle if a critical proof is browser-screenshot-only, placeholder-only, warning-only where a hard gate is required, or not tied to a source invariant.
