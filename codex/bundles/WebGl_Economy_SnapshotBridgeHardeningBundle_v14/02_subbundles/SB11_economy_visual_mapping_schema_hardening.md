# SB11 - Economy visual mapping schema hardening

Goal:
- Make visual mapping configurable enough for multiple demos.

Tasks:
1. Validate category mappings, node mappings, pose hints, symbol mappings, anchor aliases, action mappings.
2. Add diagnostics for missing asset ids or fallback-heavy mapping.
3. Ensure visual mapping JSON can map both shared-resource and finite-resource scenarios.
4. Add version/schema metadata.
5. Add strict loader for `visual.mapping.json`.

Acceptance:
- Bridge should not hardcode asset/pose/symbol choices.

## Status

Completed.

## Prerequisites

SB05 bridge projection proof.

## Validation Depth

Add or verify strict visual mapping schema/version loader tests, category/node/pose/symbol/anchor/action mapping validation, and missing asset or fallback-heavy diagnostics for both probes.

## Progression Gate

SB15 may proceed only after visual mapping proof shows bridge choices come from configurable schema data rather than hardcoded example assets.
