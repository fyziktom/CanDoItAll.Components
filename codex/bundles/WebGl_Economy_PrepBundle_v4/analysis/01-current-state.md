# Current State

- Components is on the existing `webgl-engine` branch and already has a split WebGL runtime with known hardening gaps from `bundle://00_context/01_components_review_findings.md`.
- Economy is on the existing `main` branch and already has Core, Ledger, BusinessObjects, Sdk, Simulator, and Simulator.Components projects.
- The architect bundle originally used numbered folders such as `02_subbundles`; this canonical wrapper keeps those files as authoritative source artifacts while adding validator-compatible gate structure.

