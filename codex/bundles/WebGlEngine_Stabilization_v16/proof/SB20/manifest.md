# Proof manifest - SB20

Status: completed

## Scope

Single release-candidate validation command.

## Artifacts

- Entry wrapper: `repo://scripts/validate-webgl-rc.ps1`
- RC implementation: `repo://scripts/webgl-engine/validate-release-candidate.ps1`
- Artifact manifest: `repo://artifacts/webgl-engine-rc-v16/artifact-manifest.json`
- Machine summary: `repo://artifacts/webgl-engine-rc-v16/validation-summary.json`
- Markdown summary: `repo://artifacts/webgl-engine-rc-v16/validation-summary.md`
- Transcript: `repo://artifacts/webgl-engine-rc-v16/validate-release-candidate.transcript.txt`
- Packages: `repo://artifacts/webgl-engine-rc-v16/packages/`

## Commands

- `powershell -ExecutionPolicy Bypass -File scripts\validate-webgl-rc.ps1 -SkipBrowserProof -PackageProofSuffix '-rcv16.codex'`

## Result

- RC validation passed with schema `webgl-engine-rc-validation/v2` and package version `0.1.0-rcv16.codex`.
- Steps passed: npm asset verification, scene runtime audits, runtime idle tests, command-batch policy audit, resource ownership, command batch parity, motion queue, stage runner, large scene performance, three domain hard gates, solution build, WebGlLib tests, WebGlRunLib tests, pack, package-mode restores/builds, and WebGlRunLib package-mode sample run.
- Browser proof is intentionally separate in this run because `-SkipBrowserProof` was used; SB17 and SB18 contain fresh browser artifacts.

## Production Behavior Artifact Matrix

| Signal | Producer | Consumer | Lifecycle | Negative proof |
|---|---|---|---|---|
| RC artifact manifest | `validate-release-candidate.ps1` | reviewers/CI | end of RC command | failed step prevents manifest success |
| Package-mode proof | RC wrapper | sample consumers | after pack to local package feed | restore/build/sample failures fail RC command |
