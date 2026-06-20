# Normalized requirements

R01. Stabilize `CanDoItAll.Components.WebGlLib` and `WebGlRunLib` so future work can move primarily
to Economy.

R02. Add a Components freeze gate with public API approval, action-kind approval, JS runtime API
approval, package-content approval, and browser proof.

R03. Keep `WebGlLib` lightweight and usable without `WebGlRunLib`.

R04. Keep all economic, production-line, market, investor, claim, credit, water/well, or scenario
specific semantics out of generic Components.

R05. Formalize domain-driver contracts so unavoidable domain mappings are owned by domain packages.

R06. Make readiness reports artifact-backed by verifying actual bytes, SHA-256 hashes, and schema
versions of referenced artifacts.

R07. Treat browser/WebGL as observer proof only. Economic truth must be headless.

R08. Close the `multi-goods-elite` canary as a research-style scenario, not only as a runtime demo.

R09. Split high-risk Economy mutation/store-resolution code before relying on complex scenarios.

R10. Add explicit gates that prevent simulator/runtime/projection bugs from being mistaken for
economic model outcomes.

R11. Produce non-empty proof artifacts and reject empty transcripts in bundle execution.

R12. After the freeze gate, require an explicit architecture decision to reopen generic Components.
