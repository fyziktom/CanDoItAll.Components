# Normalized requirements

| ID | Requirement | Hard gate |
| --- | --- | --- |
| R01 | Stop/Pause must immediately stop browser runtime work before waiting for C# playback task drain. | Browser proof: Pause clicked during active motion/stage; active/queued motions/stages become zero. |
| R02 | Runtime idle wait must be a hard failure whenever proof code requests settled semantics. | No proof path may return success with `runtimeIdle=false`. |
| R03 | Browser observer readiness must require artifact-backed evidence, not boolean flags. | Research-ready fails if evidence artifacts are absent or hashes do not match. |
| R04 | Design matrix factor levels must either mutate input/configuration or be rejected. | Two factor levels must produce different effective configuration hashes or fail as labels-only. |
| R05 | Golden oracle expected values must move to external fixtures. | Test reads oracle JSON; broken expected JSON produces path-addressed diff. |
| R06 | Every diagnostic must be classified into a readiness band. | Unknown diagnostics become explicit `unclassified-diagnostic` failures. |
| R07 | Store resolution must be exhaustively tested across roles and policy types. | Ambiguity in research mode is always error unless explicit policy disambiguates. |
| R08 | Metric/invariant evaluator must provide no-fallback strict mode. | Direct evaluator calls with unknown kinds fail in strict/research mode. |
| R09 | Behavior expansion profiles must be lockfile-backed and diffable. | Changing profile descriptor changes hash and is reported as behavior drift. |
| R10 | Headless runner manifest must include evidence and comparison metadata. | Manifest diff categorizes model, policy, profile, artifact, and environment drift. |
| R11 | Browser observer proof must be detached from economic truth. | Browser failure blocks visual claim but does not rewrite headless run hash. |
| R12 | Performance budgets must prevent non-comparable conclusions. | Headless hard budget failure yields `not-comparable`, not economic failure. |
| R13 | XLSX/checklist and proof manifest must be non-empty and machine-checkable. | Validator rejects empty proof transcripts and missing artifacts. |
