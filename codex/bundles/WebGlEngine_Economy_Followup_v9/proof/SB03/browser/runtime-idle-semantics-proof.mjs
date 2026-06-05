import fs from "node:fs/promises";
import { createRequire } from "node:module";
import path from "node:path";

const repoRoot = process.cwd();
const require = createRequire(import.meta.url);
const playwrightPackage = process.env.PLAYWRIGHT_PACKAGE_ROOT
    ? require(path.join(process.env.PLAYWRIGHT_PACKAGE_ROOT, "node_modules", "playwright"))
    : require("playwright");
const { chromium } = playwrightPackage;
const browserDir = path.join(repoRoot, "codex", "bundles", "WebGlEngine_Economy_Followup_v9", "proof", "SB03", "browser");
const route = process.env.SB03_WEBGL_URL || "http://localhost:5298/run-playback";
const screenshotPath = path.join(browserDir, "runtime-idle-semantics-after.png");
const assertionsPath = path.join(browserDir, "runtime-idle-semantics-assertions.json");
const consolePath = path.join(browserDir, "runtime-idle-semantics-console.log");
const hostSelector = "[data-testid='webgl-scene-host']";

await fs.mkdir(browserDir, { recursive: true });

const consoleMessages = [];
let browser;
try {
    browser = await chromium.launch({ channel: "chrome", headless: true });
} catch {
    browser = await chromium.launch({ headless: true });
}

const page = await browser.newPage({ viewport: { width: 1920, height: 1080 } });
page.on("console", message => {
    consoleMessages.push({
        type: message.type(),
        text: message.text()
    });
});
page.on("pageerror", error => {
    consoleMessages.push({
        type: "pageerror",
        text: error.message
    });
});

function motionStageBlockers(diagnostics) {
    const blockers = [];
    if ((diagnostics?.activeMotionCount || 0) > 0) {
        blockers.push(`motion:active:${diagnostics.activeMotionCount}`);
    }

    if ((diagnostics?.queuedMotionCount || 0) > 0) {
        blockers.push(`motion:queued:${diagnostics.queuedMotionCount}`);
    }

    if ((diagnostics?.queuedCommandStageCount || 0) > 0) {
        blockers.push(`command-stage:queued:${diagnostics.queuedCommandStageCount}`);
    }

    if (diagnostics?.commandStageBarrierPolicy) {
        blockers.push(`command-stage:barrier:${diagnostics.commandStageBarrierPolicy}`);
    }

    return blockers;
}

