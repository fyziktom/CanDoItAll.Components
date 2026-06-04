const fs = require("fs");
const path = require("path");
const { chromium } = require("playwright");

const outputPath = path.resolve(__dirname, "staged-batch-settled-proof.json");
const screenshotPath = path.resolve(__dirname, "staged-batch-settled-proof.png");
const baseUrl = process.env.SB03_WEBGL_URL || "http://localhost:5298/run-playback";
const chromePath = process.env.PLAYWRIGHT_CHROME_PATH || "C:\\Program Files\\Google\\Chrome\\Application\\chrome.exe";

function assert(condition, message) {
    if (!condition) {
        throw new Error(message);
    }
}

function createScene() {
    return {
        sceneId: "sb03-command-lifecycle-scene",
        title: "SB03 command lifecycle proof",
        revision: 1,
        objects: [
            {
                id: "object.runner",
                title: "Runner",
                assetId: "",
                color: "#2563eb",
                position: { x: 0, y: 0, z: 0 },
                size: { x: 1, y: 0.7, z: 1 }
            }
        ],
        links: [],
        layers: [],
        metadata: { proof: "SB03" }
    };
}

function createStagedBatch(batchId, targetX, color, durationSeconds) {
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
                        sceneId: "sb03-command-lifecycle-scene",
                        objectPatches: [
                            {
                                objectId: "object.runner",
                                color,
                                metadata: { lifecycleProofPatch: batchId }
                            }
                        ]
                    }
                ]
            }
        ]
    };
}

(async () => {
    const browser = await chromium.launch({
        headless: true,
        executablePath: fs.existsSync(chromePath) ? chromePath : undefined
    });
    const page = await browser.newPage({ viewport: { width: 1440, height: 900 } });

    try {
        await page.goto(baseUrl, { waitUntil: "networkidle" });
        await page.waitForSelector("[data-testid='webgl-scene-host']", { timeout: 30000 });
        await page.waitForFunction(() => !!window.CanDoItAll?.webglScene?.applyCommandBatchAndWait, null, { timeout: 30000 });
        await page.waitForFunction(() => {
            const host = document.querySelector("[data-testid='webgl-scene-host']");
            return !!host && !!window.CanDoItAll?.webglScene?.getDiagnostics(host);
        }, null, { timeout: 30000 });

        const proof = await page.evaluate(async ({ scene }) => {
            const host = document.querySelector("[data-testid='webgl-scene-host']");
            const runtime = window.CanDoItAll.webglScene;
            const started = runtime.importScene(host, scene, {
                deterministicMode: true,
                renderMode: "auto",
                showLabels: false,
                showDiagnosticsPanel: true
            });

            const scheduledBatch = createStagedBatchBrowser("sb03-scheduled", 2, "#0f766e", 0.75);
            const scheduledResult = runtime.applyCommandBatch(host, scheduledBatch);
            const diagnosticsAfterScheduled = runtime.getDiagnostics(host);
            const idleAfterScheduled = await runtime.waitForRuntimeIdle(host, {
                timeoutMs: 4000,
                pollIntervalMs: 16,
                reason: "sb03-scheduled-drain"
            });

            const settledBatch = createStagedBatchBrowser("sb03-settled", -1.5, "#f97316", 0.45);
            const settledResult = await runtime.applyCommandBatchAndWait(host, settledBatch, {
                timeoutMs: 4000,
                pollIntervalMs: 16,
                reason: "sb03-settled-proof"
            });
            const diagnosticsAfterSettled = runtime.getDiagnostics(host);

            return {
                started,
                scheduledResult,
                diagnosticsAfterScheduled,
                idleAfterScheduled,
                settledResult,
                diagnosticsAfterSettled,
                assertions: {
                    normalApplyScheduled:
                        scheduledResult?.success === true &&
                        scheduledResult?.lifecycleState === "scheduled" &&
                        scheduledResult?.settled === false,
                    normalApplyHasRuntimeWork:
                        Number(scheduledResult?.diagnostics?.activeMotionCount || 0) > 0 ||
                        Number(scheduledResult?.diagnostics?.queuedCommandStageCount || 0) > 0 ||
                        String(scheduledResult?.diagnostics?.commandStageBarrierPolicy || "").length > 0,
                    scheduledDrainIdle:
                        idleAfterScheduled?.idle === true &&
                        idleAfterScheduled?.timedOut === false,
                    waitApplySettled:
                        settledResult?.success === true &&
                        settledResult?.lifecycleState === "settled" &&
                        settledResult?.settled === true,
                    waitApplyRuntimeIdle:
                        settledResult?.metadata?.runtimeIdle === "true" &&
                        settledResult?.metadata?.runtimeIdleTimedOut === "false" &&
                        (settledResult?.metadata?.runtimeIdleBlockers || "") === "",
                    finalDiagnosticsIdle:
                        Number(diagnosticsAfterSettled?.activeMotionCount || 0) === 0 &&
                        Number(diagnosticsAfterSettled?.queuedMotionCount || 0) === 0 &&
                        Number(diagnosticsAfterSettled?.queuedCommandStageCount || 0) === 0 &&
                        !diagnosticsAfterSettled?.commandStageBarrierPolicy
                }
            };

            function createStagedBatchBrowser(batchId, targetX, color, durationSeconds) {
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
                                    sceneId: "sb03-command-lifecycle-scene",
                                    objectPatches: [
                                        {
                                            objectId: "object.runner",
                                            color,
                                            metadata: { lifecycleProofPatch: batchId }
                                        }
                                    ]
                                }
                            ]
                        }
                    ]
                };
            }
        }, { scene: createScene() });

        await page.screenshot({ path: screenshotPath, fullPage: true });
        fs.writeFileSync(outputPath, JSON.stringify(proof, null, 2));

        assert(proof.started === true, "Scene import did not succeed.");
        for (const [name, passed] of Object.entries(proof.assertions)) {
            assert(passed, `Assertion failed: ${name}`);
        }

        console.log(JSON.stringify({
            outputPath,
            screenshotPath,
            scheduledLifecycle: proof.scheduledResult?.lifecycleState,
            settledLifecycle: proof.settledResult?.lifecycleState,
            idleElapsedMs: proof.settledResult?.metadata?.runtimeIdleElapsedMs
        }, null, 2));
    } finally {
        await browser.close();
    }
})();
