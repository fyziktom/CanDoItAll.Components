# Semantic invariants - SB08

- Components remains domain-neutral; behavior expansion semantics live in Economy.
- Strict/research scenarios must declare a known behavior expansion profile.
- A compiled stream must expose expansion profile id, version, and hash.
- Every explicit or derived event must expose profile id, profile version, profile hash, parent provenance, and rule hash.
- Scenario definition and manifest hashes must include the resolved profile descriptor.
- Frame and delta metadata must include the profile before deterministic hashes are calculated.
- Readiness reports and headless artifacts must list the behavior profile so automatic expansion is never hidden in research output.
