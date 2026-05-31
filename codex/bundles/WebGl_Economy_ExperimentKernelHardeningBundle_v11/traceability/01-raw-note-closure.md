# Raw note closure

| Raw note | Literal wording | Normalized requirements | Owning subbundles | Planned proof | Closure |
| --- | --- | --- | --- | --- | --- |
| N01 | WebGL ordered actions can still be broken by motion replacement or by executing staged batches without real stage timing. | R02, R03, R04 | SB02, SB03, SB04 | C# tests, JS audit proof, command transcripts | Solved |
| N02 | Economy input pack validation is too shallow for controlled experiments. | R08 | SB08 | Validator tests for duplicate, missing, invalid hash, unsafe path, and pack hash | Solved |
| N03 | Generic Economy abstractions contain shared-well-specific fields such as DailyWaterNeed and MaxDailyDraw. | R09 | SB09 | Source audit and parameter migration tests | Solved |
| N04 | Transition engine remains event-switch based, scan-heavy, and does not interpret event effects, rule parameters, expected invariants, or input packs as a deterministic run source. | R11, R12, R14, R15 | SB11, SB12, SB14, SB15 | Handler/effect tests, runtime randomness audit, invariant tests | Solved |
| N05 | Shared-well readiness still requires generic inventory, travel-cost, trade/resale, rule/tax/admin, and invariant support. | R13, R14, R15, R16 | SB13, SB14, SB15, SB16 | JSON fixture readiness test and metrics proof | Solved |
| N06 | Farmer-land probe must prevent overfitting the engine to a water/well example. | R17 | SB17 | JSON fixture/readiness test using land, cash, and crop resources | Solved |
| N07 | Do not create a branch, wire cross-repo references, build the shared-well demo, add non-desktop WebGL work, or use runtime randomness for simulation. | R01, R06, R18, R20 | SB01, SB06, SB18, SB20 | Git status, boundary audits, source scans, final closure | Solved |
