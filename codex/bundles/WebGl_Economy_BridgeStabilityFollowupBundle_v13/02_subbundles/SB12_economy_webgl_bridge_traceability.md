# SB12 — Bridge traceability and provenance

## Tasks
- Add source references from WebGlRunDocument back to:
  - experiment id/hash
  - scenario definition hash
  - placement hash
  - parameter hash
  - visual mapping hash
  - frame id/hash
  - visual action id
  - event id
- Use a generic source/provenance model, not arbitrary `economy.*` metadata where that conflicts with WebGlRunLib validators.
- Add validator for bridge output.

## Tests
- Every stage has source visual action id.
- Every frame maps to simulation frame id.
- Run document hash changes when input pack hash changes.
