# Performance risk register

## Hot paths

- Event normalization over many events.
- Transition engine store lookup.
- Visual frame mapping.
- Bridge projection from visual actions to WebGL stages.
- Command batch normalization.
- WebGL motion queue and stage runner.
- Snapshot serialization and hashing.

## Required probes

- 250 actors.
- 500 stores.
- 1000 events.
- 1000 visual actions.
- 500 staged WebGL commands.
- 100 snapshots.

## Metrics

- simulation materialization ms
- visual mapping ms
- bridge projection ms
- snapshot export/import ms
- snapshot diff ms
- command batch normalization ms
- average/peak WebGL frame time if browser proof runs

## Large-screen policy

All browser proof must use 1440x900 or larger. No mobile/tablet/small-screen proof.
