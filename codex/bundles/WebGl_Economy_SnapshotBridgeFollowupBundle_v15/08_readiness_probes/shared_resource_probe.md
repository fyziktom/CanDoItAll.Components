# Shared-resource readiness probe

Do not implement a "well" demo in this bundle. Use this as a generic probe.

## Generic scenario

- A shared resource store exists at a location.
- Actors have different distances to that location.
- Actors have different inventory capacity.
- One actor can build stock and resell.
- Rules can limit extraction and add tax/fee/admin burden.
- Violations create issues and relationship changes.
- Visualization must show ordered action sequences.
- Snapshot must explain why the visual state looks bad.

## Pass criteria

- No generic code contains the word `well`.
- Resource id may be arbitrary, e.g. `resource.shared`.
- Visual action sequence compiles to stages with movement, pose, symbol, return.
- Snapshot analysis identifies admin pressure, issues, top-holder share, and relationship stress.
