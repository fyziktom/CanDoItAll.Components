# SB04 - Components JS runtime size/refactor gate

Codex must run and extend the WebGL runtime audit.

Additional requirements:

- no scene runtime JS file above 320 lines,
- warning above 220 lines,
- public facade under 180 lines,
- no circular imports,
- no duplicate command-result helpers,
- no `innerHTML` without static allowlist,
- no Economy terms in Components runtime.

Do not migrate to TypeScript.

## Status

Completed.

## Goal

Keep the Components JavaScript runtime maintainable, generic, and within the stated runtime audit thresholds.

## Prerequisites

- SB01 must pass.
- SB02/SB03 source changes must be present before final audit if they edit runtime files.

## Owned Requirements

- R04 Components Runtime Audit.

## Dependency Impact

Protects downstream Economy proof from depending on oversized or domain-coupled Components runtime files.

## Validation Depth

Audit-backed gate with source scan and line-count proof.

## Proof Required

- `node tools/webgllib/audit-scene-runtime.cjs` transcript.
- Source assertions for thresholds, circular imports, duplicate helpers, unsafe `innerHTML`, and forbidden terms.
- Proof manifest.

## Progression Gate

Pass only when the audit passes without TypeScript migration and without Economy/domain terms in Components runtime files.
