# SB06 - Components JavaScript runtime audit

## Goal

Keep JavaScript clean without TypeScript.

## Required audit rules

- Public façade `01-webgl-scene.js` should stay below 180 lines.
- Other runtime modules should warn above 220 lines and fail above 320 lines.
- No `eval`, `new Function`, dynamic script injection, or unguarded `innerHTML`.
- No WebGL runtime files may contain Economy-specific words:
  - economy
  - ledger
  - account
  - water
  - well
  - entrepreneur
  - citizen
- No small/medium/mobile optimization tasks in WebGL runtime docs or prompts.
- All JS modules must pass `node --check`.
- C# and JS batch normalizer behavior must be covered by parity/golden tests.

## Output

Produce an audit report under:

```text
artifacts/webgl-runtime-hardening-v7/
```
