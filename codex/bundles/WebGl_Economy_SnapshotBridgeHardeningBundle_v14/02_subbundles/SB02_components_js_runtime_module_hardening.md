# SB02 - Components JS runtime module hardening

Goal:
- Keep WebGL JS runtime maintainable without TypeScript.

Tasks:
1. Run `npm run webgllib:audit-scene-runtime`.
2. Report all JS modules over 220 lines.
3. Do not allow any runtime JS file over 320 lines.
4. Split only if adding behavior to an already warning-sized module.
5. Add audit output to proof.
6. Ensure no economy/domain words appear in runtime JS.
7. Ensure scheduler explicitly detects command stage runner state, not only indirect renderRequested flags.

Acceptance:
- Public facade remains under threshold.
- Runtime modules remain acyclic.
- No mobile/small-screen work is introduced.
