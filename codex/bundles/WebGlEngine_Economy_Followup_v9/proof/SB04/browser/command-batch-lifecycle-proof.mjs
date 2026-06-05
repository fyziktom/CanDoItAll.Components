import fs from "node:fs/promises";
import { createRequire } from "node:module";
import path from "node:path";

const repoRoot = process.cwd();
const require = createRequire(import.meta.url);
const playwrightPackage = process.env.PLAYWRIGHT_PACKAGE_ROOT
    ? require(path.join(process.env.PLAYWRIGHT_PACKAGE_ROOT, "node_modules", "playwright"))
    : require("playwright");
const { chromium } = playwrightPackage;
const browserDir = path.join(repoRoot, "codex", "bundles", "WebGlEngine_Economy_Followup_v9", "proof", "SB04", "browser");
const route = process.env.SB04_WEBGL_URL || "http://localhost:5298/run-playback";
const screenshotPath = path.join(browserDir, "command-batch-lifecycle-after.png");
const assertionsPath = path.join(browserDir, "command-batch-lifecycle-assertions.json");
const consolePath = path.join(browserDir, "command-batch-lifecycle-console.log");
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

function splitBlockers(value) {
    return String(value || "")
        .split(",")
        .map(item => item.trim())
        .filter(Boolean);
}

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
            sceneId: "sb04-command-lifecycle-browser-scene",
            title: "SB04 command lifecycle browser proof",
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
            metadata: { proof: "SB04" }
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
            reason: "sb04-browser-initial"
        });

        const scheduledBatch = createBarrierBatch("sb04-scheduled", 1.5, "#0f766e", 0.45);
        const scheduledResult = runtime.applyCommandBatch(host, scheduledBatch);
        const diagnosticsAfterScheduled = runtime.getDiagnostics(host);
        const idleAfterScheduled = await runtime.waitForRuntimeIdle(host, {
            timeoutMs: 3000,
            pollIntervalMs: 16,
            reason: "sb04-scheduled-drain"
        });
        const diagnosticsAfterScheduledDrain = runtime.getDiagnostics(host);

        const settledBatch = createBarrierBatch("sb04-settled", -1.25, "#f97316", 0.35);
        const settledResult = await runtime.applyCommandBatchAndWait(host, settledBatch, {
            timeoutMs: 3000,
            pollIntervalMs: 16,
            reason: "sb04-settled-wait-proof"
        });
        const diagnosticsAfterSettled = runtime.getDiagnostics(host);

        return {
            imported,
            initialIdle,
            scheduledResult,
            diagnosticsAfterScheduled,
            idleAfterScheduled,
            diagnosticsAfterScheduledDrain,
            settledResult,
            diagnosticsAfterSettled
        };

        function createBarrierBatch(batchId, targetX, color, durationSeconds) {
            return {
                batchId,
                stages: [
                    {
                        stageId: `${batchId}:motion`,
                        barrierPolicy: "wait-for-active-motions",
                        motions: [
                            {
                                motionId: `${batchId}:motion:runner`,
                                objectId: "object.runner",
                                targetPosition: { x: targetX, y: 0, z: 0 },
                                durationSeconds,
                                easing: "linear",
                                queuePolicy: "cancel-and-replace"
                            }
                        ]
                    },
                    {
                        stageId: `${batchId}:patch-after-motion`,
                        patches: [
                            {
                                sceneId: scene.sceneId,
                                objectPatches: [
                                    {
                                        objectId: "object.runner",
                                        color,
                                        metadata: { proofBatch: batchId }
                                    }
                                ]
                            }
                        ]
                    }
                ]
            };
        }
    }, hostSelector);

    const scheduledBlockers = splitBlockers(proof.scheduledResult?.diagnostics?.runtimeIdleBlockers ||
        proof.scheduledResult?.metadata?.runtimeIdleBlockers);
    const finalBlockers = motionStageBlockers(proof.diagnosticsAfterSettled);
    const disallowedConsole = consoleMessages.filter(item =>
        item.type === "error" ||
        item.type === "pageerror");
    const assertions = {
        route,
        viewport: "1920x1080",
        imported: proof.imported === true,
        initialIdle: proof.initialIdle?.idle === true,
        nonWaitingResultScheduled: proof.scheduledResult?.success === true &&
            proof.scheduledResult?.lifecycleState === "scheduled" &&
            proof.scheduledResult?.settled === false,
        nonWaitingHasRuntimeBlockers: scheduledBlockers.some(item => item.startsWith("motion:active")) ||
            scheduledBlockers.includes("command-stage:barrier"),
        scheduledDrainIdle: proof.idleAfterScheduled?.idle === true &&
            proof.idleAfterScheduled?.timedOut === false,
        waitingResultSettled: proof.settledResult?.success === true &&
            proof.settledResult?.lifecycleState === "settled" &&
            proof.settledResult?.settled === true,
        waitingRuntimeIdle: proof.settledResult?.metadata?.runtimeIdle === "true" &&
            proof.settledResult?.metadata?.runtimeIdleTimedOut === "false" &&
            splitBlockers(proof.settledResult?.metadata?.runtimeIdleBlockers).length === 0,
        activeQueuedZero: finalBlockers.length === 0,
        screenshotPath,
        consoleMessageCount: consoleMessages.length,
        disallowedConsole
    };

    const output = {
        assertions,
        scheduledBlockers,
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
            throw new Error(`SB04 browser assertion failed: ${name}`);
        }
    }

    if (disallowedConsole.length > 0) {
        throw new Error(`Browser console contained disallowed errors: ${disallowedConsole.map(item => item.text).join(" | ")}`);
    }

    console.log(`SB04 command-batch lifecycle browser proof passed: ${assertionsPath}`);
} finally {
    await browser.close();
}
