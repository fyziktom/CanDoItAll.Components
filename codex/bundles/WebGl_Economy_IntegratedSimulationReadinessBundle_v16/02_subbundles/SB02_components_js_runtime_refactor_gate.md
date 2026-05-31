# SB02 - Components JS runtime refactor gate

## Goal
Keep WebGL JS runtime maintainable without TypeScript.

## Required actions

1. Run JS runtime audit and include line-count table in proof.
2. Identify all runtime JS files above the warning threshold.
3. Split only files that are growing or mixing responsibilities.
4. Add explicit single-responsibility notes to runtime modules.
5. Fix scheduler drift: `resolveRenderReason` should directly consider `commandStageRunner.queue`, `commandStageRunner.waitSeconds`, active motions, queued motions and symbol effects.
6. Keep large-screen-only policy intact.

## Acceptance criteria

- No runtime file exceeds hard threshold.
- Public façade remains thin.
- Runtime modules remain domain-neutral.
- No mobile/tablet/small-screen work is added.
