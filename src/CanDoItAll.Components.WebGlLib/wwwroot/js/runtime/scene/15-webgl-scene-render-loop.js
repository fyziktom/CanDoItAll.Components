import { syncSymbolAnimation } from "./04-webgl-scene-symbols.js";
import { syncViewport } from "./06-webgl-scene-camera.js";
import { syncOverlays } from "./07-webgl-scene-overlays.js";
import { syncDecorations } from "./11-webgl-scene-graph.js";
import { advanceMotions } from "./14-webgl-scene-motion.js";

export function attachRenderLoop(state) {
    state.scheduleRender = reason => scheduleRender(state, reason);
    startRenderLoop(state);
}

function startRenderLoop(state) {
    const loop = timestamp => {
        if (!state.host.__webglSceneState) {
            return;
        }

        const reason = resolveRenderReason(state);
        if (reason) {
            render(state, timestamp || performance.now(), reason);
        }

        state.animationHandle = requestAnimationFrame(loop);
    };
    state.animationHandle = requestAnimationFrame(loop);
}

function render(state, timestamp, reason) {
    const start = performance.now();
    const deltaSeconds = state.lastRenderTimestamp
        ? Math.min(0.08, (timestamp - state.lastRenderTimestamp) / 1000)
        : 1 / 60;
    state.lastRenderTimestamp = timestamp;
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
    state.renderRequested = false;
    state.renderReason = "";
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
}
