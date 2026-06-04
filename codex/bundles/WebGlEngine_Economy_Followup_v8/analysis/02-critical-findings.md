# Critical findings and likely noise sources

## F01 Pause/stop ordering still risks visible motion after Pause

The code now calls runtime stop, but only after waiting for the playback task. If a running frame is inside a browser apply/idle wait path, the scene can visibly continue until the wait times out. Fix: stop browser runtime immediately, then await/drain the C# task.

## F02 Runtime idle is proof-critical, but API semantics are inconsistent

`applyCommandBatchAndWait` annotates idle status. Browser adapter treats idle timeout as failure in some paths. Codex must ensure every proof path that requests settled semantics converts idle timeout into a hard failure.

## F03 Boolean-only evidence can still inflate readiness

Readiness flags like `BrowserRuntimeExercised`, `UIExercised`, and `OracleProofExercised` are not enough. They must point to artifacts with hashes, route, console error proof, idle diagnostics, and oracle corpus id.

## F04 Design matrix factors may be passive labels

The design harness records factor levels and configuration hashes, but current run invocation still calls the same catalog scenario. If factor levels do not mutate scenario input or runtime configuration, comparisons are invalid.

## F05 Golden oracles are still too code-local

Inline oracle cases are valuable, but research-grade oracle expected values should live in JSON files. Otherwise a refactor could accidentally update both logic and expected values in the same code path.

## F06 Diagnostic classification can miss unknown messages

Known diagnostic codes are classified. Unknown backend/input diagnostics must become hard "unclassified diagnostic" findings, not disappear into an unrelated band or be ignored.

## F07 Store resolution and rejected-flow semantics need wider adversarial coverage

Strict mode catches ambiguity, capacity rejection, zero-accepted transfer, and insufficient stock in some cases. It needs combinatorial tests for source/target/shared/effect roles, multiple resources, market pools, and explicit vs implicit policies.

## F08 Metric/invariant registry protects definitions, but evaluator behavior still needs no-fallback proof

Unknown metric/invariant kinds are validated, but evaluator calls should also fail loudly when invoked directly in strict/research context.

## F09 Behavior expansion profile changes must be diffable

Profile id/version/hash are now carried. Need lockfile/diff tooling that says whether a run changed because of economic inputs or because behavior expansion semantics changed.

## F10 Browser observer should be deterministic proof, not a demo screenshot

Browser observer proof must compare headless run document hash against browser-loaded document hash and assert idle/stage/motion counters, not just screenshots or status labels.
