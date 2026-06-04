const fs = require("fs");
const path = require("path");
const { chromium } = require("playwright");

const outputPath = path.resolve(__dirname, "observer-boundary-proof.json");
const screenshotPath = path.resolve(__dirname, "observer-boundary-proof.png");
const url = process.env.SB13_WEBGL_URL || "http://127.0.0.1:5298/run-playback";
const chromePath = process.env.PLAYWRIGHT_CHROME_PATH || "C:\\Program Files\\Google\\Chrome\\Application\\chrome.exe";

function assert(condition, message) {
    if (!condition) {
        throw new Error(message);
    }
}

async function readDiagnosticsJson(page) {
    const text = await page.locator('[data-testid="webgl-run-diagnostics-json"]').textContent();
    return JSON.parse(text || "{}");
}

async function waitForObserver(page, label) {
    await page.waitForFunction(() => {
        const node = document.querySelector('[data-testid="webgl-run-diagnostics-json"]');
        if (!node?.textContent) {
            return false;
        }

        const diagnostics = JSON.parse(node.textContent);
        return diagnostics?.observer?.observerProofValid === true &&
            diagnostics?.observer?.claimStatus === "observer-valid" &&
            diagnostics?.observer?.documentHashesMatch === true &&
            diagnostics?.observer?.expectedDocumentHash &&
            diagnostics?.observer?.browserLoadedDocumentHash;
    }, null, { timeout: 30000 });

    const diagnostics = await readDiagnosticsJson(page);
    return {
        label,
        status: {
            frame: (await page.locator('[data-testid="webgl-run-frame"]').textContent() || "").trim(),
            playing: (await page.locator('[data-testid="webgl-run-playing"]').textContent() || "").trim(),
            status: (await page.locator('[data-testid="webgl-run-status"]').textContent() || "").trim()
        },
        observer: diagnostics.observer,
        proofSnapshot: diagnostics.proofSnapshot,
        runSnapshot: diagnostics.runSnapshot,
        batch: diagnostics.batch,
        idle: diagnostics.idle
    };
}

async function waitForRuntimeIdle(page, reason) {
    return page.evaluate(async idleReason => {
        const host = document.querySelector('[data-testid="webgl-scene-host"]');
        if (!host || !window.CanDoItAll?.webglScene) {
            return { available: false, idle: null, diagnostics: null };
        }

        const idle = await window.CanDoItAll.webglScene.waitForRuntimeIdle(host, {
            timeoutMs: 4000,
            pollIntervalMs: 16,
            reason: idleReason
        });
        const diagnostics = window.CanDoItAll.webglScene.getDiagnostics(host);
        return { available: true, idle, diagnostics };
    }, reason);
}

