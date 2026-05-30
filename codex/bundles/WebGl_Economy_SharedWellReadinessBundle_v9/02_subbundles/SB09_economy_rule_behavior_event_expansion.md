# SB09 - Economy: rule/behavior driven event expansion

## Problem
The shared-well scenario still manually declares many concrete events.

## Tasks
- Add behavior/rule expansion contracts:
  - `ISimulationScenarioBehaviorExpander`
  - `ISimulationRuleEventExpander`
  - `SimulationBehaviorExpansionContext`
- Implement generic simple expanders:
  - resource need/use;
  - collect resource from nearest source;
  - return home;
  - perform administration;
  - apply rule check;
  - trade/sell surplus;
  - tax/fee/admin overhead.
- Do not hardcode shared-well logic into abstractions.

## Tests
Generate a small event stream from generic rule/behavior definitions, not only from explicit scheduled events.
