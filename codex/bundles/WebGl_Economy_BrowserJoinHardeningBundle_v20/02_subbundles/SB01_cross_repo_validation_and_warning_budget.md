# SB01 - Cross-repo validation and warning budget

## Goal

Keep validation honest and reduce noise from unrelated legacy warnings.

## Tasks

- Keep full build/test commands.
- Add a `validation-warning-budget.md` for Economy.
- Classify known warnings:
  - legacy tolerated for now,
  - security warning that must be tracked,
  - new warning that must fail the simulation/bridge gate.
- Add a focused simulation/bridge build/test command that avoids unrelated projects where possible.
- Preserve full-solution validation as a separate transcript.

## Acceptance

- Existing transcripts are non-empty.
- New transcripts are non-empty.
- Components build remains 0 warnings.
- Economy simulation/bridge focused test has no new warnings.
- Full Economy solution warning count is documented and bounded.
