# SB03 — Components WebGlRunDocument Runner Contract

## Goal

Define a stable generic runner/controller contract that can apply `WebGlRunDocument` to the WebGL runtime.

## Required actions

Add or harden a generic execution abstraction in `CanDoItAll.Components.WebGlRunLib`:

```text
IWebGlRunDocumentRunner
IWebGlRunFrameApplier
WebGlRunExecutionState
WebGlRunExecutionResult
WebGlRunExecutionDiagnostics
```

The runner must stay domain-neutral and must not know about Economy.

## Required behavior

- Apply initial scene.
- Apply a selected frame by index.
- Apply frame stages in order.
- Preserve traceability from run frame/stage metadata to command batches.
- Return diagnostics for unresolved objects, failed patches, failed motions and skipped stages.

## Acceptance

Components tests must run a synthetic generic run document with two actors and one target object through the runner contract without any Economy dependency.
