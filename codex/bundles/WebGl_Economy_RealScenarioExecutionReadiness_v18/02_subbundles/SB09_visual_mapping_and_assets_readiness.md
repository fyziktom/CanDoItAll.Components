# SB09 — Visual Mapping And Assets Readiness

## Goal

Ensure visual mapping is stable enough for the first real large-screen demo.

## Required actions

- Validate visual mapping schema for both probe fixtures.
- Validate category mappings, action mappings, anchors, pose hints and symbol mappings.
- Validate that mapped asset IDs exist in the Components asset catalog or are explicitly no-op/fallback.
- Produce a warning summary for missing pose/symbol assets.
- Keep renderer-neutral visual mapping separate from WebGL-specific bridge code.

## Acceptance

Both probe fixtures must generate a WebGL run document with:

- initial objects
- links
- timeline frames
- stages
- commands
- source traceability
- no unexpected diagnostic fallback in strict mode
