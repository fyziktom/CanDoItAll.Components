# Current-state analysis after v12 execution

## Findings that look good

### F01 - Generic action vocabulary improved

`WebGlRunActionKinds` now exposes `DirectedFlowVisual` instead of `ResourceTransferVisual`. This is
a meaningful improvement because the generic run layer now talks about a directed visual flow rather
than an economic resource transfer.

Required follow-up: freeze the action-kind vocabulary with approval tests and document deprecation
rules. Future new action kinds require a non-economy canary and architectural review.

### F02 - Domain mapping driver pattern exists

`WebGlRunLib` now includes `IWebGlRunDomainMappingDriver`, driver manifest hashing, metadata
scrubbing, and validation. This is the right abstraction for keeping generic rendering and domain
semantics separated.

Required follow-up: harden this as a public contract. Add compatibility tests, driver manifest
approval snapshots, and a non-economy driver canary.

### F03 - Economy bridge uses the driver pattern

`EconomyWebGlRunDomainMappingDriver` maps Economy visual action kinds into generic run action kinds.
`ResourceTransferVisual` maps to `DirectedFlowVisual`, which is the right direction.

Required follow-up: move the driver into its own clear boundary area, avoid default hard-coded driver
construction where DI is available, and make the driver manifest part of exported evidence.

### F04 - Domain leakage scanning exists

Components has a domain leakage workflow and a config that now includes newer canary terms such as
`capital`, `claim`, `credit`, `elite`, `exchange`, `equity`, and `investor`.

Required follow-up: split the audit into hard source/package gates and soft historical docs/bundle
gates. Current allowlists are useful, but too broad for a freeze-grade source gate.

### F05 - Multi-goods-elite canary exists

The new `multi-goods-elite` scenario is structurally different from shared-well/farmer-land:
multiple goods, credit, equity-like claims, capital contribution, policy shock, and dependency.

Required follow-up: prove it is not just a more complicated transfer scenario. Add external oracle
coverage, metamorphic properties, and explicit exchange/investment semantics.

## Main weaknesses

### W01 - Components public API is not frozen

The generic engine has grown quickly. There is no explicit public API baseline, no approval file for
public types/action kinds/options, and no policy describing what changes are allowed after freeze.

Risk: Codex or future work can keep adding generic concepts for Economy needs, slowly turning the
generic engine into an Economy-shaped engine.

Remediation: add public API approval snapshots, action-kind approval, JS runtime API approval, NuGet
package content approval, and a release-candidate freeze gate.

### W02 - Domain leakage audit is good but not freeze-grade

The audit config scans broadly, but also allowlists active bundles, historic docs, and tool files.
That is reasonable for transitional proof, but freeze-grade source gates must be stricter and easier
to reason about.

Risk: domain terms can survive behind an allowlist or in JSON/config/public test fixtures that later
become copied into production examples.

Remediation: split into:
- `generic-source-hard-gate`
- `generic-public-api-hard-gate`
- `package-content-hard-gate`
- `docs-and-bundle-soft-audit`

### W03 - Evidence validation is still partly structural

The readiness evidence model validates records, SHA-256 formatting, schema version fields, bytes, and
band matching. The next step must verify actual files: open artifact, compute bytes/hash, parse JSON,
read schemaVersion, and compare all claims.

Risk: readiness can become artifact-shaped without truly artifact-backed evidence.

Remediation: add `EconomyExperimentEvidenceResolver`, file-backed verification, manifest inclusion,
and negative tests for tampered evidence.

### W04 - Multi-goods-elite semantics are still thin

The canary introduces investment and elite/dependency vocabulary, but SimpleAccounts handlers still
mainly materialize those events as generic transfers/relationships.

Risk: concentration or elite metrics may look meaningful while the model does not yet express pricing,
returns, ownership, or investment feedback strongly enough.

Remediation: introduce an Economy-side exchange/investment semantics driver, while keeping simulation
abstractions generic.

### W05 - Mutation/store resolution layer remains high-risk

`SimpleSimulationStateTransitionEngine.Mutations.cs` still owns transfers, store resolution, policy
resolution, rejection flow generation, severity logic, and metadata propagation.

Risk: a scenario result can be affected by implicit store resolution or rejection policy details
instead of the economic model.

Remediation: split into store resolver, transfer applier, rejection policy, flow provenance writer,
and event-effect applier. Add golden and metamorphic tests around the split.

### W06 - Browser observer proof still needs freeze-grade state comparison

The browser observer should prove that the browser-loaded document and runtime state match expected
hashes and final/settled state. It must not compare expected document to itself or accept a C# fallback
state as browser state.

Risk: visual proof can look green without proving that the browser actually replayed the run.

Remediation: add browser-exported run document hash, live scene content hash, final object position
proof, idle proof, and mismatch negative tests.

### W07 - Components still carries large demo/assets footprint

The compare output shows GLB assets and sandbox/proof artifacts changing frequently. That is acceptable
for development, but a freeze candidate must define what ships in NuGet packages and what is demo-only.

Risk: package consumers get unnecessary assets, unstable demos, or hidden coupling to sandbox content.

Remediation: package content approval, asset package policy, WebGlLib-only sample gate, and versioned
demo assets outside the engine core.
