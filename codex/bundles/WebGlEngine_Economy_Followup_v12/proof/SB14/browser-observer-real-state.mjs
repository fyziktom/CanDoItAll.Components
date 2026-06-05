import fs from "node:fs/promises";
import { createRequire } from "node:module";
import path from "node:path";

const repoRoot = process.cwd();
const require = createRequire(import.meta.url);
const playwrightPackage = process.env.PLAYWRIGHT_PACKAGE_ROOT
    ? require(path.join(process.env.PLAYWRIGHT_PACKAGE_ROOT, "node_modules", "playwright"))
    : require("playwright");
const { chromium } = playwrightPackage;

const proofDir = path.join(
    repoRoot,
    "codex",
    "bundles",
    "WebGlEngine_Economy_Followup_v12",
    "proof",
    "SB14");
const route = process.env.CDA_SB14_WEBGL_URL || "http://127.0.0.1:5327/run-playback";
const screenshotPath = path.join(proofDir, "browser-observer-after.png");
const observerProofPath = path.join(proofDir, "browser-observer-proof.json");
const assertionsPath = path.join(proofDir, "browser-observer-assertions.json");
const consolePath = path.join(proofDir, "browser-observer-console.log");
const progressPath = path.join(proofDir, "browser-observer-progress.log");
const diagnosticsSelector = "[data-testid='webgl-run-diagnostics-json']";

await fs.mkdir(proofDir, { recursive: true });
await fs.writeFile(progressPath, "");

async function writeProgress(message) {
    await fs.appendFile(progressPath, `${new Date().toISOString()} ${message}\n`);
}

async function readDiagnostics(page) {
    const text = await page.locator(diagnosticsSelector).textContent();
    return JSON.parse(text || "{}");
}

function vectorMatches(expected, actual) {
    return near(expected?.x, actual?.x) &&
        near(expected?.y, actual?.y) &&
        near(expected?.z, actual?.z);
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

function near(expected, actual) {
    return Math.abs(Number(expected) - Number(actual)) <= 0.001;
}

const consoleMessages = [];
let browser;
try {
    browser = await chromium.launch({ channel: "chrome", headless: true });
} catch {
    browser = await chromium.launch({ headless: true });
}

const page = await browser.newPage({ viewport: { width: 1920, height: 1080 } });
page.on("console", message => {
    consoleMessages.push({ type: message.type(), text: message.text() });
});
page.on("pageerror", error => {
    consoleMessages.push({ type: "pageerror", text: error.message });
});

try {
    await writeProgress(`goto ${route}`);
    await page.goto(route, { waitUntil: "domcontentloaded", timeout: 60000 });
    await writeProgress("wait scene host");
    await page.waitForSelector("[data-testid='webgl-scene-host']", { timeout: 30000 });
    await writeProgress("wait webgl runtime state");
    await page.waitForFunction(() => {
        const host = document.querySelector("[data-testid='webgl-scene-host']");
        return !!window.CanDoItAll?.webglScene && !!host?.__webglSceneState;
    }, null, { timeout: 30000 });
    await writeProgress("wait run playback bridge");
    await page.waitForFunction(() =>
        !!window.CanDoItAll?.webglSandbox?.runPlayback?.reference,
        null,
        { timeout: 30000 });

    await writeProgress("start playback through browser bridge");
    await page.evaluate(() => {
        window.__sb14RunPlaybackError = "";
        window.__sb14RunPlaybackPromise = window.CanDoItAll.webglSandbox.runPlayback
            .play()
            .catch(error => {
                window.__sb14RunPlaybackError = error?.message || String(error);
            });
        return true;
    });

    let completionWaitTimedOut = false;
    try {
        await writeProgress("wait playback completion");
        await page.waitForFunction(selector => {
            const text = document.querySelector(selector)?.textContent || "{}";
            try {
                const diagnostics = JSON.parse(text);
                return diagnostics.currentFrameIndex >= 3 &&
                    diagnostics.isPlaying === false;
            } catch {
                return false;
            }
        }, diagnosticsSelector, { timeout: 90000 });
    } catch {
        completionWaitTimedOut = true;
        await writeProgress("playback completion timed out");
    }

    await writeProgress("read diagnostics");
    const diagnostics = await readDiagnostics(page);
    const observer = diagnostics.observer || {};
    const hashes = diagnostics.hashes || {};
    const playBridgeError = await page.evaluate(() => window.__sb14RunPlaybackError || "");
    await writeProgress("capture screenshot");
    await page.screenshot({ path: screenshotPath, fullPage: true });

    const disallowedConsole = consoleMessages.filter(item =>
        item.type === "error" ||
        item.type === "pageerror");

    const assertions = {
        route,
        viewport: "1920x1080",
        playbackCompleted: completionWaitTimedOut === false,
        loadedRunId: diagnostics.runId === "generic-run-demo",
        browserDocumentLoaded: hashes.browserDocumentLoaded === true,
        documentHashesMatch: observer.documentHashesMatch === true &&
            hashes.documentHashesMatch === true &&
            observer.expectedDocumentHash === observer.browserLoadedDocumentHash,
        sceneContentHashesMatch: observer.sceneContentHashesMatch === true &&
            observer.expectedSceneContentHash === observer.browserLoadedSceneContentHash,
        driverHashesMatch: observer.driverHashesMatch === true &&
            typeof observer.expectedDriverHash === "string" &&
            observer.expectedDriverHash.startsWith("sha256:") &&
            observer.expectedDriverHash === observer.browserLoadedDriverHash,
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
            observer.completedStageIds.length >= 4,
        finalPositionsObserved: positionsMatch(
            observer.expectedFinalObjectPositions,
            observer.browserFinalObjectPositions),
        noObserverErrors: Array.isArray(observer.errors) && observer.errors.length === 0,
        screenshotPath,
        consoleMessageCount: consoleMessages.length,
        disallowedConsole
    };

    await fs.writeFile(observerProofPath, `${JSON.stringify(observer, null, 2)}\n`);
    await fs.writeFile(assertionsPath, `${JSON.stringify({ assertions, diagnostics }, null, 2)}\n`);
    await fs.writeFile(
        consolePath,
        consoleMessages.length === 0
            ? "(no browser console messages)\n"
            : `${consoleMessages.map(item => `[${item.type}] ${item.text}`).join("\n")}\n`);

    for (const [name, passed] of Object.entries(assertions)) {
        if (typeof passed === "boolean" && !passed) {
            throw new Error(`SB14 browser observer assertion failed: ${name}`);
        }
    }

    if (playBridgeError) {
        throw new Error(`Run playback bridge error: ${playBridgeError}`);
    }

    if (disallowedConsole.length > 0) {
        throw new Error(`Browser console contained disallowed errors: ${disallowedConsole.map(item => item.text).join(" | ")}`);
    }

    await writeProgress("proof passed");
    console.log(`SB14 browser observer proof passed: ${observerProofPath}`);
} finally {
    await browser.close();
}
