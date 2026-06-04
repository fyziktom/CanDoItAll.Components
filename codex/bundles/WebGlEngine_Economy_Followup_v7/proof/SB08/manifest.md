# Proof manifest - SB08

Status: completed

Required artifacts:
- `proof/SB08/transcripts/behavior-profile-tests.txt` - passed focused versioned profile/hash/provenance test.
- `proof/SB08/artifacts/expanded-event-provenance.json` - generated from the real compiler/materializer/readiness reporter.

Additional proof:
- `proof/SB08/transcripts/behavior-profile-tests-failing-first.txt` - failing-first proof that profile hash was absent before implementation.
- `proof/SB08/transcripts/behavior-profile-hardening-tests.txt` - passed `SimulationEconomicTrustHardeningTests`.
- `proof/SB08/transcripts/behavior-profile-transition-metric-tests.txt` - passed `SimulationTransitionAndMetricHardeningTests`.
- `proof/SB08/transcripts/behavior-profile-hash-regression-tests.txt` - passed preparation/input hash regression classes.
- `proof/SB08/transcripts/expanded-event-provenance-export.txt` - artifact export transcript.
- `proof/SB08/refactor-gate-review.md` - forced post-SB08 refactor/gate review.

Implemented:
- Descriptor-backed behavior expansion profiles with id, version, rule-set id, rule fingerprints, and deterministic profile hash.
- Expansion provenance on every explicit or derived event, including `expansionProfileVersion`, `expansionProfileHash`, and `expansionRuleHash`.
- Scenario definition/manifest hash sensitivity to resolved behavior profiles.
- Frame/delta metadata propagation before deterministic frame/hash-chain calculation.
- Readiness, frame-hash artifact, and run-summary profile metadata.
