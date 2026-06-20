# Research readiness checklist

| Gate | Required evidence | v12 status |
| --- | --- | --- |
| Scenario pack and strict load | Valid input pack, references, hashes, strict policy | Passed for `multi-goods-elite` |
| Headless simulation correctness | Stores, flows, relationships, metrics, invariants | Passed |
| Artifact-backed evidence | Existing files, byte counts, sha256, schemaVersion | Passed |
| External oracle corpus | Golden oracle corpus plus negative diff proof | Passed in tests; final CLI canary run used `--no-oracle` |
| Browser observer proof | Browser-loaded document hash, scene hash, driver hash, runtime idle, completed stages, final positions | Passed for generic `/run-playback` route |
| Warning and diagnostic firewall | Classified diagnostics and unclassified negative test | Passed |
| Performance comparability | Headless hard failures mark not-comparable; browser-only failures are observer warnings | Passed |
| Research ready | All gates plus oracle and browser evidence | Not claimed for the no-oracle CLI canary run |
