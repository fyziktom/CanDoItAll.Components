# Domain driver contract v2

A domain driver must provide:

- Stable `DriverId`
- Stable semantic version
- Deterministic driver hash
- Mapping from domain action kinds to generic action kinds
- Boundary options/forbidden term registry
- Metadata scrubber
- Opaque source provenance policy
- Trace-map artifact schema

Every generated WebGlRunDocument produced by a domain bridge must include:

- `driver.id`
- `driver.version`
- `driver.hash`
- `driver.manifestHash`
- `source.traceMapHash` when opaque source provenance is enabled

The generic layer must validate driver manifest shape but must not know Economy-specific terms.
