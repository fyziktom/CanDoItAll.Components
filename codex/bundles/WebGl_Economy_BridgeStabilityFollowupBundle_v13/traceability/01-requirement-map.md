# Requirement map

| Requirement | Owning subbundle(s) | Proof target |
|---|---|---|
| Components remains generic UI/WebGL infrastructure with no Economy references. | SB01, SB09, SB16 | Components project/reference audit and domain-term audit |
| WebGL stage execution uses real stage barriers and per-object queued motion semantics. | SB02, SB15 | Components tests and JS audits for stage runner and motion queue |
| Generic action plans compile into staged command batches. | SB03, SB05 | WebGlRunLib compiler tests and bridge projection tests |
| Economy bridge builds initial scenes with node/object and link projection. | SB04, SB12 | Economy bridge tests validating objects, links, mappings, and provenance |
| Economy bridge emits actual actions/command batches and avoids global action duplication. | SB05, SB15 | Economy bridge tests validating motion/patch batches and step partitioning |
| Bridge dependency on Components supports local project mode and package mode. | SB06 | csproj/property tests and boundary audit |
| Visual mapping uses schema-driven asset, pose, symbol, category, anchor, and fallback decisions. | SB07 | loader/validator tests with shared-well and farmer-land fixtures |
| Committed input packs use real strict SHA-256 hashes. | SB08 | strict fixture tests and hash update tool |
| Simulation transitions and metrics remain generic and mutation-safe. | SB10, SB11 | transition/metric/invariant tests |
| Future joined sandbox belongs in Economy, not Components. | SB13 | compile-only Economy simulation sandbox project |
| WebGL remains desktop/large-screen only. | SB14 | WebGL runtime audit policy |
| Final closure cites artifact-backed commands, hashes, anti-stub audit, and raw-note closure. | SB16 | execution report and proof manifests |
