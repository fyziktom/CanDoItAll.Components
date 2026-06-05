# Current-state analysis after v11

## Positive changes

- **F01 (Done)** Generic action vocabulary is cleaner: DirectedFlowVisual replaced ResourceTransferVisual in WebGlRunLib. Source: `C-WGRUN-ACTIONKINDS`.
- **F02 (Done/partial)** Generic validator now takes boundary options and scans more scene/run fields. Source: `C-WGRUN-VALIDATOR`.
- **F03 (Done/partial)** Generic domain driver contract exists with manifest/hash/scrubber. Source: `C-DOMAIN-DRIVER`.

## Remaining gaps

- **F04 (Partial)** Source provenance still can carry raw domain IDs/values because source.* keys bypass domain-value term validation. Source: `C-WGRUN-VALIDATOR`. Remediation: Introduce opaque provenance mode and domain-side trace map artifacts.
- **F05 (Partial)** Domain leakage CI is stronger, but allowlists are broad and can hide drift in docs/bundles/tools. Source: `C-DOMAIN-AUDIT`. Remediation: Split source/package hard gate from docs/history audit; enforce allowlist expiry/reason owner.
- **F06 (Partial)** Economy bridge mapping driver exists, but bridge contracts/driver/mapper are still packed into a large mixed file. Source: `E-BRIDGE-DRIVER`. Remediation: Refactor into driver, contracts, mapper, metadata policy files with focused tests.
- **F07 (Open)** Multi-goods-elite exists, but currently models exchange/investment mainly as transfers/claims through SimpleAccounts. Source: `E-MULTIGOODS`. Remediation: Add exchange/investment semantic driver or explicit event handlers with balance-sheet semantics.
- **F08 (Open)** Multi-goods metrics exist but need external oracle and metamorphic proofs. Source: `E-MULTIGOODS-INVARIANTS`. Remediation: Add external JSON oracle cases and monotonic/conservation metamorphic tests.
- **F09 (Open)** Readiness evidence validates shape of evidence records but not actual file existence, bytes, hash, or schema content. Source: `E-READINESS`. Remediation: Add artifact resolver/hash verifier and make research-ready impossible without real file-backed evidence.
- **F10 (Open)** SimpleSimulationStateTransitionEngine.Mutations remains a high-risk concentration of semantics. Source: `E-MUTATIONS`. Remediation: Split into store resolver, transfer engine, rejection policy, flow factory, diagnostic classifier.
- **F11 (Open)** Economy boundary audit has line-count gates but needs CI integration and updated allowlist for refactor state. Source: `E-BOUNDARY-AUDIT`. Remediation: Run as mandatory gate and fail bundle closure if disabled or not cited.
- **F12 (Open)** We still need non-economy generic driver canary to prove WebGlRunLib is not tuned to economic visual flows. Source: `C-DOMAIN-DRIVER`. Remediation: Add manufacturing/logistics-neutral or graph-process driver using DirectedFlowVisual and non-economic symbols.

## Research-readiness answer

The system is usable for exploratory runs and engineering analysis. For research-grade economic conclusions, require strict headless validity, real artifact-backed evidence, an external oracle corpus, deterministic manifests, and explicit browser observer proof. Browser/WebGL must never become the economic source of truth.
