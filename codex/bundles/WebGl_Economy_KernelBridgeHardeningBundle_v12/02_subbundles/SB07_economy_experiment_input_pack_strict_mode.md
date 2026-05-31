# SB07 - Economy Experiment Input Pack Strict Mode

## Problem

The validator accepts placeholder hashes such as `sha256:scenario`. This is useful for early sketches but not acceptable for deterministic experiments.

## Goal

Introduce strict mode.

## Required behavior

- In strict mode, `ContentHash` and hash manifest entries must match:
  - `^sha256:[0-9a-f]{64}$`
- Every input document must have a matching entry in `Hashes.InputHashes`.
- No extra `Hashes.InputHashes` keys are allowed unless explicitly permitted.
- `PackHash` must be validated against canonical content.
- All referenced files must exist when `VerifyDocumentReferences=true`.
- `ResolveInputPath` must verify final path remains under `BaseDirectory`.

## Compatibility

- Keep loose mode for examples or docs.
- Test fixtures used for strict proof must use real hashes.
- Mark placeholder fixtures as non-strict examples if they remain.

## Tests

- strict mode rejects placeholder hash
- strict mode accepts real computed hashes
- path traversal and absolute paths fail
- stale hash fails
