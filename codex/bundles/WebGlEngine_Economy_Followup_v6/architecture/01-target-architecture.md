# Target architecture additions

## 1. Experiment trust boundary

The economic simulation stack must produce three separate artifact classes:

```text
Economic model artifacts:
  scenario definition
  event stream
  simulation frames/deltas
  metrics/invariants
  deterministic hashes

Projection artifacts:
  visual frames
  WebGlRunDocument
  scene/run mapping diagnostics

Runtime artifacts:
  browser playback diagnostics
  WebGL proof snapshots
  pause/idle/runtime performance proof
```

A runtime artifact failure must not automatically invalidate the economic model artifact. It should classify the result as "model computed, visualization failed".

## 2. Strict mode

Introduce:

```csharp
public enum SimulationExperimentMode
{
    Exploratory,
    Strict,
    ResearchGrade
}
```

Strict/research-grade runs should fail on:

- unknown event kind,
- unknown metric kind,
- unknown invariant kind,
- missing actor/resource/store reference,
- ambiguous store resolution,
- insufficient stock unless explicitly allowed,
- rejected flow unless the scenario explicitly models rationing,
- unapproved warning category,
- non-deterministic hash,
- invalid scenario pack hash.

## 3. Readiness report

Create a top-level report:

```csharp
public sealed class EconomyExperimentReadinessReport
{
    public string ExperimentId { get; set; }
    public string ScenarioPackHash { get; set; }
    public string RunHash { get; set; }
    public ValidityBand Scenario { get; set; }
    public ValidityBand Simulation { get; set; }
    public ValidityBand Metrics { get; set; }
    public ValidityBand Projection { get; set; }
    public ValidityBand Runtime { get; set; }
    public ValidityBand Performance { get; set; }
    public string ConfidenceLevel { get; set; }
}
```

## 4. Runtime settled-state contract

WebGlLib should support:

```csharp
Task<WebGlRuntimeDiagnostics?> WaitForRuntimeIdleAsync(
    TimeSpan timeout,
    WebGlRuntimeIdleOptions options,
    CancellationToken cancellationToken = default);
```

The JS side should return success only when:

- activeMotionCount == 0
- queuedMotionCount == 0
- queuedCommandStageCount == 0
- no active barrier
- no pending GLB disposal
- render loop is idle or stable according to the selected mode

## 5. Store resolution policy

A store lookup must be explicit:

```text
owner+resource exact
location+resource exact
storeId exact
market/shared-pool policy
explicit fallback disabled in strict mode
```

Multiple matches must be errors unless the scenario defines a tie-breaker.

## 6. Oracle suite

Golden scenarios should include:

- one actor uses own stock,
- shared well depletion,
- insufficient stock strict failure,
- capacity clamp,
- ambiguous store failure,
- transfer between actors,
- tax/fee transfer,
- rule enforcement issue,
- conservation and non-conservation scenarios,
- behavior expansion disabled/enabled comparison.
