# Semantic invariants - SB13

Status: completed

## Invariants

1. A factor cell must change the effective scenario document.

   Any selected factor that leaves the scenario unchanged must throw `design-factor-no-effect:<factorId>`. The proof report records `design-factor-no-effect:unchangedPointer` for an unchanged JSON pointer cell.

2. Effective scenario hashes are first-class run evidence.

   Every design run record now includes `originalScenarioHash`, `effectiveScenarioHash`, and `appliedEffects`. The design summary records `distinctEffectiveScenarioHashCount`, and SB13 proof requires it to be `2`.

3. Policy-only factor changes still count as scenario changes.

   A policy fee/rule parameter can alter the effective scenario/configuration hash even when the simple model output hash stays stable. SB13 records this explicitly rather than relying only on run/frame hashes.

4. JSON pointer factors are scoped to the scenario model.

   `json-pointer` factors must start at a whitelisted `SimulationScenarioDefinition` root, cannot use empty, `.`, `..`, or `-` segments, and must target an existing node.

5. Typed factor bindings cover the requested families.

   Capacity (`initial-store-capacity`, `resource-capacity`), event schedule (`scheduled-event-step`, `scheduled-event-offset`), investment metadata (`investment-rate`), and policy/fee/threshold (`rule-parameter`, `policy-fee`, `policy-threshold`, `fee`) bindings all route through the materializer and record applied effects.

## Validation

- `proof/SB13/transcripts/design-harness-focused-tests.txt`: focused tests passed 2/2.
- `proof/SB13/factor-effect-report.json`: two runs, two distinct effective scenario hashes, two distinct configuration hashes, no-effect pointer rejected.
- `proof/SB13/design-matrix-summary.json`: every run has `effectiveScenarioHash` and six applied effects.
- `proof/SB13/transcripts/source-assertions.txt`: all source and artifact assertions passed.
