import fs from "node:fs/promises";
import { createRequire } from "node:module";
import path from "node:path";

const repoRoot = process.cwd();
const require = createRequire(import.meta.url);
const playwrightPackage = process.env.PLAYWRIGHT_PACKAGE_ROOT
  ? require(path.join(process.env.PLAYWRIGHT_PACKAGE_ROOT, "node_modules", "playwright"))
  : require("playwright");
const { chromium } = playwrightPackage;
const browserDir = path.join(repoRoot, "codex", "bundles", "WebGlEngine_Economy_Followup_v9", "proof", "SB02", "browser");
const route = "http://localhost:5298/run-playback";
const screenshotPath = path.join(browserDir, "runplayback-pause-after.png");
const assertionsPath = path.join(browserDir, "runplayback-pause-assertions.json");
const consolePath = path.join(browserDir, "runplayback-pause-console.log");
const hostSelector = "[data-testid='webgl-scene-host']";
const diagnosticsSelector = "[data-testid='webgl-run-diagnostics-json']";

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

async function runtimeSample() {
  return await page.evaluate(selector => {
    const host = document.querySelector(selector);
    const diagnostics = window.CanDoItAll?.webglScene?.getDiagnostics(host) ?? null;
    return {
      diagnostics,
      hasRuntime: !!host?.__webglSceneState
    };
  }, hostSelector);
}

async function readUiDiagnostics() {
  const text = await page.locator(diagnosticsSelector).textContent();
  return JSON.parse(text || "{}");
}

try {
  await page.goto(route, { waitUntil: "domcontentloaded" });
  await page.waitForSelector(hostSelector, { timeout: 15000 });
  await page.waitForFunction(selector => {
    const host = document.querySelector(selector);
    return !!window.CanDoItAll?.webglScene && !!host?.__webglSceneState;
  }, hostSelector, { timeout: 15000 });
  await page.getByRole("button", { name: "Play" }).click();
  await page.waitForFunction(selector => {
    const host = document.querySelector(selector);
    const diagnostics = window.CanDoItAll?.webglScene?.getDiagnostics(host);
    return (diagnostics?.activeMotionCount || 0) > 0 ||
      (diagnostics?.queuedMotionCount || 0) > 0 ||
      (diagnostics?.queuedCommandStageCount || 0) > 0 ||
      !!diagnostics?.currentCommandBatchId;
  }, hostSelector, { timeout: 5000 });

  const beforePause = await runtimeSample();
  const pauseStarted = Date.now();
  await page.getByRole("button", { name: "Pause" }).click();

  let settledSample = null;
  let settledElapsedMs = null;
  while (Date.now() - pauseStarted <= 500) {
    const sample = await runtimeSample();
    const blockers = motionStageBlockers(sample.diagnostics);
    if (sample.hasRuntime && blockers.length === 0) {
      settledSample = {
        ...sample,
        motionStageBlockers: blockers
      };
      settledElapsedMs = Date.now() - pauseStarted;
      break;
    }

    await page.waitForTimeout(25);
  }

  const after500Sample = await runtimeSample();
  const after500Blockers = motionStageBlockers(after500Sample.diagnostics);
  await page.waitForTimeout(1000);
  const uiDiagnostics = await readUiDiagnostics();
  await page.screenshot({ path: screenshotPath, fullPage: true });

  const disallowedConsole = consoleMessages.filter(item =>
    item.type === "error" ||
    item.type === "pageerror");
  const assertions = {
    route,
    viewport: "1920x1080",
    beforePause,
    settledWithin500ms: settledSample !== null && settledElapsedMs <= 500,
    settledElapsedMs,
    settledSample,
    after500: {
      ...after500Sample,
      motionStageBlockers: after500Blockers
    },
    uiDiagnostics: {
      runtimeStopGeneration: uiDiagnostics.runtimeStopGeneration,
      ignoredStaleRuntimeCallbackCount: uiDiagnostics.ignoredStaleRuntimeCallbackCount,
      idle: uiDiagnostics.idle,
      batch: uiDiagnostics.batch,
      isPlaying: uiDiagnostics.isPlaying
    },
    screenshotPath,
    consoleMessageCount: consoleMessages.length,
    disallowedConsole
  };

  await fs.writeFile(assertionsPath, `${JSON.stringify(assertions, null, 2)}\n`);
  await fs.writeFile(consolePath, consoleMessages.map(item => `[${item.type}] ${item.text}`).join("\n"));

  if (!assertions.settledWithin500ms) {
    throw new Error(`Pause did not clear motion/stage blockers within 500 ms. Blockers: ${after500Blockers.join(", ")}`);
  }

  if (after500Blockers.length > 0) {
    throw new Error(`Motion/stage blockers remain after 500 ms: ${after500Blockers.join(", ")}`);
  }

  if ((assertions.uiDiagnostics.runtimeStopGeneration || 0) < 1) {
    throw new Error("UI diagnostics did not expose runtimeStopGeneration after pause.");
  }

  if (disallowedConsole.length > 0) {
    throw new Error(`Browser console contained disallowed errors: ${disallowedConsole.map(item => item.text).join(" | ")}`);
  }
} finally {
  await browser.close();
}
