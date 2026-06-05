import fs from "node:fs/promises";
import { createRequire } from "node:module";
import path from "node:path";

const repoRoot = process.cwd();
const require = createRequire(import.meta.url);
const playwrightPackage = process.env.PLAYWRIGHT_PACKAGE_ROOT
    ? require(path.join(process.env.PLAYWRIGHT_PACKAGE_ROOT, "node_modules", "playwright"))
    : require("playwright");
const { chromium } = playwrightPackage;
const browserDir = path.join(repoRoot, "codex", "bundles", "WebGlEngine_Economy_Followup_v9", "proof", "SB12", "browser");
const route = process.env.SB12_WEBGL_URL || "http://localhost:5298/run-playback";
const runDocumentPath = path.join(
    repoRoot,
    "codex",
    "bundles",
    "WebGlEngine_Economy_Followup_v9",
    "proof",
    "SB12",
    "economy-real-run",
    "multi-goods-elite",
    "webgl.run-document.json");
const screenshotPath = path.join(browserDir, "multi-goods-browser-after.png");
const observerProofPath = path.join(browserDir, "multi-goods-observer-proof.json");
const assertionsPath = path.join(browserDir, "multi-goods-browser-assertions.json");
const consolePath = path.join(browserDir, "multi-goods-browser-console.log");
const progressPath = path.join(browserDir, "multi-goods-browser-progress.log");
const diagnosticsSelector = "[data-testid='webgl-run-diagnostics-json']";
const forbiddenExecutableTerms = ["buyer", "seller", "investor", "elite"];

await fs.mkdir(browserDir, { recursive: true });
await fs.writeFile(progressPath, "");

async function writeProgress(message) {
    await fs.appendFile(progressPath, `${new Date().toISOString()} ${message}\n`);
}

function vectorMatches(expected, actual) {
    return near(expected?.x, actual?.x) &&
        near(expected?.y, actual?.y) &&
        near(expected?.z, actual?.z);
}

function near(expected, actual) {
    return Math.abs(Number(expected) - Number(actual)) <= 0.001;
}

function positionsMatch(expected, actual) {
    if (!expected || !actual) {
        return false;
    }

    const expectedKeys = Object.keys(expected).sort();
    const actualKeys = Object.keys(actual).sort();
    if (expectedKeys.length !== actualKeys.length ||
        expectedKeys.some((key, index) => key !== actualKeys[index])) {
        return false;
    }

    return expectedKeys.every(key => vectorMatches(expected[key], actual[key]));
}

function containsForbiddenTerm(value) {
    const text = String(value || "").toLowerCase();
    return forbiddenExecutableTerms.some(term => text.includes(term));
}

function collectGenericityFailures(document) {
    const failures = [];
    for (const item of document?.initialScene?.scene?.objects || []) {
        if (containsForbiddenTerm(item.kind)) {
            failures.push(`object:${item.id}:kind:${item.kind}`);
        }

        if (containsForbiddenTerm(item.family)) {
            failures.push(`object:${item.id}:family:${item.family}`);
        }
    }

    for (const frame of document?.timeline?.frames || []) {
        for (const stage of frame.stages || []) {
            if (!String(stage.stageId || "").startsWith("visual-action.")) {
                failures.push(`stage:${stage.stageId}:not-generic-id`);
            }

            if (containsForbiddenTerm(stage.stageId)) {
                failures.push(`stage:${stage.stageId}:domain-term`);
            }
        }
    }

    return failures;
}

async function readDiagnostics(page) {
    const text = await page.locator(diagnosticsSelector).textContent();
    return JSON.parse(text || "{}");
}

const runDocumentJson = await fs.readFile(runDocumentPath, "utf8");
const runDocument = JSON.parse(runDocumentJson);
const expectedRunId = runDocument?.runId?.value || "";
const expectedMaxFrame = Math.max(...(runDocument?.timeline?.frames || []).map(frame => Number(frame.index)));
const expectedStageCount = (runDocument?.timeline?.frames || [])
    .flatMap(frame => frame.stages || [])
    .length;
const genericityFailures = collectGenericityFailures(runDocument);
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

