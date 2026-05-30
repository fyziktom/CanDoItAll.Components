# SB15 — Economy: SimpleAccounts materializer refactor

## Problem
Scenario factories still materialize many frames manually.

## Required work
- Split scenario definition factories from materialization.
- Materializer should consume `SimulationEventStream`.
- State transition logic should apply events to stores/relationships/issues.
- Shared-well/entrepreneur factories should mostly define initial state + event/rule definitions.
- Add consistency tests for every generated frame/delta.
