# Proof Manifest - SB11

## Status

Completed.

## Changed Files

- Components package version props and package docs.
- WebGlLib-only sample docs.
- Economy package-consumer docs.
- SB11 NuGet proof configs.
- Hashes: `proof/SB12/transcripts/changed-file-hashes.txt`.

## Command Transcripts

- `proof/SB11/transcripts/components-pack-sb11-proof-version.txt`
- `proof/SB11/transcripts/nupkg-file-hashes.txt`
- `proof/SB11/transcripts/nupkg-content-listing.txt`
- `proof/SB11/transcripts/nupkg-content-hygiene-scan.txt`
- `proof/SB11/transcripts/expected-fail-webgllib-only-stale-feed-restore.txt`
- `proof/SB11/transcripts/passing-webgllib-only-fresh-feed-restore.txt`
- `proof/SB11/transcripts/passing-webgllib-only-fresh-feed-build.txt`
- `proof/SB11/transcripts/webgllib-only-sample-package-graph-scan.txt`
- `proof/SB11/transcripts/passing-economy-webglbridge-fresh-feed-restore.txt`
- `proof/SB11/transcripts/passing-economy-webglbridge-fresh-feed-build.txt`
- `proof/SB11/transcripts/economy-webglbridge-package-graph-scan.txt`
- `proof/SB11/transcripts/passing-economy-components-fresh-feed-restore.txt`
- `proof/SB11/transcripts/passing-economy-components-fresh-feed-build.txt`
- `proof/SB11/transcripts/economy-components-package-graph-scan.txt`

## Browser Artifacts

N/A.

## Source Assertions

- `CanDoItAllPackageProofSuffix` creates unique proof versions without changing the default base version.
- Stale-feed restore fails for the SB11 prerelease.
- Fresh-feed package-mode consumers build from isolated caches and graph scans show expected package references.
- WebGlLib-only sample graph contains WebGlLib and does not contain WebGlRunLib.

## Gate Decision

Passed. Package proof is isolated, unique-versioned, and consumer-verified.
