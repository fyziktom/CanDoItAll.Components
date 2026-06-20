const fs = require("fs");
const path = require("path");
const { chromium } = require("playwright");

const outputPath = process.argv[2];
const screenshotPath = process.argv[3];
const url = process.argv[4] || "http://localhost:5298/run-playback";

if (!outputPath) {
  throw new Error("Usage: node pause-settled-after.cjs <output-json> <screenshot-path> [url]");
}

async function readDiagnosticsJson(page) {
  const text = await page.locator('[data-testid="webgl-run-diagnostics-json"]').textContent();
  return JSON.parse(text || "{}");
}

async function readBrowserRuntime(page, reason) {
  return page.evaluate(async idleReason => {
    const host = document.querySelector('[data-testid="webgl-scene-host"]');
    if (!host || !window.CanDoItAll?.webglScene) {
      return { available: false, diagnostics: null, idle: null };
    }

    const idle = await window.CanDoItAll.webglScene.waitForRuntimeIdle(host, {
      timeoutMs: 2000,
      pollIntervalMs: 16,
      reason: idleReason
    });
    const diagnostics = window.CanDoItAll.webglScene.getDiagnostics(host);
    return { available: true, diagnostics, idle };
  }, reason);
}

async function status(page) {
  return {
    frame: (await page.locator('[data-testid="webgl-run-frame"]').textContent() || "").trim(),
    playing: (await page.locator('[data-testid="webgl-run-playing"]').textContent() || "").trim(),
    status: (await page.locator('[data-testid="webgl-run-status"]').textContent() || "").trim()
  };
}

async function enqueueLongMotion(page) {
  return page.evaluate(() => {
    const host = document.querySelector('[data-testid="webgl-scene-host"]');
    if (!host || !window.CanDoItAll?.webglScene) {
      throw new Error("WebGL runtime facade was not available.");
    }

    return window.CanDoItAll.webglScene.enqueueMotionDetailed(host, {
      motionId: "sb02.long.pause.motion",
      objectId: "object.runner",
      targetPosition: { x: 3, y: 0, z: 0 },
      durationSeconds: 5,
      queuePolicy: "cancel-and-replace"
    });
  });
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
  await page.getByRole("button", { name: "Play" }).click();
  await page.waitForTimeout(100);
  const injectedMotion = await enqueueLongMotion(page);
  await page.waitForFunction(() => {
    const host = document.querySelector('[data-testid="webgl-scene-host"]');
    const diagnostics = host && window.CanDoItAll?.webglScene?.getDiagnostics(host);
    return (diagnostics?.activeMotionCount || 0) > 0;
  }, null, { timeout: 5000 });

  const beforePause = {
    status: await status(page),
    diagnosticsJson: await readDiagnosticsJson(page),
    browserRuntime: {
      available: true,
      diagnostics: await page.evaluate(() => {
        const host = document.querySelector('[data-testid="webgl-scene-host"]');
        return window.CanDoItAll.webglScene.getDiagnostics(host);
      })
    }
  };

  await page.getByRole("button", { name: "Pause" }).click();
  await page.waitForFunction(() => {
    const playing = document.querySelector('[data-testid="webgl-run-playing"]')?.textContent?.trim();
    const host = document.querySelector('[data-testid="webgl-scene-host"]');
    const diagnostics = host && window.CanDoItAll?.webglScene?.getDiagnostics(host);
    return playing === "False" &&
      (diagnostics?.activeMotionCount || 0) === 0 &&
      (diagnostics?.queuedMotionCount || 0) === 0 &&
      (diagnostics?.queuedCommandStageCount || 0) === 0;
  }, null, { timeout: 6000 });

  const afterPause = {
    status: await status(page),
    diagnosticsJson: await readDiagnosticsJson(page),
    browserRuntime: await readBrowserRuntime(page, "sb02-pause-settled-after")
  };

  await page.waitForTimeout(750);
  const afterLateDrain = {
    status: await status(page),
    diagnosticsJson: await readDiagnosticsJson(page),
    browserRuntime: await readBrowserRuntime(page, "sb02-pause-settled-late-drain")
  };

  if (screenshotPath) {
    await page.screenshot({ path: screenshotPath, fullPage: true });
  }

  await browser.close();

  const artifact = {
    schemaVersion: "webgl-runtime-pause-settled/v1",
    route: url,
    viewport: { width: 1440, height: 900 },
    injectedMotion,
    beforePause,
    afterPause,
    afterLateDrain,
    assertions: {
      runtimeFacadeAvailable: afterPause.browserRuntime.available === true,
      activeMotionWasPresentBeforePause: (beforePause.browserRuntime.diagnostics?.activeMotionCount || 0) > 0,
      pauseSetsPlayingFalse: afterPause.status.playing === "False",
      pauseStatusSettled: afterPause.status.status === "Paused.",
      afterPauseBrowserIdle: afterPause.browserRuntime.idle?.idle === true,
      afterPauseIdleDiagnosticsAgree: afterPause.browserRuntime.diagnostics?.lastRuntimeStopIdle === true,
      afterPauseIdleDidNotTimeOut: afterPause.browserRuntime.diagnostics?.lastRuntimeStopTimedOut === false,
      afterPauseNoActiveMotions: (afterPause.browserRuntime.diagnostics?.activeMotionCount || 0) === 0,
      afterPauseNoQueuedMotions: (afterPause.browserRuntime.diagnostics?.queuedMotionCount || 0) === 0,
      afterPauseNoQueuedCommandStages: (afterPause.browserRuntime.diagnostics?.queuedCommandStageCount || 0) === 0,
      stopGenerationAdvanced: (afterPause.browserRuntime.diagnostics?.runtimeStopGeneration || 0) > 0,
      noLateMotionCompletedStatusMutation: !String(afterLateDrain.status.status || "").startsWith("Motion completed:")
    },
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
