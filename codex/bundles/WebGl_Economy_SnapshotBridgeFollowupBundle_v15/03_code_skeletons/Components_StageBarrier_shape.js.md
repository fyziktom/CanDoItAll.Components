# Code skeleton — JS stage barrier concept

```javascript
// This is intentionally plain JavaScript, not TypeScript.
export function canAdvanceStage(state, stage) {
    const barrier = stage?.metadata?.barrier || "time-delay";
    if (barrier === "wait-for-active-motions") {
        return (state.motions?.size || 0) === 0 && (state.diagnostics?.queuedMotionCount || 0) === 0;
    }

    if (barrier === "wait-for-object-motions") {
        const objectId = stage?.metadata?.objectId || "";
        return !objectId || !hasObjectMotion(state, objectId);
    }

    return (state.commandStageRunner?.waitSeconds || 0) <= 0;
}
```
