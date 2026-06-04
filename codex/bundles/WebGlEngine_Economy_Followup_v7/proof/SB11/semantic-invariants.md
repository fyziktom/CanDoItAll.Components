# Semantic invariants - SB11

- Components remains domain-neutral; reproducibility manifest, artifact citations, and diff semantics live in Economy.
- A headless run manifest must identify repo commits, package versions, scenario/catalog pack hashes, behavior profile hash, policy id, seed, frame hash chain, metric hashes, and artifact hashes.
- Required headless artifacts must carry root `schemaVersion` values and be hashable/citable.
- The readiness report must include artifact citations and artifact-set validity metadata.
- Missing or schema-less required artifacts must fail artifact readiness with `artifact-set-incomplete`.
- Two identical catalog runs must compare equivalent except for approved volatile artifact hashes.
- Manifest diffs must categorize scenario, model-output, policy, metric, artifact, repository, and package changes.
- `readiness-report.json` measured performance data is the approved volatile artifact; it remains cited and validated but is excluded from deterministic artifact-set equality.

Production behavior artifact matrix:

| Production signal | Producer | Consumer | Invariant | Proof |
|---|---|---|---|---|
| Manifest v2 reproducibility fields | `EconomyHeadlessRunManifest` and `EconomyHeadlessExperimentRunner.WriteHeadlessRunManifest` | Audit consumers and downstream SB12-SB15 gates | Manifest has enough data to audit a run without opening the browser. | `proof/SB11/transcripts/reproducibility-manifest-tests.txt` |
| Artifact schema/citation validation | `EconomyHeadlessArtifactSetValidator` | Readiness reports and manifest citations | Missing or schema-less artifacts cannot be silently treated as research-ready proof. | `proof/SB11/transcripts/reproducibility-manifest-tests.txt` |
| Manifest diff categories | `EconomyHeadlessRunManifestDiffTool` | Manifest diff sample and audit automation | Scenario/model/policy changes are visible as categorized differences. | `proof/SB11/artifacts/manifest-diff-sample.json` |
