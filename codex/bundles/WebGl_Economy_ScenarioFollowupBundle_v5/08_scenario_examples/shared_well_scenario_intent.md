# Shared well scenario intent

This is not WebGL-specific. It describes what the simulator should be able to represent.

## Actors

- north household
- south household
- free rider
- well keeper
- rule council

## Locations

- north home
- south home
- free rider home
- shared well
- council place
- well keeper workplace

## Resources

- water
- labor
- compliance
- trust
- reserve fund
- well health

## Example event/action chain

Simulation event:

```text
household-north uses shared-well
```

Economy visual actions:

```text
move-to-target: household-north -> shared-well
show-status-symbol: household-north, water-use
show-resource-flow: shared-well -> household-north, water
return-to-home: household-north -> north-home
```

Future WebGlRun actions:

```text
move-to-object
show-symbol
pulse-link
return-to-anchor
```

## Important

The engine must not know this is water. It only knows object IDs and generic action kinds.
