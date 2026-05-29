# SB07 - Components refactoring gate A

Repository: `CanDoItAll.Components`

## Goal

Stop and clean up before Economy scenario mapping begins.

## Checks

- No `WebGlLib` runtime JS file above 320 lines.
- No `WebGlRunLib` C# file above 260 lines.
- No sandbox page code-behind above 260 lines without split.
- No direct economy wording inside reusable Components projects.
- Asset cache disposal has proof.
- Scene index sync has proof.
- Command results are unified.

## Output

Write:

```text
artifacts/webgl-economy-followup-v5/components/SB07/refactoring-report.md
```
