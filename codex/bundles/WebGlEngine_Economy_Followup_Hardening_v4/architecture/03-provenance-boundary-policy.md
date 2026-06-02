# Provenance boundary policy v2

## Problem

The generic WebGlRun validators currently skip any key starting with `source.`. This makes Economy bridge validation pass, but it has no schema.

## Target

Components should validate provenance shape, not interpret domain content.

Suggested policy:

- Allowed prefix: `source.`
- Required for commands produced by domain bridges:
  - `source.visualActionId`
  - `source.eventId`
  - `source.simulationFrameId`
  - `source.inputPackHash`
- Optional generic keys:
  - `source.kind`
  - `source.domain`
  - `source.traceId`
  - `source.sequence`
  - `source.parentId`
- Max key length: 96
- Max value length: 512 unless explicitly configured.
- Values are opaque strings; Components must not branch on Economy-specific content.
- Non-source metadata remains strict against domain leakage.

Economy-specific validators may additionally require Economy fields and may interpret their values.
