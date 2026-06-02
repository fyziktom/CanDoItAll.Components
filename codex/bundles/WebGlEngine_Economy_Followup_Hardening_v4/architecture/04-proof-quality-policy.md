# Proof quality policy

## Required for every critical subbundle

- failing-first proof for the exact bug/risk;
- passing proof after implementation;
- source assertion scan showing the intended implementation exists;
- boundary audit if a package boundary is touched;
- build/test transcript with non-empty command output;
- changed-source hash list after implementation;
- if browser behavior is claimed:
  - screenshot,
  - console errors/warnings,
  - assertion JSON,
  - runtime diagnostics JSON.

## Empty transcript rule

A completed subbundle may include optional empty placeholder files only if they are explicitly listed as optional. Required proof files must have non-whitespace content and must include command line, exit code/result, and at least one semantic assertion or summary.
