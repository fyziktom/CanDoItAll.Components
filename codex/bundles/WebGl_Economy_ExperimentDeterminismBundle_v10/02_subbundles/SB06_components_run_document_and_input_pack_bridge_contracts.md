# SB06 — Components: run document and input-pack bridge contracts

## Problem

`WebGlRunDocument` exists, but the experiment layer will need to reference input hashes and provenance without pulling in Economy types.

## Tasks

1. Add generic provenance metadata helpers:
   - `InputDocumentRef`
   - `InputHashRef`
   - `RunSourceRef`
2. Keep them string-based/domain-neutral.
3. Add validator that forbids domain-specific keys in generic WebGL docs.
4. Do not reference Economy projects.

## Done criteria

- WebGlRunLib can record input provenance generically.
- No Economy/ledger/water/farmer words in generic WebGL source.
