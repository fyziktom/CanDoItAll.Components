# SB11 - Exchange/investment semantic driver

Avoid treating investment/elite formation as just opaque transfers.

Tasks:
- Add Economy-side semantic driver for exchange/investment/claim/return/dependency, without changing
  generic simulation abstractions unless absolutely required.
- Make valuation/price/return assumptions explicit in scenario metadata or policy files.
- Add metrics for concentration, dependency, return extraction, and liquidity.
- Keep SimpleAccounts generic but allow driver-specific handlers if needed.

Required proof:
- semantic driver tests,
- no generic Components changes,
- no hidden fallback from unknown investment events,
- canary metrics change in expected direction.

