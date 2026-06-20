# SB09 - Economy evidence resolver and artifact-backed readiness

Make readiness truly artifact-backed.

Tasks:
- Add `EconomyExperimentEvidenceResolver` that opens files/streams, computes bytes, SHA-256, and
  JSON schemaVersion.
- Evidence validation must compare actual artifact bytes/hash/schema to claimed evidence records.
- Runtime/UI/oracle exercised flags must be derived from evidence, not from booleans alone.
- Keep booleans as request intent only.

Required proof:
- positive valid artifact evidence,
- tampered artifact negative test,
- missing artifact negative test,
- schema mismatch negative test,
- readiness report showing derived evidence counts.

