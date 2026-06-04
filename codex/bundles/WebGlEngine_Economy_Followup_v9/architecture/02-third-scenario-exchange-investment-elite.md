# Third scenario specification: multi-goods exchange and elite formation

## Goal

Add a structurally different scenario pack named for example `exchange-investment-elite`.

The point is not to produce a final economic theory. The point is to expose genericity gaps in the simulation kernel, metrics, projection and WebGL observer layers.

## Minimal model

Entities:
- 20 small producers/consumers,
- 3 rich capital providers,
- 1 exchange venue or clearing hub,
- 5 goods categories: food, tools, energy, services, capital.

Events:
- production/work events producing different goods,
- exchange/swap events between goods categories,
- investment/capital-contribution events from rich actors to small actors,
- return/fee/claim/repayment events,
- policy shock events such as fee increase or capital access restriction.

Metrics:
- total resource by category,
- HHI by owner and category,
- top owner share by wealth proxy,
- dependency ratio on capital providers,
- unmet demand count,
- transfer/trade volume,
- admin/fee burden.

Invariants/oracles:
- closed resources are conserved except declared production/consumption/fees,
- investment creates a claim/obligation but does not create free goods unless explicitly modeled,
- fees/taxes must appear as flows and not disappear silently,
- no store below zero in ResearchStrict unless explicitly allowed,
- concentration metrics move in expected direction under higher investment advantage.

Visualization:
- generic nodes for actors, goods stores and exchange hub,
- links for dependency/exchange/capital flows,
- symbols for shortage, concentration warning and active investment claim,
- no Components-level concepts named buyer/seller/investor/elite/monopoly.
