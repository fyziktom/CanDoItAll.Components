# SB01 - Cross-repo validation and warning budget

## Status

Completed. Entry and closure gates passed.

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

## Prerequisites

- Current branch must remain unchanged in both repositories.
- Read root README, validation commands, forbidden reference policy, and traceability.

## Owned Requirements

- R01 Cross-repo validation and warning budget.

## Dependency Impact

All later subbundles depend on this warning and boundary baseline. If this proof is stale, downstream validation cannot separate new regressions from legacy warning noise.

## Validation Depth

Command transcript proof plus source/boundary scans. Record known warnings as bounded budget entries, not passing proof.

## Proof Required

- `bundle://proof/SB01/transcripts/branch-status.txt`
- `bundle://proof/SB01/transcripts/warning-budget.txt`
- `bundle://proof/SB01/transcripts/focused-validation.txt`
- `bundle://proof/SB01/manifest.md`
- `bundle://proof/SB01/semantic-invariants.md`

## Browser Validation Logging

N/A. This is a validation baseline subbundle with no browser-visible surface.

## Semantic Adequacy Gate

- Shallow-pass trap: a warning budget file exists but does not prove new Components/Economy bridge warnings are bounded.
- Adversarial negative proof: scan or command output distinguishes known legacy warnings from a new warning bucket.
- Semantic positive proof: focused Components/Economy validation transcript is non-empty and classifies warning counts.
- Anti-stub audit: proof files and report rows must contain real command output, not placeholder text.

## Progression Gate

Pass only when branch/status, warning budget, focused validation, and boundary baseline are recorded. SB02 must not start if branch or warning state is unknown.
