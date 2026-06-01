# SB15 Semantic Invariants

| Invariant ID | Shallow-pass trap | Adversarial negative proof | Semantic positive proof | Anti-stub audit |
|---|---|---|---|---|
| SB15-CLOSE-001 | A closure report can look complete while transcripts are empty, critical proof files are missing, raw notes remain open, or the final validator has not accepted the bundle. | `bundle://proof/SB15/transcripts/non-empty-transcript-check.txt`, `bundle://proof/SB15/transcripts/critical-proof-manifest-audit.txt`, and the completed-stage validator reject empty transcripts, missing critical manifests, and incomplete statuses. | SB15 records passing Components build/tests/audits, passing Economy build/full tests/boundary audits, closed raw notes, final fake-proof resistance, and completed validator proof in `bundle://proof/SB15/manifest.md`. | `bundle://proof/SB15/transcripts/final-anti-stub-audit.txt` scans changed production and proof-critical files for placeholder, fake, stub, TODO, and NotImplemented indicators before final closure. |

## Closure Contract

SB15 may pass only when final proof is command-backed, non-empty, and consistent with the source state that the bundle leaves behind.

## Required Invariants

- Components build, WebGlLib tests, WebGlRunLib tests, runtime audit, asset verification, and GLB inventory all have real non-empty transcripts.
- Economy build, full test suite, boundary audit, real scenario headless runner, and strict input pack validation all have real non-empty transcripts.
- The Economy renderer-neutral boundary audit must pass after all SB15 source cleanups.
- Full Economy tests must pass after the renderer-neutral string fix, neutral runtime default, analyzer split, and test split.
- Critical subbundles must have both `manifest.md` and `semantic-invariants.md`.
- The final fake-proof resistance artifact must summarize evidence and warnings without treating warnings as hidden failures.
- The completed-stage bundle validator must pass after status/report updates.

## Guardrails Preserved

- Components remains generic and Economy-free.
- Joined simulation plus visualization remains in Economy.
- No final UI demo was added.
- Large-screen-only browser proof remains deferred to the planned generated-document loader/comparator path.
- No branch, stage, commit, or push operation was performed.

## Known Warning-Only Conditions

- Economy build output still includes existing `ncalc` compatibility warnings.
- Economy solution build still includes existing IPFS/OpenTelemetry advisory warnings.
- Components scene runtime audit still reports existing JavaScript file-size warnings while exiting successfully.
- Git diff checks report line-ending normalization warnings only and `LASTEXITCODE=0`.
