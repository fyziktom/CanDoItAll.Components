# Main weaknesses and remediation

## 1. Generic action vocabulary leakage

`WebGlRunActionKinds.ResourceTransferVisual` is still in the generic Components run layer. Even if many domains can visualize a flow/transfer, the name `ResourceTransfer` biases the generic API toward Economy semantics.

Target remediation:

- Rename to a generic action such as `DirectedFlowVisual`, `ValueFlowVisual`, or `LinkPayloadFlow`.
- Keep Economy names in `CanDoItAll.Economy.Simulation.WebGlBridge` only.
- Provide backward-compatible shim only if necessary, marked obsolete and blocked from new use.

## 2. Domain driver architecture missing

The Economy bridge is effectively already a domain driver, but the architecture does not make that explicit enough.

Target remediation:

- Introduce `IWebGlRunDomainMappingDriver` or equivalent in a generic-neutral way.
- Domain driver owns: domain vocabulary, mapping to generic run actions, boundary terms, validation profile, proof hooks, and oracle fixtures.
- Generic Components package owns only scene/run primitives and driver extension points.

## 3. Browser observer proof can still be too optimistic

Observer proof must compare actual browser state, not expected/fallback state.

Target remediation:

- Export browser scene state and hash after playback.
- Compare expected run-document hash and exported scene content hash.
- Assert object positions from browser snapshot only; no fallback positions in proof.

## 4. Readiness can be inflated by caller flags

Readiness options still expose exercise flags. Research-grade reports should derive them from evidence.

Target remediation:

- Create `EconomyExperimentEvidenceBundle` with artifact paths, hashes, schema versions, and validator results.
- Readiness reporter computes exercised bands from evidence bundle.
- If caller passes only booleans, cap status at exploratory.

## 5. Third scenario is present but not yet a strong genericity canary

The `multi-goods-elite` scenario is excellent, but it must test exchange/investment semantics, concentration metrics, design matrix changes, and visualization without leaking scenario names into generic layers.

Target remediation:

- Add external oracle for third scenario.
- Add design matrix factors for investment size, fee rate, claim issue size, initial wealth distribution, and exchange volume.
- Add metamorphic tests and comparability gates.
