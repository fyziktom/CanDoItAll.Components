# Stage runner diagnostics shape

```javascript
export function snapshotStageRunner(state) {
    const runner = state.commandStageRunner || {};
    return {
        currentBatchId: runner.currentBatchId || "",
        currentStageId: runner.currentStageId || "",
        queuedCommandStageCount: runner.queue?.length || 0,
        commandStageWaitSeconds: Math.round((Number(runner.waitSeconds) || 0) * 1000) / 1000,
        completedCommandStageCount: runner.completedCount || 0,
        failedCommandStageCount: runner.failedCount || 0,
        cancelled: runner.cancelled === true
    };
}
```
