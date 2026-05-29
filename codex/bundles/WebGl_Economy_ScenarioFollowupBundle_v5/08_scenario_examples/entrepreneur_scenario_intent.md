# Small entrepreneur scenario intent

This is not WebGL-specific.

## Actors

- baker
- farmer
- carpenter
- customer group
- local fund

## Locations

- bakery
- farm
- workshop
- market
- fund office

## Example event/action chains

### Sale

Simulation event:

```text
customer-group buys goods from baker
```

Economy visual actions:

```text
move-to-target: customer-group -> market
show-resource-flow: baker -> customer-group, goods
show-resource-flow: customer-group -> baker, cash
return-to-home: customer-group
```

### Loan paperwork

Simulation event:

```text
local-fund gives loan to baker
```

Economy visual actions:

```text
move-to-target: baker -> fund-office
change-pose: baker, admin-writing
show-status-symbol: baker, document
show-resource-flow: local-fund -> baker, debt-credit
return-to-home: baker
```

## Important

This must be expressed through generic simulation and visual action contracts, not hardcoded into WebGL.
