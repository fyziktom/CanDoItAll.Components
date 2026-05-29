# SB07 — Components refactoring gate A

## Goal

Stop Codex from continuing if the runtime is drifting into a monolith.

## Tasks

Run an audit and fix before continuing:

1. JS module line counts:
   - facade max 180;
   - normal runtime max 320;
   - warning at 220.
2. Razor file line counts:
   - split if > 240 lines unless it is mostly markup.
3. CSS split:
   - sandbox CSS must be split by page/layout/component.
4. No unsafe JS:
   - no unguarded `innerHTML`;
   - no eval/new Function/document.write;
   - no dynamic script injection.
5. No domain leaks:
   - scan WebGlLib/WebGlRunLib for economy/ledger/process keywords.

## Output

`artifacts/webgl-engine-prep-v4/REFACTORING_GATE_A.md`
