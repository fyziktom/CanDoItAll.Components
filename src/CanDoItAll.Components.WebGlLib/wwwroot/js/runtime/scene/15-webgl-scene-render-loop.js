import { syncSymbolAnimation } from "./04-webgl-scene-symbols.js";
import { syncViewport } from "./06-webgl-scene-camera.js";
import { syncOverlays } from "./07-webgl-scene-overlays.js";
import { syncDecorations } from "./11-webgl-scene-graph.js";
import { advanceMotions } from "./14-webgl-scene-motion.js";
import { createRenderScheduler } from "./22-webgl-scene-scheduler.js";

export function attachRenderLoop(state) {
    state.renderScheduler = createRenderScheduler(state, (timestamp, reason) => render(state, timestamp, reason));
    state.scheduleRender = reason => state.renderScheduler.schedule(reason);
    scheduleRender(state, "create");
}

function render(state, timestamp, reason) {
    const start = performance.now();
    const deltaSeconds = state.lastRenderTimestamp
        ? Math.min(0.08, (timestamp - state.lastRenderTimestamp) / 1000)
        : 1 / 60;
    state.diagnostics.lastDeltaSeconds = deltaSeconds;
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
    state.diagnostics.totalFrameTimeMs = (state.diagnostics.totalFrameTimeMs || 0) + state.diagnostics.frameTimeMs;
    state.diagnostics.averageFrameTimeMs = state.diagnostics.totalFrameTimeMs / Math.max(1, state.diagnostics.renderCount);
    state.diagnostics.peakFrameTimeMs = Math.max(state.diagnostics.peakFrameTimeMs || 0, state.diagnostics.frameTimeMs);
    state.frame += 1;
    if (state.cameraDampingFrames > 0) {
        state.cameraDampingFrames -= 1;
    }
}

function scheduleRender(state, reason = "invalidated") {
    state.renderScheduler.schedule(reason);
}
