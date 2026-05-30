# Shared-well and farmer-land gap analysis

## Shared-well readiness questions

A minimal but meaningful shared-well experiment needs to answer all of these generically:

1. Where are actors, homes, well, rule council, and stores?
2. How far is each actor from the well?
3. How much water does each actor need per step?
4. What is each actor's carry capacity?
5. Who can store water, how much, and at what cost?
6. Who can resell water, under which rules?
7. What admin burden is created by resale or rule compliance?
8. Who pays tax/fee and to whom?
9. How are rule violations detected?
10. How are trust/conflict/maintenance/well-health updated?
11. Which event sequence should be visualized?
12. What metrics decide whether this institution is good or unstable?

Current code covers part of this:

- actors, locations, resources, stores, scheduled events
- travel/resource use/return/admin/rule events
- basic visual action mapping
- deterministic hashing

Still missing or weak:

- explicit experiment input pack
- placement file separate from scenario
- parameter file separate from scenario
- rules file separate from scenario
- behavior/rule expansion engine
- simple state transition engine from events to frames
- typed references for event source/target
- distance/travel/carry/inventory/trade/tax/admin models
- stage-preserving visual action pipeline

## Farmer-land example

The farmer-land scenario tests a different class of experiment:

- limited land parcels
- farmers want to expand
- external buyers may have unlimited or limited demand
- rules must prevent one oligarch from acquiring everything
- productivity, capital, debt, labor, land quality, transport distance, and market access all matter

This scenario prevents overfitting to the shared-well case. Required generic capabilities:

- finite spatial resource: land parcels / area
- ownership transfer and lease rights
- expansion/investment events
- external demand model
- concentration metrics: e.g. HHI, top-share, Gini-like metric
- anti-concentration rules: caps, progressive fees, community rights, auction rules
- rule impact metrics: efficiency vs fairness vs stability
- versioned treatment variants for experimental comparison

## General conclusion

The code is moving toward the right architecture, but the next wave must focus on the experiment layer and deterministic state transition layer. Otherwise later demos will still be handcrafted frame sequences.
