# SB15 proof manifest

## Scope

Economy farmer-land generalization probe.

## Changed files

- `repo://CanDoItAll.Economy/tests/CanDoItAll.Economy.Tests/Fixtures/ExperimentInputs/farmer-land/experiment.json`
- `repo://CanDoItAll.Economy/tests/CanDoItAll.Economy.Tests/SimulationExperimentInputTests.cs`

## Proof

- Transcript: `bundle://proof/SB08/transcripts/economy-validation.txt`
- Anti-stub audit: `bundle://proof/SB02/transcripts/anti-stub-audit.txt`
- Semantic invariants: `bundle://proof/SB15/semantic-invariants.md`

## Failing-first / semantic proof

`FarmerLandProbe_ValidatesGenericContractsWithoutBuildingFullSimulation` proves finite land parcels, ownership, external demand, rules, and concentration metrics can be represented without a full simulation implementation.

## Production Behavior Artifact Matrix

| Producer | Consumer | Lifecycle | Negative proof |
| --- | --- | --- | --- |
| Farmer-land test input pack | experiment validator and hash contract | non-UI fixture -> generic contract validation -> follow-up surface | No farmer-land-specific runtime branch or full simulation was added. |
