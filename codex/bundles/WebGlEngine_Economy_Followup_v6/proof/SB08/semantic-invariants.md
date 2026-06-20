# Semantic Invariants for SB08

## Invariant SB08-expansion-profile-controls-generated-events

Source: scenario event compiler and behavior expander.

Expected behavior: strict scenarios must name an expansion profile; `none` preserves base events, while enabled profiles generate only the documented expansion families with parent/profile provenance.

Passing result: `BehaviorExpansionProfiles_DisabledVsEnabledAndCarryProvenance` and strict missing-profile validation tests passed.

Why this prevents simulator-noise contamination: implicit behavior expansion cannot add economic actions that the scenario author did not explicitly allow.

