# Semantic invariants - SB20

- One command must validate source assets, JS runtime structure, idle policy, command batches, resource ownership, domain boundaries, .NET build/tests, packages, and package-mode samples.
- RC artifacts must include machine-readable JSON, markdown summary, transcript, package directory, and artifact manifest.
- Browser proof may be skipped only with an explicit switch and must be supplied separately for final signoff.
- Package version suffix is explicit and traceable: `0.1.0-rcv16.codex`.

## Production Behavior Artifact Matrix

| Behavior | Producer | Consumer | Lifecycle | Proof |
|---|---|---|---|---|
| RC validation summary | `validate-release-candidate.ps1` | humans and automation | after all gates pass | `repo://artifacts/webgl-engine-rc-v16/validation-summary.json` |
| Package-mode samples | local NuGet config and sample builds | package consumers | after `dotnet pack` | passed rows in `validation-summary.md` |
