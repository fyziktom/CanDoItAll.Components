# SB11 - Economy: distance, inventory and trade primitives

## Problem
The demo needs distance-aware behavior and stockpiling/resale.

## Tasks
Add generic models/contracts, not demo code:
- actor carrying capacity;
- actor inventory/resource store;
- location distance calculator;
- travel cost/duration policy;
- surplus/shortage detector;
- trade offer/request;
- fee/tax/admin burden model.

## Tests
- Closer actor has lower travel duration/cost.
- Actor cannot carry more than capacity.
- Surplus actor can create a sell offer.
- Tax/admin event can be derived from trade event.
