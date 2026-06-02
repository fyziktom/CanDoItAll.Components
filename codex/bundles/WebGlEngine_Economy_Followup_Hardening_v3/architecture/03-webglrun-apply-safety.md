# WebGlRun Apply Safety Architecture

## Current risk

`WebGlRunDocumentValidator` rejects frames that mix direct frame-level commands and stages, but `WebGlRunFrameApplyResult.FromFrame` still drops frame-level commands when stages exist.

## Target

- `FromFrame` must either:
  1. return a `WebGlRunFrameApplyResult` with an error and no command batch, or
  2. require a strict option and throw a documented exception.

Preferred contract:

```csharp
public sealed class WebGlRunFrameApplyOptions
{
    public bool FailOnMixedDirectAndStagedCommands { get; set; } = true;
    public bool PreserveValidationErrorsInResult { get; set; } = true;
}
```

- Browser apply adapter must not continue after required reset failure.
- Runtime reset should import the full scene document or explicitly merge scene document runtime options with adapter runtime options.
- Final proof must include direct API tests that do not call the validator first.