try {
    await page.goto(route, { waitUntil: "domcontentloaded" });
    await page.waitForSelector(hostSelector, { timeout: 15000 });
    await page.waitForFunction(selector => {
        const host = document.querySelector(selector);
        return !!window.CanDoItAll?.webglScene?.applyCommandBatchAndWait && !!host?.__webglSceneState;
    }, hostSelector, { timeout: 15000 });

    const proof = await page.evaluate(async selector => {
        const host = document.querySelector(selector);
        const runtime = window.CanDoItAll.webglScene;
        const scene = {
            sceneId: "sb03-runtime-idle-browser-scene",
            title: "SB03 runtime idle browser proof",
            revision: 1,
            objects: [
                {
                    id: "object.runner",
                    title: "Runner",
                    color: "#2563eb",
                    position: { x: 0, y: 0, z: 0 },
                    size: { x: 1, y: 0.7, z: 1 }
                }
            ],
            links: [],
            layers: [],
            metadata: { proof: "SB03" }
        };

        const imported = runtime.importScene(host, scene, {
            deterministicMode: true,
            renderMode: "auto",
            showLabels: false,
            showDiagnosticsPanel: true
        });
        const initialIdle = await runtime.waitForRuntimeIdle(host, {
            timeoutMs: 2000,
            pollIntervalMs: 16,
            reason: "sb03-browser-initial"
        });

        const batch = {
            batchId: "sb03-runtime-idle-browser-batch",
            stages: [
                {
                    stageId: "sb03-stage-motion",
                    barrierPolicy: "wait-for-active-motions",
                    motions: [
                        {
                            motionId: "sb03-motion-runner",
                            objectId: "object.runner",
                            targetPosition: { x: 1.25, y: 0, z: 0 },
                            durationSeconds: 0.2,
                            easing: "linear",
                            queuePolicy: "cancel-and-replace"
                        }
                    ]
                },
                {
                    stageId: "sb03-stage-final-color",
                    patches: [
                        {
                            sceneId: scene.sceneId,
                            objectPatches: [
                                {
                                    objectId: "object.runner",
                                    color: "#0f766e",
                                    metadata: { proofStage: "final-color" }
                                }
                            ]
                        }
                    ]
                }
            ]
        };
        const batchResult = await runtime.applyCommandBatchAndWait(host, batch, {
            timeoutMs: 4000,
            pollIntervalMs: 16,
            reason: "sb03-browser-batch-settled"
        });
        const diagnosticsAfterBatch = runtime.getDiagnostics(host);

        const finalPatchResult = runtime.applyPatchDetailed(host, {
            sceneId: scene.sceneId,
            objectPatches: [
                {
                    objectId: "object.runner",
                    color: "#f97316",
                    metadata: { proofStage: "final-render-drain" }
                }
            ]
        });
        const diagnosticsBeforeFinalDrain = runtime.getDiagnostics(host);
        const finalRenderIdle = await runtime.waitForRuntimeIdle(host, {
            timeoutMs: 1000,
            pollIntervalMs: 16,
            reason: "sb03-browser-final-render-drain"
        });
        const diagnosticsAfterFinalDrain = runtime.getDiagnostics(host);

        return {
            imported,
            initialIdle,
            batchResult,
            diagnosticsAfterBatch,
            finalPatchResult,
            diagnosticsBeforeFinalDrain,
            finalRenderIdle,
            diagnosticsAfterFinalDrain
        };
    }, hostSelector);

    const finalBlockers = motionStageBlockers(proof.diagnosticsAfterFinalDrain);
    const disallowedConsole = consoleMessages.filter(item =>
        item.type === "error" ||
        item.type === "pageerror");
    const assertions = {
        route,
        viewport: "1920x1080",
        imported: proof.imported === true,
        initialIdle: proof.initialIdle?.idle === true,
        batchSettled: proof.batchResult?.success === true &&
            proof.batchResult?.lifecycleState === "settled" &&
            proof.batchResult?.settled === true,
        batchIdleSemantics: proof.batchResult?.metadata?.runtimeIdle === "true" &&
            proof.batchResult?.metadata?.runtimeIdleTimedOut === "false",
        finalRenderIdle: proof.finalRenderIdle?.success === true &&
            proof.finalRenderIdle?.idle === true &&
            proof.finalRenderIdle?.semanticIdle === true &&
            proof.finalRenderIdle?.visualIdle === true &&
            proof.finalRenderIdle?.finalRenderDrained === true,
        finalDiagnosticsIdle: proof.diagnosticsAfterFinalDrain?.semanticIdle === true &&
            proof.diagnosticsAfterFinalDrain?.visualIdle === true &&
            proof.diagnosticsAfterFinalDrain?.finalRenderDrained === true,
        activeQueuedZero: finalBlockers.length === 0,
        screenshotPath,
        consoleMessageCount: consoleMessages.length,
        disallowedConsole
    };

    const output = {
        assertions,
        finalBlockers,
        proof
    };

    await page.screenshot({ path: screenshotPath, fullPage: true });
    await fs.writeFile(assertionsPath, `${JSON.stringify(output, null, 2)}\n`);
    const consoleText = consoleMessages.length === 0
        ? "(no browser console messages)\n"
        : `${consoleMessages.map(item => `[${item.type}] ${item.text}`).join("\n")}\n`;
    await fs.writeFile(consolePath, consoleText);

    for (const [name, passed] of Object.entries(assertions)) {
        if (typeof passed === "boolean" && !passed) {
            throw new Error(`SB03 browser assertion failed: ${name}`);
        }
    }

    if (disallowedConsole.length > 0) {
        throw new Error(`Browser console contained disallowed errors: ${disallowedConsole.map(item => item.text).join(" | ")}`);
    }

    console.log(`SB03 browser runtime idle proof passed: ${assertionsPath}`);
} finally {
    await browser.close();
}
