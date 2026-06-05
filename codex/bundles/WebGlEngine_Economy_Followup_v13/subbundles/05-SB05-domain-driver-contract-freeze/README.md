# SB05 - Domain driver contract freeze

Freeze the generic domain driver contract.

Tasks:
- Review `IWebGlRunDomainMappingDriver`, manifest, hash, validator, metadata scrubber.
- Add approval snapshot for driver manifest schema.
- Add a non-Economy fake driver in tests to prove genericity.
- Ensure driver metadata scrubber produces opaque source metadata and drops driver policy metadata.
- Document driver versioning and compatibility.

Required proof:
- Economy driver validation,
- fake non-economy driver validation,
- manifest hash stability test,
- negative test for unsupported generic action mapping.

