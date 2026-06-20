import fs from "node:fs";
import path from "node:path";
import { fileURLToPath } from "node:url";
import {
    buildRuntimeIdleState,
    shouldTreatFinalScheduledRenderAsDrained,
    syncRuntimeIdleDiagnostics
} from "../../../../../../src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/41-webgl-scene-runtime-idle-state.js";

const __filename = fileURLToPath(import.meta.url);
const __dirname = path.dirname(__filename);
const outputPath = path.join(__dirname, "runtime-idle-final-render-assertions.json");

function assert(condition, message) {
    if (!condition) {
        throw new Error(message);
    }
}

function createState(overrides = {}) {
    return {
        sceneModel: {
            sceneId: "sb03-runtime-idle-proof",
            objects: [],
            links: [],
            uiState: {},
            metadata: {}
        },
        options: {
            deterministicMode: true,
            renderMode: "auto",
            runtimeBudget: {}
        },
        assetCache: { mode: "state-local" },
        motions: new Map(),
        motionQueuesByObjectId: new Map(),
        commandStageRunner: null,
        symbolGroups: new Map(),
        animationHandle: 0,
        isRenderingFrame: false,
        diagnostics: createDiagnostics(),
        ...overrides
    };
}

function createDiagnostics(overrides = {}) {
    return {
        loadedAssetIds: new Set(),
        missingAssetIds: new Set(),
        fallbackObjectIds: new Set(),
        modelInstanceIds: new Set(),
        primitiveInstanceIds: new Set(),
        failedAssetUris: new Set(),
        missingFallbackAssetIds: new Set(),
        failedPatchCommands: new Set(),
        failedCommandDetails: [],
        modelDiagnostics: new Map(),
        visibilityCounts: {
            visibleObjectCount: 0,
            hiddenObjectCount: 0,
            visibleLinkCount: 0,
            hiddenLinkCount: 0
        },
        assetCachePendingDisposalCount: 0,
        queuedMotionCount: 0,
        isRenderLoopActive: false,
        finalRenderDrained: false,
        ...overrides
    };
}

function run() {
    const scheduledOnlyState = createState({
        animationHandle: 42,
        diagnostics: createDiagnostics({
            isRenderLoopActive: true,
            lastScheduledReason: "final-paint"
        })
    });
    const beforeScheduledOnly = buildRuntimeIdleState(scheduledOnlyState);
    const firstProbeDrain = shouldTreatFinalScheduledRenderAsDrained(beforeScheduledOnly, 1);
    const secondProbeDrain = shouldTreatFinalScheduledRenderAsDrained(beforeScheduledOnly, 2);
    scheduledOnlyState.diagnostics.finalRenderDrained = secondProbeDrain;
    const scheduledOnlyResult = buildRuntimeIdleState(scheduledOnlyState);
    syncRuntimeIdleDiagnostics(scheduledOnlyState, scheduledOnlyResult);

    assert(beforeScheduledOnly.semanticIdle === true, "scheduled-only state should be semantically idle before wait");
    assert(beforeScheduledOnly.visualIdle === false, "scheduled-only state should not be visually idle before final drain");
    assert(beforeScheduledOnly.visualBlockers.includes("render-loop:scheduled"), "scheduled-only state should expose scheduled render as a visual blocker");
    assert(firstProbeDrain === false, "first semantic-idle probe should not drain the final scheduled render");
    assert(secondProbeDrain === true, "second consecutive semantic-idle probe should drain the final scheduled render");
    assert(scheduledOnlyResult.idle === true, "scheduled-only wait should report aggregate idle");
    assert(scheduledOnlyResult.semanticIdle === true, "scheduled-only wait should preserve semantic idle");
    assert(scheduledOnlyResult.visualIdle === true, "scheduled-only wait should drain visual idle after two probes");
    assert(scheduledOnlyResult.finalRenderDrained === true, "scheduled-only wait should mark final render drained");
    assert(scheduledOnlyResult.blockers.length === 0, "scheduled-only wait should have no aggregate blockers after drain");

    const activeMotionState = createState({
        animationHandle: 43,
        motions: new Map([["motion.active", { motionId: "motion.active" }]]),
        diagnostics: createDiagnostics({
            isRenderLoopActive: true,
            lastScheduledReason: "motion"
        })
    });
    const activeMotionResult = buildRuntimeIdleState(activeMotionState);
    const activeMotionDrain = shouldTreatFinalScheduledRenderAsDrained(activeMotionResult, 2);

    assert(activeMotionResult.idle === false, "active motion should still block idle");
    assert(activeMotionResult.semanticIdle === false, "active motion should not be semantically idle");
    assert(activeMotionResult.blockers.includes("motion:active:1"), "active motion should remain a semantic blocker");
    assert(activeMotionDrain === false, "active motion must not satisfy final render drain");
    assert(activeMotionResult.finalRenderDrained === false, "active motion must not be hidden by final render drain");

    const continuousRenderState = createState({
        options: {
            deterministicMode: true,
            renderMode: "continuous",
            runtimeBudget: {}
        }
    });
    const continuousRenderResult = buildRuntimeIdleState(continuousRenderState);
    const continuousRenderDrain = shouldTreatFinalScheduledRenderAsDrained(continuousRenderResult, 2);

    assert(continuousRenderResult.idle === false, "continuous render mode should still block idle");
    assert(continuousRenderResult.semanticIdle === true, "continuous render mode can be semantically idle");
    assert(continuousRenderResult.visualIdle === false, "continuous render mode should not be visually idle");
    assert(continuousRenderResult.visualBlockers.includes("render-loop:continuous-mode"), "continuous render mode should remain a visual blocker");
    assert(continuousRenderDrain === false, "continuous render mode must not satisfy scheduled-only final drain");

    const proof = {
        proof: "SB03 runtime idle final scheduled render",
        generatedAt: new Date().toISOString(),
        assertions: {
            scheduledOnlyWasSemanticIdleBeforeWait: beforeScheduledOnly.semanticIdle === true,
            scheduledOnlyWasVisualBlockedBeforeWait: beforeScheduledOnly.visualIdle === false &&
                beforeScheduledOnly.visualBlockers.includes("render-loop:scheduled"),
            scheduledOnlyDrainedAfterTwoProbes: firstProbeDrain === false &&
                secondProbeDrain === true &&
                scheduledOnlyResult.idle === true &&
                scheduledOnlyResult.finalRenderDrained === true &&
                scheduledOnlyResult.blockers.length === 0,
            activeMotionStillBlocksSemanticIdle: activeMotionResult.idle === false &&
                activeMotionResult.blockers.includes("motion:active:1") &&
                activeMotionDrain === false,
            continuousRenderStillBlocksVisualIdle: continuousRenderResult.idle === false &&
                continuousRenderResult.visualBlockers.includes("render-loop:continuous-mode") &&
                continuousRenderDrain === false
        },
        beforeScheduledOnly,
        firstProbeDrain,
        secondProbeDrain,
        scheduledOnlyResult,
        syncedDiagnostics: {
            semanticIdle: scheduledOnlyState.diagnostics.semanticIdle,
            visualIdle: scheduledOnlyState.diagnostics.visualIdle,
            finalRenderDrained: scheduledOnlyState.diagnostics.finalRenderDrained
        },
        activeMotionResult,
        activeMotionDrain,
        continuousRenderResult
    };

    fs.writeFileSync(outputPath, `${JSON.stringify(proof, null, 2)}\n`, "utf8");
    console.log(`SB03 JS runtime idle proof passed: ${outputPath}`);
}

run();
