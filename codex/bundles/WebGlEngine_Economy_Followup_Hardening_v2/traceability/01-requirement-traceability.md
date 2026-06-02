# Requirement traceability

| Requirement | Finding | Owning subbundle(s) | Proof expected |
| --- | --- | --- | --- |
| R01 | F01 | SB02, SB11 | Runtime catalog tests, Node browser proof without `tests/` path. |
| R02 | F01 | SB02 | Scenario catalog abstraction and runtime provider proof. |
| R03 | F02 | SB03 | Failing-first direct+staged command loss test; passing preservation/rejection test. |
| R04 | F03 | SB04 | Revision normalization serializer/reducer/import/export tests. |
| R05 | F04 | SB04 | Browser apply adapter reset policy tests. |
| R06 | F05 | SB05 | Strict/permissive patch transaction proof in C# and browser. |
| R07 | F06 | SB06 | Generic/domain provenance validator tests and boundary scans. |
| R08 | F07 | SB07 | Static-only rejection or dynamic object evolving-reference validation proof. |
| R09 | F08 | SB08 | Async asset load/dispose stress and texture ownership proof. |
| R10 | F12 | SB09 | Isolated package-mode restore/build and nupkg/dependency audit. |
| R11 | F10 | SB10 | Updated README/docs and public surface inventory. |
| R12 | F11 | SB11 | Browser proof for Economy Node and Components run playback routes. |
| R13 | F09 | SB01, SB12 | Proof manifest quality audit and final red-team closure. |
| R14 | All | All | Boundary audits and architecture docs. |
