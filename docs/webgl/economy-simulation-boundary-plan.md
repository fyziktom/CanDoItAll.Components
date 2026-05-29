# Future Economy Simulation Boundary

This Components repository remains generic. It may ship `CanDoItAll.Components.WebGlLib` for scene rendering and `CanDoItAll.Components.WebGlRunLib` for playback over scene patches, but it must not implement economy rules.

## Future Economy Projects

The future Economy repository should own these projects:

| Project | Allowed dependencies | Responsibility |
| --- | --- | --- |
| `CanDoItAll.Economy.Simulation.Abstractions` | None or core-only primitives | Economic run ids, scenario ids, actors, resources, flows, observations, backend/projector interfaces. |
| `CanDoItAll.Economy.Simulation.SimpleAccounts` | `Simulation.Abstractions`, `Economy.Accounts` | Simple-account backend implementation. |
| `CanDoItAll.Economy.Simulation.Ledger` | `Simulation.Abstractions`, `Economy.Ledger` | Ledger-backed backend implementation. |
| `CanDoItAll.Economy.Simulation.Visualization` | `Simulation.Abstractions`, WebGL packages | Project economy run frames into generic `WebGlRunLib` scene patches. |

## Forbidden In Shared Abstractions

- Ledger transaction classes.
- UTXO assumptions.
- Simple-account balance mutation classes.
- EF Core or persistence-specific records.
- UI components or WebGL runtime details.
- Scenario-specific well/community/business rules.

## Proof Expected Later

The Economy repo should add architecture tests proving:

- Abstractions do not reference Ledger or Accounts.
- Ledger adapters do not reference SimpleAccounts.
- SimpleAccounts adapters do not reference Ledger.
- Visualization depends on generic WebGL packages only at the projection boundary.

The shared-well proof belongs in the Economy repo: compare SimpleAccounts and Ledger backend observations through shared abstractions, then project those observations into `WebGlRunLib` patches without pushing economy terms into Components.
