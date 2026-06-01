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