try {
    await writeProgress(`goto ${route}`);
    await page.goto(route, { waitUntil: "domcontentloaded" });
    await writeProgress("wait scene host");
    await page.waitForSelector("[data-testid='webgl-scene-host']", { timeout: 15000 });
    await writeProgress("wait webgl runtime state");
    await page.waitForFunction(() => {
        const host = document.querySelector("[data-testid='webgl-scene-host']");
        return !!window.CanDoItAll?.webglScene && !!host?.__webglSceneState;
    }, null, { timeout: 15000 });
    await writeProgress("wait run playback bridge");
    await page.waitForFunction(() =>
        !!window.CanDoItAll?.webglSandbox?.runPlayback?.reference,
        null,
        { timeout: 15000 });

    await writeProgress(`load generated run document ${expectedRunId}`);
    await page.evaluate(json => window.CanDoItAll.webglSandbox.runPlayback.loadDocumentJson(json), runDocumentJson);
    await page.waitForFunction(({ selector, runId }) => {
        const text = document.querySelector(selector)?.textContent || "{}";
        try {
            return JSON.parse(text).runId === runId;
        } catch {
            return false;
        }
    }, { selector: diagnosticsSelector, runId: expectedRunId }, { timeout: 15000 });

    await writeProgress("start playback through browser bridge");
    await page.evaluate(() => {
        window.__sb12RunPlaybackError = "";
        window.__sb12RunPlaybackPromise = window.CanDoItAll.webglSandbox.runPlayback
            .play()
            .catch(error => {
                window.__sb12RunPlaybackError = error?.message || String(error);
            });
        return true;
    });
    let completionWaitTimedOut = false;
    try {
        await writeProgress("wait playback completion");
        await page.waitForFunction(({ selector, maxFrame }) => {
            const text = document.querySelector(selector)?.textContent || "{}";
            try {
                const diagnostics = JSON.parse(text);
                return diagnostics.currentFrameIndex >= maxFrame &&
                    diagnostics.isPlaying === false;
            } catch {
                return false;
            }
        }, { selector: diagnosticsSelector, maxFrame: expectedMaxFrame }, { timeout: 90000 });
    } catch {
        completionWaitTimedOut = true;
        await writeProgress("playback completion timed out");
    }

    await writeProgress("read diagnostics");
    const diagnostics = await readDiagnostics(page);
    const playBridgeError = await page.evaluate(() => window.__sb12RunPlaybackError || "");
    await writeProgress("capture screenshot");
    await page.screenshot({ path: screenshotPath, fullPage: true });

    const observer = diagnostics.observer || {};
    const hashes = diagnostics.hashes || {};
    const runtimeObjectProbe = await page.evaluate(objectId => {
        const host = document.querySelector("[data-testid='webgl-scene-host']");
        const state = host?.__webglSceneState;
        const vector = value => value
            ? { x: Number(value.x) || 0, y: Number(value.y) || 0, z: Number(value.z) || 0 }
            : null;
        const sceneObject = state?.sceneModel?.objects?.find?.(item => item.id === objectId) || null;
        const group = state?.objectGroups?.get?.(objectId) || null;
        const objectPosition = state?.objectPositions?.get?.(objectId) || null;
        return {
            objectId,
            sceneObjectPosition: vector(sceneObject?.position),
            objectPosition: vector(objectPosition),
            groupPosition: vector(group?.position),
            sceneObjectKind: sceneObject?.kind || "",
            sceneObjectFamily: sceneObject?.family || "",
            hasGroup: !!group
        };
    }, "object.node.actor.institution.policy-board");
    const disallowedConsole = consoleMessages.filter(item =>
        item.type === "error" ||
        item.type === "pageerror");
    const assertions = {
        route,
        viewport: "1920x1080",
        runDocumentPath,
        expectedRunId,
        expectedObjectCount: (runDocument?.initialScene?.scene?.objects || []).length,
        expectedLinkCount: (runDocument?.initialScene?.scene?.links || []).length,
        expectedStageCount,
        runtimeObjectProbe,
        genericExecutableBoundary: genericityFailures.length === 0,
        genericityFailures,
        playbackCompleted: completionWaitTimedOut === false,
        loadedGeneratedRunId: diagnostics.runId === expectedRunId,
        browserDocumentLoaded: hashes.browserDocumentLoaded === true,
        documentHashesMatch: observer.documentHashesMatch === true &&
            hashes.documentHashesMatch === true &&
            observer.expectedDocumentHash === observer.browserLoadedDocumentHash,
        observerProofValid: observer.observerProofValid === true &&
            observer.claimStatus === "observer-valid",
        browserHashesExported: typeof hashes.browserLoadedSceneContentHash === "string" &&
            hashes.browserLoadedSceneContentHash.startsWith("sha256:") &&
            typeof hashes.browserProofSnapshotHash === "string" &&
            hashes.browserProofSnapshotHash.startsWith("sha256:"),
        playBridgeError,
        runtimeIdle: observer.runtimeIdle === true &&
            Array.isArray(observer.runtimeIdleBlockers) &&
            observer.runtimeIdleBlockers.length === 0,
        completedStagesObserved: Array.isArray(observer.completedStageIds) &&
            observer.completedStageIds.length >= Math.max(1, expectedStageCount),
        finalPositionsObserved: positionsMatch(
            observer.expectedFinalObjectPositions,
            observer.browserFinalObjectPositions),
        noObserverErrors: Array.isArray(observer.errors) && observer.errors.length === 0,
        screenshotPath,
        consoleMessageCount: consoleMessages.length,
        disallowedConsole
    };

    const output = {
        assertions,
        diagnostics
    };

    await fs.writeFile(observerProofPath, `${JSON.stringify(observer, null, 2)}\n`);
    await fs.writeFile(assertionsPath, `${JSON.stringify(output, null, 2)}\n`);
    const consoleText = consoleMessages.length === 0
        ? "(no browser console messages)\n"
        : `${consoleMessages.map(item => `[${item.type}] ${item.text}`).join("\n")}\n`;
    await fs.writeFile(consolePath, consoleText);

    for (const [name, passed] of Object.entries(assertions)) {
        if (typeof passed === "boolean" && !passed) {
            throw new Error(`SB12 observer assertion failed: ${name}`);
        }
    }

    if (playBridgeError) {
        throw new Error(`Run playback bridge error: ${playBridgeError}`);
    }

    if (disallowedConsole.length > 0) {
        throw new Error(`Browser console contained disallowed errors: ${disallowedConsole.map(item => item.text).join(" | ")}`);
    }

    await writeProgress("proof passed");
    console.log(`SB12 multi-goods observer proof passed: ${observerProofPath}`);
} finally {
    await browser.close();
}
