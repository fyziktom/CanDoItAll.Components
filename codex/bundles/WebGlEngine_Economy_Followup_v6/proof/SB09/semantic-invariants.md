# Semantic Invariants for SB09

## Invariant SB09-scenario-packs-are-closed-by-default

Source: filesystem scenario catalog and scenario manifest updater.

Expected behavior: every runtime pack file must either be declared in `requiredFiles` and hashed or the manifest must explicitly opt into extra files for exploratory work.

Passing result: `FileSystemScenarioCatalogRejectsUndeclaredExtraPackFileWhenPolicyDisallows` and `ScenarioManifestUpdaterAdoptsCompanionFileWithFreshHashes` passed.

Why this prevents simulator-noise contamination: hidden files cannot change runtime scenario context without being surfaced by catalog validation.

