# SB14 Semantic Invariants

Status: Completed

## Shallow-pass trap

A snapshot serializer roundtrip alone is not enough. SB14 must materialize the shared-resource input pack, choose a rule/admin burden step, attach optional visual playback state, export snapshot JSON, re-import with hash validation, and answer the pause/analyze question from snapshot data.

## Adversarial negative proof

The proof would fail if it inspected WebGL runtime internals directly, skipped the shared-resource fixture, omitted store quantities or relationships, or answered the visual-state question from hard-coded text. The probe analyzes the imported `SimulationRunSnapshot` and asserts admin actors, active issues, store quantities, trust relationships, conflict relationships, top resource holder share, and admin burden.

## Semantic positive proof

`shared-well-step-2-analysis-proof.json` reports 3 actors doing admin work, 1 active issue, store quantities for all four resource stores, a trust relationship, a conflict relationship, top resource holder share `0.884...`, admin burden `35` minutes, hash verification, and visual metadata including node-object/action/stage counts.

## Anti-stub audit

The anti-stub scan found no TODO, `NotImplementedException`, skip, placeholder, stub, or fake markers in the snapshot analysis probe. The targeted test generated both the exported snapshot JSON and the analysis proof JSON.
