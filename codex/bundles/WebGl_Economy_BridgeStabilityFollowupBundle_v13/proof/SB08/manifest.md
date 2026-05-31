# SB08 Proof - Economy experiment pack strict fixture hashes

## Scope

Committed strict SHA-256 hashes for the shared fixture packs and added tamper tests for fixture documents and pack metadata.

## Changed-file hashes

- `b46d18c13f1350a5dd2b19c21dfb68918e4305530923e7b901e27f96358a8c3e  C:\repositories\CanDoItAll.Economy\tests\CanDoItAll.Economy.Tests\Fixtures\ExperimentInputs\shared-well\experiment.json`
- `6d19af1963a624accebd0f83fbbf176b365259b808fe6c7263686c1b8e22546a  C:\repositories\CanDoItAll.Economy\tests\CanDoItAll.Economy.Tests\Fixtures\ExperimentInputs\shared-well\visual.mapping.json`
- `324708115bf55055423c0d5f0aba942cc25a54cd5f7837916962802ce7af88c9  C:\repositories\CanDoItAll.Economy\tests\CanDoItAll.Economy.Tests\Fixtures\ExperimentInputs\farmer-land\experiment.json`
- `36bb96ffaa2272efde8a2820670a99c9bfd83d73c0f5c037904b20b3e2e51782  C:\repositories\CanDoItAll.Economy\tests\CanDoItAll.Economy.Tests\Fixtures\ExperimentInputs\farmer-land\visual.mapping.json`
- `78d030b615c5c9d0323000b3c79e61cedb97a31da5905730991167d0b51ae82e  C:\repositories\CanDoItAll.Economy\tools\CanDoItAll.Economy.ExperimentInputPackTools\CanDoItAll.Economy.ExperimentInputPackTools.csproj`
- `88971f5de1193a08f7b241ca67c2756bd0359427e6e20e1c70ba9c7f7b0dbff9  C:\repositories\CanDoItAll.Economy\tools\CanDoItAll.Economy.ExperimentInputPackTools\Program.cs`
- `8d872bb80e6dc3e23e27c0a2abb41e658498acdd331af82cdd16e130fa1d455a  C:\repositories\CanDoItAll.Economy\tests\CanDoItAll.Economy.Tests\SimulationExperimentInputPackStrictModeTests.cs`

## Validation transcript

- Hash tool run updated both fixture packs.
- Shared-well pack hash: `sha256:4bfe9154edd648592001d3d2043cf8c6f3368b3303a55794c2e8a1e662504f3d`.
- Farmer-land pack hash: `sha256:6c201edeca5dfb2fb4624c31687f5ee361330d631c5627a91c8336875996d424`.
- Strict fixture tests: pass, 10 tests.
- Full Economy tests: pass, 483 tests.

## Semantic invariants

- Strict mode validates committed fixtures directly, not only temporary generated packs.
- Document tampering and pack metadata tampering are rejected.
- Fixture hashes are real SHA-256 values, not placeholders.
