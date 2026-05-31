# Semantic invariants SB12

Status: Completed.

## Backend-Neutral Sandbox Orchestration

Invariant ID: SB12-backend-neutral-sandbox-orchestration

`EconomySimulationSandboxWorkflow` orchestrates input loading, backend materialization, visualization mapping, WebGL projection, and snapshot creation through explicit interfaces. SimpleAccounts is now one backend implementation behind `IEconomySimulationBackend`, not a hard-coded workflow dependency.

## Shallow-Pass Trap

A shallow pass would add interface names while still constructing SimpleAccounts, visualization, bridge, and snapshot logic directly inside the workflow. The constructor now accepts all required seams, and `SimulationSandboxWorkflow_AcceptsInjectedFakeBackend` proves an injected fake backend can drive the same visualization, projection, and snapshot stages.

## Adversarial Negative Proof

`WebGlBridgeProject_DoesNotReferenceSimpleAccountsOrLedger` and `bundle://proof/SB12/source-assertions/webgl-bridge-simpleaccounts-ledger-scan.txt` prove the bridge project does not depend on SimpleAccounts or Ledger even while the sandbox project wires the SimpleAccounts adapter.

## Semantic Positive Proof

`SimulationSandboxWorkflow_WiresInputPackBackendVisualFramesAndBridgeWithoutRequiringFinalDemo` proves the default SimpleAccounts path still loads fixture input packs, materializes frames, maps visual frames, projects a WebGL run document, and builds snapshots. The fake backend test proves the same workflow can run without a SimpleAccounts scenario.

## Anti-Stub Audit

`bundle://proof/SB12/source-assertions/anti-stub-scan.txt` records no TODO, NotImplementedException, NotSupportedException, stub, or placeholder markers in the sandbox workflow sources or tests.

## Completed Validator Tokens

Shallow-pass trap: SB12 rejects interface-only seams that still hard-code SimpleAccounts in the workflow.
Adversarial negative proof: fake backend workflow tests prove the orchestrator can run without constructing the SimpleAccounts engine directly.
Semantic positive proof: default and fake backend tests prove input loading, visualization, WebGL projection, and snapshot creation flow through injected pipelines.
Anti-stub audit: SB12 anti-stub source assertion confirms backend-neutral proof is executable code, not placeholders.
