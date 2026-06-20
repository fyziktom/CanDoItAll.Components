# Main weaknesses and remediations

## 1. Evidence records are not enough
`EconomyExperimentEvidenceValidator` validates fields such as hash format, bytes > 0, and schema version, but it does not itself read the referenced file and compare the real bytes/hash/schema. This is the most important research-readiness gap.

**Fix:** add `IEconomyExperimentEvidenceArtifactResolver` and require artifact-backed validation before any runtime/UI/oracle band can be exercised.

## 2. Raw provenance can leak domain identities
`source.*` metadata is intentionally allowed, but current generic validation treats allowed source keys as traceability and does not enforce opaque values. A source event id like `event.exchange...` can flow into generic run documents.

**Fix:** domain driver emits opaque source ids/hashes into generic run documents and writes a domain-side trace map artifact.

## 3. Domain-driver pattern is not fully operationalized
The generic driver contract exists, but driver manifest/hash must be present in WebGlRunDocument metadata and observer proof. The Economy driver should be split into focused files.

**Fix:** driver manifest/hashes become required proof artifacts and run-document metadata fields.

## 4. SimpleAccounts mutation layer is a semantic hotspot
Store resolution, transfer application, rejected flows, severity policy, and metadata propagation are still in one large mutation file.

**Fix:** split into services and add focused tests for each semantic decision.

## 5. Multi-goods-elite is still a canary, not proof
The third scenario exists, but exchange and investment semantics are largely encoded as transfers/claims.

**Fix:** add an explicit exchange/investment semantic driver and metamorphic tests.

## 6. Domain boundary scans need hard source gates
Components has a stronger audit config, but broad allowlists and docs/bundle paths must not mask source/package regressions.

**Fix:** split hard source/package gate from docs/history audit, with expiring allowlist entries.
