# SB01 Semantic Invariants

## Invariant ID

SB01-warning-boundary-baseline

## Shallow-pass trap

A warning budget file can exist without proving that current validation output is real or that new bridge/sandbox warnings are bounded.

## Adversarial negative proof

`bundle://proof/SB01/transcripts/focused-validation.txt` contains package and source warnings from the current build/test path, proving the gate does not suppress warning output.

## Semantic positive proof

`bundle://proof/SB01/transcripts/components-build-baseline.txt` passed with zero Components warnings, and focused Economy validation passed 15 tests while preserving warning visibility.

## Anti-stub audit

`bundle://proof/SB01/transcripts/anti-stub-audit.txt` confirms the warning budget and validator script do not contain TODO/NotImplemented placeholders.

## Raw-note literal closure

- Do not create a new branch: preserved; branch/status transcript shows the current checked-out branches.
- Components must remain Economy-free: boundary scan found no Economy references in generic Components WebGL libraries.
- Proof transcripts must be non-empty: SB01 transcripts are non-empty and indexed in the manifest.

## Production Behavior Artifact Matrix

| Artifact | Producer | Consumer | Lifecycle | Negative-test citation |
|---|---|---|---|---|
| `repo://codex/validation-warning-budget.md` | SB01 bundle execution | Final validation and warning-gate reviews | Rechecked whenever validation warnings change | `bundle://proof/SB01/transcripts/focused-validation.txt` preserves real warning lines for classification. |