(async () => {
    const browser = await chromium.launch({
        headless: true,
        executablePath: fs.existsSync(chromePath) ? chromePath : undefined
    });
    const page = await browser.newPage({ viewport: { width: 1440, height: 900 } });
    const consoleMessages = [];
    const pageErrors = [];
    page.on("console", message => consoleMessages.push({ type: message.type(), text: message.text() }));
    page.on("pageerror", error => pageErrors.push(error.message));

    try {
        await page.goto(url, { waitUntil: "networkidle" });
        await page.locator('[data-testid="webgl-scene-host"]').waitFor({ state: "visible", timeout: 30000 });
        await page.waitForFunction(() => !!window.CanDoItAll?.webglScene?.getDiagnostics, null, { timeout: 30000 });

        await page.getByRole("button", { name: "Snapshot" }).click();
        const initialObserver = await waitForObserver(page, "initial-snapshot");

        await page.getByRole("button", { name: "Step" }).click();
        await page.waitForFunction(() => {
            const frame = document.querySelector('[data-testid="webgl-run-frame"]')?.textContent?.trim();
            return frame === "1";
        }, null, { timeout: 30000 });
        const browserIdleAfterStep = await waitForRuntimeIdle(page, "sb13-observer-boundary-after-step");
        await page.getByRole("button", { name: "Snapshot" }).click();
        const afterVisualMutation = await waitForObserver(page, "after-visual-frame");

        await page.screenshot({ path: screenshotPath, fullPage: true });

        const artifact = {
            schemaVersion: "webgl-run-observer-boundary-browser-proof/v1",
            route: url,
            viewport: { width: 1440, height: 900 },
            actions: [
                "open-run-playback",
                "capture-initial-snapshot",
                "apply-one-visual-frame",
                "wait-runtime-idle",
                "capture-post-visual-snapshot"
            ],
            initialObserver,
            afterVisualMutation,
            browserIdleAfterStep,
            interpretation: {
                browserProofRole: "observer-only",
                economicTruthSource: "headless/oracle artifacts",
                visualFailurePolicy: "blocks browser-observer-valid and visual demo claims without mutating headless artifacts"
            },
            assertions: {
                initialObserverValid:
                    initialObserver.observer?.schemaVersion === "webgl-run-observer-proof/v1" &&
                    initialObserver.observer?.observerProofValid === true &&
                    initialObserver.observer?.claimStatus === "observer-valid",
                initialDocumentHashesMatch:
                    initialObserver.observer?.documentHashesMatch === true &&
                    initialObserver.observer?.expectedDocumentHash === initialObserver.observer?.browserLoadedDocumentHash,
                initialRuntimeAndUiExercised:
                    initialObserver.observer?.browserRuntimeValid === true &&
                    initialObserver.observer?.uiValid === true &&
                    initialObserver.observer?.metadata?.runtimeDiagnosticsCaptured === "True" &&
                    initialObserver.observer?.metadata?.proofSnapshotCaptured === "True",
                afterVisualObserverValid:
                    afterVisualMutation.observer?.observerProofValid === true &&
                    afterVisualMutation.observer?.claimStatus === "observer-valid",
                afterVisualDocumentHashesMatch:
                    afterVisualMutation.observer?.documentHashesMatch === true &&
                    afterVisualMutation.observer?.expectedDocumentHash === afterVisualMutation.observer?.browserLoadedDocumentHash,
                visualActionsDidNotChangeExpectedDocumentHash:
                    initialObserver.observer?.expectedDocumentHash === afterVisualMutation.observer?.expectedDocumentHash,
                visualActionsDidNotChangeBrowserLoadedDocumentHash:
                    initialObserver.observer?.browserLoadedDocumentHash === afterVisualMutation.observer?.browserLoadedDocumentHash,
                browserRuntimeSettledAfterVisualAction:
                    browserIdleAfterStep.available === true &&
                    browserIdleAfterStep.idle?.idle === true &&
                    browserIdleAfterStep.idle?.timedOut === false,
                observerCarriesNoEconomicReadinessClaim:
                    !Object.prototype.hasOwnProperty.call(initialObserver.observer || {}, "runHash") &&
                    !Object.prototype.hasOwnProperty.call(initialObserver.observer || {}, "scenarioPackHash") &&
                    !Object.prototype.hasOwnProperty.call(initialObserver.observer || {}, "economicStatus")
            },
            consoleMessages,
            pageErrors
        };

        fs.writeFileSync(outputPath, `${JSON.stringify(artifact, null, 2)}\n`, "utf8");

        for (const [name, passed] of Object.entries(artifact.assertions)) {
            assert(passed, `Assertion failed: ${name}`);
        }
        assert(pageErrors.length === 0, `Page errors were captured: ${pageErrors.join("; ")}`);

        console.log(JSON.stringify({
            outputPath,
            screenshotPath,
            passedAssertions: Object.keys(artifact.assertions),
            expectedDocumentHash: artifact.initialObserver.observer?.expectedDocumentHash,
            afterExpectedDocumentHash: artifact.afterVisualMutation.observer?.expectedDocumentHash
        }, null, 2));
    } finally {
        await browser.close();
    }
})().catch(error => {
    console.error(error);
    process.exit(1);
});
