# SB11 - Economy simulation event stream

Repository: `CanDoItAll.Economy`

## Goal

Add explicit simulation events so visualization can animate behavior instead of only showing snapshots.

## Add contracts

```text
SimulationEvent
SimulationEventEffect
SimulationEventParticipant
SimulationEventKind
SimulationEventTiming
```

## Suggested event kinds

```text
resource-use
resource-transfer
travel
return-home
work
administration
trade
loan
repayment
rule-check
rule-violation
rule-enforcement
relationship-change
maintenance
production
consumption
```

## Add to frame/delta

```text
SimulationFrame.Events
SimulationFrameDelta.AddedEvents
```

## Shared-well examples

- household-north travels to well.
- household-north uses water.
- household-north returns home.
- free-rider uses water without maintenance contribution.
- rule-council enforces rule.
- well-keeper performs repair.

## Tests

- events are included in deterministic frame hash;
- playbackSpeed metadata is excluded;
- missing actor/resource refs fail validation.
