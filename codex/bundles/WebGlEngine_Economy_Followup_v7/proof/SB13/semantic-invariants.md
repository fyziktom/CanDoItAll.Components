# Semantic Invariants - SB13

| Invariant | Implementation surface | Guardrail | Proof |
|---|---|---|---|
| Components remain domain-neutral | `WebGlRunObserverProof`, RunPlayback diagnostics, Components tests | Observer proof hashes generic WebGL run documents and does not include Economy scenario, ledger, market, or policy semantics. | `proof/SB13/transcripts/source-assertion-observer-boundary-scan.txt` |
| Browser observer is not economic truth | Economy readiness docs and focused test | Browser/WebGL failure blocks `browser-observer-valid` and visual demo claims only; headless/oracle status is interpreted separately. | `proof/SB13/transcripts/headless-vs-browser-hash-tests.txt` |
| Browser-loaded document must match expected document | `WebGlRunObserverProof.Compare` | Any expected/browser-loaded document hash mismatch emits `browser-document-hash-mismatch`, `observer-failed`, and invalid observer proof. | `proof/SB13/transcripts/headless-vs-browser-hash-tests.txt` |
| Runtime and UI failures cannot be ignored | `WebGlRunObserverSnapshot`, `WebGlRunObserverProofReport` | Missing runtime/UI exercise or runtime/UI errors turn the observer report invalid even when document hashes match. | `proof/SB13/transcripts/headless-vs-browser-hash-tests.txt` |
| Visualization actions do not mutate source artifacts | Browser proof and Economy focused test | Browser visual stepping leaves generic document hashes stable; simulated browser failure leaves headless run and scenario pack hashes unchanged. | `proof/SB13/browser/observer-boundary-proof.json` and `proof/SB13/transcripts/headless-vs-browser-hash-tests.txt` |

## Production Behavior Artifact Matrix

| Signal | Producer | Consumer | Lifecycle | Negative-test citation |
|---|---|---|---|---|
| `expectedDocumentHash` / `browserLoadedDocumentHash` | `WebGlRunObserverProof.ComputeDocumentHash` | Observer report, browser proof, readiness interpretation docs | Computed from run id, initial scene content hash, deterministic timeline identity, and metadata; match is required for `observer-valid` | `proof/SB13/transcripts/headless-vs-browser-hash-tests.txt` exercises mismatch failure |
| Runtime/UI exercised metadata | RunPlayback `BuildObserverSnapshot` | Observer proof report and browser proof | Captured from runtime diagnostics and proof snapshot; missing exercise produces `observer-not-run` | `proof/SB13/browser/observer-boundary-proof.json` proves true browser capture; focused tests cover not-valid failure paths |
| `observerProofValid` / `claimStatus` | `WebGlRunObserverProof.Compare` | Browser proof and Economy documentation | Converts hash/runtime/UI state into valid, not-run, or failed visual observer claim | `proof/SB13/transcripts/headless-vs-browser-hash-tests.txt` covers runtime-error `observer-failed` |
