# Implementation Prompt

Use this bundle as the durable source of truth. Before editing source, run the selected subbundle entry gate against `README.md`, `plan/01-phase-plan.md`, the subbundle README, and `traceability/01-requirement-traceability.md`.

Implement only the selected subbundle. Respect the standard-component scope: do not refactor WebGL or Canvas implementation. Prefer existing repo patterns, shared bases, and Tailwind component-layer utilities. When styling changes are made, capture real Playwright screenshots at the required viewports and answer the screenshot review questions while proof is fresh.

For critical subbundles, create `proof/SBxx/manifest.md` and `proof/SBxx/semantic-invariants.md` with changed-file hashes, transcripts, source assertions, anti-stub audit, failing-first or explicit exemption, passing proof, and raw-note literal closure.
