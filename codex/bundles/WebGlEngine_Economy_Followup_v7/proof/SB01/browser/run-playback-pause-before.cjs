const fs = require("fs");
const path = require("path");
const { chromium } = require("playwright");

const outputPath = process.argv[2];
const screenshotPath = process.argv[3];
const url = process.argv[4] || "http://localhost:5298/run-playback";

if (!outputPath) {
  throw new Error("Usage: node run-playback-pause-before.cjs <output-json> <screenshot-path> [url]");
}

async function readDiagnostics(page) {
  const text = await page.locator('[data-testid="webgl-run-diagnostics-json"]').textContent();
  return JSON.parse(text || "{}");
}

async function readBrowserRuntime(page) {
  return page.evaluate(async () => {
    const host = document.querySelector('[data-testid="webgl-scene-host"]');
    if (!host || !window.CanDoItAll?.webglScene) {
      return {
        available: false,
        diagnostics: null,
        idle: null
      };
    }

    const diagnostics = window.CanDoItAll.webglScene.getDiagnostics(host);
    const idle = await window.CanDoItAll.webglScene.waitForRuntimeIdle(host, {
      timeoutMs: 2000,
      pollIntervalMs: 16,
      reason: "sb01-current-pause-baseline"
    });

    return {
      available: true,
      diagnostics,
      idle
    };
  });
}

async function status(page) {
  return {
    frame: (await page.locator('[data-testid="webgl-run-frame"]').textContent() || "").trim(),
    playing: (await page.locator('[data-testid="webgl-run-playing"]').textContent() || "").trim(),
    status: (await page.locator('[data-testid="webgl-run-status"]').textContent() || "").trim()
  };
}

(async () => {
  const launchOptions = { headless: true };
  if (process.env.PLAYWRIGHT_EXECUTABLE_PATH) {
    launchOptions.executablePath = process.env.PLAYWRIGHT_EXECUTABLE_PATH;
  }

  const browser = await chromium.launch(launchOptions);
  const page = await browser.newPage({ viewport: { width: 1440, height: 900 } });
  const consoleMessages = [];
  const pageErrors = [];
  page.on("console", message => consoleMessages.push({ type: message.type(), text: message.text() }));
  page.on("pageerror", error => pageErrors.push(error.message));

  await page.goto(url, { waitUntil: "networkidle" });
  await page.locator('[data-testid="webgl-scene-host"]').waitFor({ state: "visible", timeout: 30000 });
  await page.getByRole("button", { name: "Snapshot" }).click();
  await page.waitForTimeout(300);
  const initial = {
    status: await status(page),
    diagnosticsJson: await readDiagnostics(page),
    browserRuntime: await readBrowserRuntime(page)
  };

  await page.getByRole("button", { name: "Play" }).click();
  await page.waitForTimeout(120);
  const duringPlayback = {
    status: await status(page),
    diagnosticsJson: await readDiagnostics(page),
    browserRuntime: await readBrowserRuntime(page)
  };

  await page.getByRole("button", { name: "Pause" }).click();
  await page.waitForTimeout(1200);
  const afterPause = {
    status: await status(page),
    diagnosticsJson: await readDiagnostics(page),
    browserRuntime: await readBrowserRuntime(page)
  };

  await page.waitForTimeout(1000);
  const afterLateDrain = {
    status: await status(page),
    diagnosticsJson: await readDiagnostics(page),
    browserRuntime: await readBrowserRuntime(page)
  };

  if (screenshotPath) {
    await page.screenshot({ path: screenshotPath, fullPage: true });
  }

  await browser.close();

  const artifact = {
    schemaVersion: "webgl-run-playback-pause-baseline/v1",
    route: url,
    viewport: { width: 1440, height: 900 },
    assertions: {
      runtimeFacadeAvailable: afterPause.browserRuntime.available === true,
      pauseSetsPlayingFalse: afterPause.status.playing === "False",
      afterPauseBrowserIdle: afterPause.browserRuntime.idle?.idle === true,
      afterPauseNoActiveMotions: (afterPause.browserRuntime.diagnostics?.activeMotionCount || 0) === 0,
      afterPauseNoQueuedMotions: (afterPause.browserRuntime.diagnostics?.queuedMotionCount || 0) === 0,
      afterPauseNoQueuedCommandStages: (afterPause.browserRuntime.diagnostics?.queuedCommandStageCount || 0) === 0,
      noLateMotionCompletedStatusMutation: !String(afterLateDrain.status.status || "").startsWith("Motion completed:")
    },
    initial,
    duringPlayback,
    afterPause,
    afterLateDrain,
    consoleMessages,
    pageErrors
  };

  fs.mkdirSync(path.dirname(outputPath), { recursive: true });
  fs.writeFileSync(outputPath, `${JSON.stringify(artifact, null, 2)}\n`, "utf8");

  const failed = Object.entries(artifact.assertions).filter(([, value]) => value !== true);
  if (failed.length > 0 || pageErrors.length > 0) {
    console.error(JSON.stringify({ failedAssertions: failed.map(([key]) => key), pageErrors }, null, 2));
    process.exitCode = 1;
  } else {
    console.log(JSON.stringify({ passedAssertions: Object.keys(artifact.assertions), outputPath, screenshotPath }, null, 2));
  }
})().catch(error => {
  console.error(error);
  process.exit(1);
});
