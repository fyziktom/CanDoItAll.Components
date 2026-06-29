# QA Prompt

Review the selected subbundle as a gatekeeper, not as a prose editor.

## Gate Checklist

- Verify every covered raw note and requirement has implementation evidence or a documented exception.
- Confirm prerequisites and dependency impacts still match `bundle://plan/01-phase-plan.md`.
- Check source references use portable `repo://` or `bundle://` paths and exist.
- Confirm tests and commands have transcripts with command lines and exit codes.
- For critical subbundles, inspect `proof/SBxx/manifest.md` and `proof/SBxx/semantic-invariants.md`.
- Reject shallow proof that only shows file existence, non-empty text, rendered body text, or status-table completion.

## UI Review Questions

- Is the target route or window actually opened in the claimed state?
- Is content readable at maximized desktop, fixed desktop, tablet when applicable, and mobile/narrow widths?
- Is there horizontal page overflow, clipped visible text, hidden buttons, bad z-index layering, or dead available space?
- For floating windows, do drag, resize, minimize, restore, reset, hide, and show states preserve geometry and focus affordances?
- For Canvas, do selection, context menu, quick create, keyboard, zoom, fit/focus, minimap, diagnostics, accessibility mirror, and calendar interactions behave as production features?

## Closure Decision

- Mark the gate `Passed` only when proof artifacts support the claim.
- Mark `Blocked` when the environment prevents necessary browser, test, package, or asset proof.
- Reopen the owning earlier subbundle when a later screenshot or test invalidates a foundation.
