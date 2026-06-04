# Proof Manifest - SB13

Status: completed

Implementation:
- Added Components `WebGlRunObserverProof` and `WebGlRunObserverSnapshot` contracts to compare the expected headless/generic run document hash with the browser-loaded run document hash.
- Surfaced observer proof in the WebGL sandbox RunPlayback diagnostics JSON so browser proof can assert observer status without claiming economic truth.
- Added Components focused coverage for hash match, hash mismatch, runtime-error failure, and source-document immutability.
- Added Economy focused coverage proving browser observer failure blocks visual claims without mutating headless readiness, run hash, scenario pack hash, or gate state.
- Updated Components and Economy docs to define browser/WebGL playback as observer evidence only.

Required artifacts:
- `proof/SB13/browser/observer-boundary-proof.json`
- `proof/SB13/transcripts/headless-vs-browser-hash-tests.txt`

Additional proof:
- `proof/SB13/browser/observer-boundary-proof.cjs`
- `proof/SB13/browser/observer-boundary-proof.png`
- `proof/SB13/transcripts/observer-boundary-browser-proof.txt`
- `proof/SB13/transcripts/playwright-temp-install.txt`
- `proof/SB13/transcripts/webgl-sandbox-sb13.out.txt`
- `proof/SB13/transcripts/webgl-sandbox-sb13-stop.txt`
- `proof/SB13/transcripts/headless-vs-browser-hash-tests-failing-first.txt`
- `proof/SB13/transcripts/headless-vs-browser-hash-tests-components-clean.txt`
- `proof/SB13/transcripts/headless-vs-browser-hash-tests-economy-rerun.txt`
- `proof/SB13/transcripts/webglrunlib-observer-regression-tests.txt`
- `proof/SB13/transcripts/browser-observer-regression-tests.txt`
- `proof/SB13/transcripts/source-assertion-observer-boundary-scan.txt`
- `proof/SB13/transcripts/anti-stub-audit.txt`
- `proof/SB13/transcripts/changed-file-hashes.txt`
- `proof/SB13/transcripts/bundle-validator-prepared-after-sb13.txt`

## Production Behavior Artifact Matrix

| Signal | Producer | Consumer | Lifecycle | Negative-test citation |
|---|---|---|---|---|
| `WebGlRunObserverProofReport` | `WebGlRunObserverProof.Compare` in Components WebGlRunLib | RunPlayback diagnostics, browser proof, Economy readiness interpretation docs | Created from expected document, browser-loaded document, and observer snapshot; valid only when hashes match and runtime/UI are exercised with no errors | `proof/SB13/transcripts/headless-vs-browser-hash-tests.txt` covers document mismatch and runtime error failure |
| `observer.claimStatus` | Components observer proof compare path | Browser proof and Economy readiness classification | Emits `observer-not-run`, `observer-valid`, or `observer-failed`; visual claims require `observer-valid` | `proof/SB13/transcripts/headless-vs-browser-hash-tests.txt` covers `observer-failed`; `proof/SB13/browser/observer-boundary-proof.json` covers valid browser observer |
| Browser observer boundary docs | Economy experiment readiness documentation | Researchers interpreting headless/oracle/browser bands | Browser failure blocks visual demo claims but never mutates headless run/scenario hashes | `proof/SB13/transcripts/headless-vs-browser-hash-tests.txt` covers unchanged headless hashes after observer failure |
