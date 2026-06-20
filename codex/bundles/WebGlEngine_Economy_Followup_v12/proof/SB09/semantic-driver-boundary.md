# Exchange and investment semantic driver boundary

The SimpleAccounts mutation path now delegates exchange and investment-like primitives through `SimpleExchangeInvestmentSemanticDriver`:

- contribution
- trade sell and trade buy
- claim issue
- return pay
- obligation create

Store resolution, accepted/rejected flow creation, rejection policy, and store-resolution metadata were split out of `SimpleSimulationStateTransitionEngine.Mutations.cs` so the engine no longer concentrates those responsibilities in one file.

Validation:

- `proof/SB09/exchange-investment-driver-tests.txt`
- `proof/SB08/mutation-split-build.txt`
- `proof/SB08/line-count-audit.txt`
