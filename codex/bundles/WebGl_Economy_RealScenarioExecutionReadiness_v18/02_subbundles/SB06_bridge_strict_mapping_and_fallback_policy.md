# SB06 — Bridge Strict Mapping And Fallback Policy

## Goal

Prevent visual problems from being hidden by fallback objects or no-op actions.

## Required actions

- Strengthen bridge validator.
- Add strict projection mode in `EconomyWebGlProjectionOptions` if not already sufficient.
- Make unresolved subject/target object an error in strict mode.
- Make unsupported action kind an error in strict mode.
- Make missing required pose/symbol/asset mappings an error when the visual mapping says they are required.
- Ensure fallback use is explicitly marked in diagnostics and metadata.

## Acceptance

Negative tests must prove strict mode rejects:

- missing node mapping
- missing asset mapping
- unsupported action kind
- missing source event id
- stage with only metadata and no explicit wait marker

Positive tests must prove permissive mode still works for diagnostics/prototyping.
