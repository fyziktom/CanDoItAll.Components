# SB09 — Bridge diagnostics strictness

## Goal

Avoid silent fallback that hides broken visual mapping.

## Required behavior

Bridge validator should distinguish:

- error: missing source simulation frame id;
- error: missing input pack hash;
- error: stage with no command unless explicit wait marker;
- warning or error depending options: unresolved node/object mapping;
- warning or error depending options: missing asset mapping;
- warning or error depending options: missing pose/symbol mapping;
- warning: fallback object used;
- warning: fallback no-op pose/symbol used.

Add options:

- `AllowFallbackObject`
- `AllowNoOpPoseFallback`
- `AllowNoOpSymbolFallback`
- `TreatUnresolvedMappingAsError`

## Closure proof

- positive strict mapping test;
- negative unresolved node test;
- negative missing asset/symbol/pose test.
