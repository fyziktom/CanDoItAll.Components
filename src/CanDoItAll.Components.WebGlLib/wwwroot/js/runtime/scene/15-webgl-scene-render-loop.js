import { syncSymbolAnimation } from "./04-webgl-scene-symbols.js";
import { syncViewport } from "./06-webgl-scene-camera.js";
import { syncOverlays } from "./07-webgl-scene-overlays.js";
import { syncDecorations } from "./11-webgl-scene-graph.js";
import { advanceMotions } from "./14-webgl-scene-motion.js";

export function attachRenderLoop(state) {
    state.scheduleRender = reason => scheduleRender(state, reason);
    scheduleRender(state, "create");
}

function requestNextFrame(state) {
    if (state.animationHandle) {
        return;
    }

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
                render(state, timestamp || performance.now(), reason);
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

function render(state, timestamp, reason) {
    const start = performance.now();
    const deltaSeconds = state.lastRenderTimestamp
        ? Math.min(0.08, (timestamp - state.lastRenderTimestamp) / 1000)
        : 1 / 60;
    state.lastRenderTimestamp = timestamp;
    state.renderRequested = false;
    state.renderReason = "";
    syncViewport(state);
    syncDecorations(state);
    state.controls.update();
    advanceMotions(state, deltaSeconds);
    const elapsedSeconds = state.options.deterministicMode ? state.frame / 60 : timestamp / 1000;
    syncSymbolAnimation(state, elapsedSeconds);
    state.renderer.render(state.scene, state.camera);
    syncOverlays(state);
    state.diagnostics.renderCount += 1;
    state.diagnostics.lastFrameReason = reason;
    state.diagnostics.frameTimeMs = performance.now() - start;
    state.frame += 1;
    if (state.cameraDampingFrames > 0) {
        state.cameraDampingFrames -= 1;
    }
}

function resolveRenderReason(state) {
    const mode = state.options.renderMode || "auto";
    if (mode === "continuous") {
        return "continuous";
    }

    if (state.motions.size > 0) {
        return "motion";
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

function scheduleRender(state, reason = "invalidated") {
    state.renderRequested = true;
    state.renderReason = reason;
    if (state.isRenderingFrame) {
        return;
    }

    requestNextFrame(state);
}
