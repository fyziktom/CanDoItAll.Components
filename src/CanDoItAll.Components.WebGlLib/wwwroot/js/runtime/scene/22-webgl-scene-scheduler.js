import { hasPendingCommandStageRunnerWork } from "./30-webgl-scene-stage-runner.js";

export function createRenderScheduler(state, renderFrame) {
    return {
        schedule(reason = "invalidated") {
            state.renderRequested = true;
            state.renderReason = reason;
            state.diagnostics.lastScheduledReason = reason;
            if (state.isRenderingFrame || state.animationHandle) {
                return;
            }

            requestNextFrame(state, renderFrame);
        },
        cancel() {
            if (state.animationHandle) {
                cancelAnimationFrame(state.animationHandle);
                state.animationHandle = 0;
            }

            state.diagnostics.isRenderLoopActive = false;
        }
    };
}

export function resolveRenderReason(state) {
    const mode = state.options.renderMode || "auto";
    state.diagnostics.renderSchedulerMode = mode;
    if (mode === "continuous") {
        return "continuous";
    }

    if (state.motions.size > 0) {
        return "motion";
    }

    if (hasPendingCommandStageRunnerWork(state)) {
        return "command-stage";
    }

    if (mode === "auto" && state.diagnostics.animatedSymbolCount > 0) {
        return "symbol-effect";
    }

    if (mode === "auto" && state.cameraDampingFrames > 0) {
        return "camera-damping";
    }

    if (state.renderRequested) {
        return state.renderReason || "invalidated";
    }

    return "";
}

function requestNextFrame(state, renderFrame) {
    state.diagnostics.isRenderLoopActive = true;
    state.diagnostics.idleSinceTimestamp = 0;
    const loop = timestamp => {
        state.animationHandle = 0;
        if (!state.host.__webglSceneState) {
            state.diagnostics.isRenderLoopActive = false;
            return;
        }

        const reason = resolveRenderReason(state);
        if (reason) {
            state.isRenderingFrame = true;
            try {
                renderFrame(timestamp || performance.now(), reason);
            } finally {
                state.isRenderingFrame = false;
            }
        }

        if (resolveRenderReason(state)) {
            state.animationHandle = requestAnimationFrame(loop);
            return;
        }

        state.diagnostics.isRenderLoopActive = false;
        state.diagnostics.idleSinceTimestamp = performance.now();
    };
    state.animationHandle = requestAnimationFrame(loop);
}
