# SB04 - Domain boundary audit hardening

Refine domain leakage scanning into hard and soft gates.

Tasks:
- Split source/package audit from docs/bundle historical audit.
- Generic source gate must have minimal allowlist.
- Add a machine-readable term registry shared by Components tests and CI.
- Include latest canary terms: economy, ledger, market, account, buyer, seller, price, capital,
  claim, credit, elite, exchange, equity, investor, water, well, production-line, work-order,
  machine, farmer, land, parcel, monopoly, oligarchy.
- CI must fail on generic source leakage.
- Economy-side bridge may mention domain terms only inside Economy packages.

Required proof:
- negative test injecting a forbidden term into generic source,
- positive test proving Economy driver can own the term,
- audit transcript with no empty files.

