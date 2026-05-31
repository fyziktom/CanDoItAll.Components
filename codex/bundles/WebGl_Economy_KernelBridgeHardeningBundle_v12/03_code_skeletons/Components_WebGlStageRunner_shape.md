# Components WebGL Stage Runner Shape

```javascript
export function createStageRunner(state, applyStage) {
    const queue = [];
    let running = false;
    let cancelled = false;

    return {
        enqueue(batchId, stages) {
            queue.push({ batchId, stages: [...stages] });
            if (!running) {
                void run();
            }
        },
        cancel(reason = "cancelled") {
            cancelled = true;
            queue.length = 0;
            state.diagnostics.lastStageCancelReason = reason;
        }
    };

    async function run() {
        running = true;
        try {
            while (queue.length && !cancelled) {
                const item = queue.shift();
                for (const stage of item.stages) {
                    if (cancelled) break;
                    await applyStage(stage);
                    if (Number(stage.waitSeconds) > 0) {
                        await delaySeconds(stage.waitSeconds);
                    }
                }
            }
        } finally {
            running = false;
            cancelled = false;
        }
    }
}
```
