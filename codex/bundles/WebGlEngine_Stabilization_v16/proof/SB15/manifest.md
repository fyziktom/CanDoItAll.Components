# Proof manifest - SB15

Status: completed

## Scope

Domain boundary audit v5 with owner/reason/expiry validation for allowlists.

## Artifacts

- Auditor implementation: `repo://tools/webgllib/domain-boundary-auditor.cjs`
- Audit configuration: `repo://tools/webgllib/domain-boundary-audit.config.json`
- Final changed-file hashes: `bundle://proof/SB22/changed-file-hashes.txt`
- RC transcript: `repo://artifacts/webgl-engine-rc-v16/validate-release-candidate.transcript.txt`
- RC summary: `repo://artifacts/webgl-engine-rc-v16/validation-summary.md`

## Commands

- Domain audits executed inside `powershell -ExecutionPolicy Bypass -File scripts\validate-webgl-rc.ps1 -SkipBrowserProof -PackageProofSuffix '-rcv16.codex'`.

## Result

- `generic-source-hard-gate` passed: 196 scanned files, 0 allowlisted matches in generic source scopes.
- `generic-public-api-hard-gate` passed: 9 scanned files, 0 allowlisted matches.
- `package-content-hard-gate` passed: 60 scanned files, 0 allowlisted matches.
- Allowlist entries now require `glob`, `reason`, `owner`, and non-expired `expires` metadata.

## Production Behavior Artifact Matrix

| Signal | Producer | Consumer | Lifecycle | Negative proof |
|---|---|---|---|---|
| Forbidden term registry | `domain-boundary-audit.config.json` | `domain-boundary-auditor.cjs` | every RC wrapper run | hard gates fail on forbidden terms in generic source/package/API scopes |
| Allowlist metadata | config allowlist entries | auditor validation | before scan results are accepted | missing/expired owner/reason/expiry is rejected |
