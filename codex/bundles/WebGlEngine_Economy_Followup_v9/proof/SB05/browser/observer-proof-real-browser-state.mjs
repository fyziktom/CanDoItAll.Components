import fs from "node:fs/promises";
import { createRequire } from "node:module";
import path from "node:path";

const repoRoot = process.cwd();
const require = createRequire(import.meta.url);
const playwrightPackage = process.env.PLAYWRIGHT_PACKAGE_ROOT
    ? require(path.join(process.env.PLAYWRIGHT_PACKAGE_ROOT, "node_modules", "playwright"))
    : require("playwright");
const { chromium } = playwrightPackage;
const browserDir = path.join(repoRoot, "codex", "bundles", "WebGlEngine_Economy_Followup_v9", "proof", "SB05", "browser");
const route = process.env.SB05_WEBGL_URL || "http://localhost:5298/run-playback";
const screenshotPath = path.join(browserDir, "observer-proof-after.png");
const observerProofPath = path.join(browserDir, "observer-proof.json");
const assertionsPath = path.join(browserDir, "observer-proof-assertions.json");
const consolePath = path.join(browserDir, "observer-proof-console.log");
const progressPath = path.join(browserDir, "observer-proof-progress.log");
const diagnosticsSelector = "[data-testid='webgl-run-diagnostics-json']";
const runPlaybackSourcePath = path.join(
    repoRoot,
    "src",
    "CanDoItAll.Components.WebGlSandbox",
    "Components",
    "Pages",
    "RunPlayback.razor.cs");

await fs.mkdir(browserDir, { recursive: true });
await fs.writeFile(progressPath, "");

async function writeProgress(message) {
    await fs.appendFile(progressPath, `${new Date().toISOString()} ${message}\n`);
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

async function readDiagnostics() {
    const text = await page.locator(diagnosticsSelector).textContent();
    return JSON.parse(text || "{}");
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

function vectorMatches(expected, actual) {
    return near(expected?.x, actual?.x) &&
        near(expected?.y, actual?.y) &&
        near(expected?.z, actual?.z);
}

function near(expected, actual) {
    return Math.abs(Number(expected) - Number(actual)) <= 0.001;
}

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

    await writeProgress("start playback through browser bridge");
    await page.evaluate(() => {
        window.__sb05RunPlaybackError = "";
        window.__sb05RunPlaybackPromise = window.CanDoItAll.webglSandbox.runPlayback
            .play()
            .catch(error => {
                window.__sb05RunPlaybackError = error?.message || String(error);
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
        }, diagnosticsSelector, { timeout: 60000 });
    } catch {
        completionWaitTimedOut = true;
        await writeProgress("playback completion timed out");
    }

    await writeProgress("read diagnostics");
    const diagnostics = await readDiagnostics();
    const playBridgeError = await page.evaluate(() => window.__sb05RunPlaybackError || "");
    await writeProgress("capture screenshot");
    await page.screenshot({ path: screenshotPath, fullPage: true });
    const sourceText = await fs.readFile(runPlaybackSourcePath, "utf8");
    const sourceNoSelfCompare = !/WebGlRunObserverProof\.Compare\(\s*runDocument\s*,\s*runDocument\b/s.test(sourceText);

    const observer = diagnostics.observer || {};
    const hashes = diagnostics.hashes || {};
    const disallowedConsole = consoleMessages.filter(item =>
        item.type === "error" ||
        item.type === "pageerror");
    const assertions = {
        route,
        viewport: "1920x1080",
        playbackCompleted: completionWaitTimedOut === false,
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
        sourceNoSelfCompare,
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
            throw new Error(`SB05 observer assertion failed: ${name}`);
        }
    }

    if (disallowedConsole.length > 0) {
        throw new Error(`Browser console contained disallowed errors: ${disallowedConsole.map(item => item.text).join(" | ")}`);
    }

    await writeProgress("proof passed");
    console.log(`SB05 observer proof passed: ${observerProofPath}`);
} finally {
    await browser.close();
}
