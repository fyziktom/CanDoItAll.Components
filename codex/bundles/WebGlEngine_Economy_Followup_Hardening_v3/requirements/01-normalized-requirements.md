# Normalized Requirements

| ID | Requirement | Priority | Owning subbundle |
|---|---|---:|---|
| R01 | Audit the pushed v2 implementation and distinguish real proof from placeholders or weak transcripts. | P0 | SB01 |
| R02 | Add reusable Economy simulation sandbox service-registration APIs for non-Node consumers. | P0 | SB02 |
| R03 | Turn runtime scenarios into portable scenario packs with manifest/version/hash metadata. | P0 | SB03 |
| R04 | Add a real scenario selection UX and invalid/missing scenario state in the sandbox page. | P1 | SB03, SB09 |
| R05 | Add pathless/session-by-scenario APIs and portable session export/import semantics. | P0 | SB04 |
| R06 | Replace sync-over-async snapshot persistence with async APIs or isolated background-safe behavior. | P0 | SB05 |
| R07 | Make WebGlRun frame apply enforce mixed-command policy itself; no silent drops. | P0 | SB06 |
| R08 | Make browser apply fail fast when required scene reset is impossible or import fails. | P0 | SB06 |
| R09 | Preserve/apply WebGlSceneDocument runtime options during document import/reset. | P0 | SB07 |
| R10 | Harden `source.*` provenance into typed, bounded, non-executable traceability. | P0 | SB08 |
| R11 | Improve browser UI diagnostics so users see selected scenario, validity, frame/stage state, and runtime apply errors. | P1 | SB09 |
| R12 | Define resource/performance budgets and warnings for large scenes/assets/motions/stages. | P1 | SB10 |
| R13 | Harden package versioning/fresh-feed proof and WebGlLib-only consumer proof. | P1 | SB11 |
| R14 | Complete final cross-repo red-team proof with builds, tests, package-mode, browser large/narrow, and stress checks. | P0 | SB12 |
