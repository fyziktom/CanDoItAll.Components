import assert from "node:assert/strict";
import { buildRuntimeIdleState } from "../../src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/41-webgl-scene-runtime-idle-state.js";

function createScheduledRenderOnlyState() {
    return {
        diagnostics: {},
        motions: new Map(),
        motionQueuesByObjectId: new Map(),
        commandStageRunner: {
            queue: [],
            activeBarrier: null,
            currentStageId: "",
            cancelled: false
        },
        animationHandle: 42,
        isRenderingFrame: false,
        options: {
            renderMode: "auto"
        }
    };
}

const semanticOnly = buildRuntimeIdleState(createScheduledRenderOnlyState(), {
    policyMode: "semanticOnly"
});
assert.equal(semanticOnly.policyMode, "semanticOnly");
assert.equal(semanticOnly.semanticIdle, true);
assert.equal(semanticOnly.visualIdle, false);
assert.equal(semanticOnly.idle, true);
assert.deepEqual(semanticOnly.blockers, []);
assert.deepEqual(semanticOnly.visualBlockers, ["render-loop:scheduled"]);

const visualStrict = buildRuntimeIdleState(createScheduledRenderOnlyState(), {
    policyMode: "visualStrict"
});
assert.equal(visualStrict.policyMode, "visualStrict");
assert.equal(visualStrict.semanticIdle, true);
assert.equal(visualStrict.visualIdle, false);
assert.equal(visualStrict.idle, false);
assert.equal(visualStrict.finalRenderDrainAllowed, false);
assert.deepEqual(visualStrict.blockers, ["render-loop:scheduled"]);

const finalDrainState = createScheduledRenderOnlyState();
finalDrainState.diagnostics.finalRenderDrained = true;
const allowFinalDrain = buildRuntimeIdleState(finalDrainState, {
    policyMode: "allowFinalRenderDrain"
});
assert.equal(allowFinalDrain.policyMode, "allowFinalRenderDrain");
assert.equal(allowFinalDrain.semanticIdle, true);
assert.equal(allowFinalDrain.visualIdle, true);
assert.equal(allowFinalDrain.idle, true);
assert.equal(allowFinalDrain.finalRenderDrainAllowed, true);
assert.deepEqual(allowFinalDrain.blockers, []);
assert.deepEqual(allowFinalDrain.visualBlockers, []);
assert.deepEqual(allowFinalDrain.rawVisualBlockers, ["render-loop:scheduled"]);

const unknownPolicy = buildRuntimeIdleState(createScheduledRenderOnlyState(), {
    policyMode: "not-a-policy"
});
assert.equal(unknownPolicy.policyMode, "allowFinalRenderDrain");

console.log("Runtime idle policy tests passed.");
