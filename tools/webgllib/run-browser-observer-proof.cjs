const fs = require("node:fs");
const path = require("node:path");
const { chromium } = require("playwright");

const args = parseArgs(process.argv.slice(2));
const url = args.url || "http://127.0.0.1:5206/run-playback";
const output = args.output || path.join("artifacts", "webgl-engine-rc", "browser-observer-proof.json");
const screenshot = args.screenshot || path.join("artifacts", "webgl-engine-rc", "run-playback.png");
const timeoutMs = Number(args.timeoutMs || 45000);

main().catch(error => {
  console.error(error.stack || error.message || String(error));
  process.exitCode = 1;
});

async function main() {
  fs.mkdirSync(path.dirname(output), { recursive: true });
  fs.mkdirSync(path.dirname(screenshot), { recursive: true });

  const consoleErrors = [];
  const browser = await chromium.launch({
    headless: true,
    args: [
      "--ignore-gpu-blocklist",
      "--use-gl=swiftshader"
    ]
  });

  try {
    const page = await browser.newPage({ viewport: { width: 1440, height: 960 } });
    page.on("console", message => {
      if (message.type() === "error") {
        consoleErrors.push(message.text());
      }
    });
    page.on("pageerror", error => {
      consoleErrors.push(error.message);
    });

    await page.goto(url, { waitUntil: "domcontentloaded", timeout: timeoutMs });
    await page.waitForSelector("[data-testid='webgl-run-playback-stage'] [data-testid='webgl-scene-host']", { timeout: timeoutMs });
    await page.waitForFunction(() =>
      Boolean(window.CanDoItAll?.webglScene?.waitForRuntimeIdle) &&
      Boolean(window.CanDoItAll?.webglSandbox?.runPlayback?.reference), null, { timeout: timeoutMs });

    await page.getByRole("button", { name: /^Play$/ }).click();
    await page.waitForTimeout(700);
    await page.getByRole("button", { name: /^Pause$/ }).click();
    const cancellationDiagnostics = await waitForDiagnostics(page, diagnostics =>
      Number(diagnostics.runtimeStopGeneration || 0) > 0 &&
      diagnostics.idle?.idle === true, timeoutMs);

    await page.goto(url, { waitUntil: "domcontentloaded", timeout: timeoutMs });
    await page.waitForSelector("[data-testid='webgl-run-playback-stage'] [data-testid='webgl-scene-host']", { timeout: timeoutMs });
    await page.waitForFunction(() =>
      Boolean(window.CanDoItAll?.webglScene?.waitForRuntimeIdle) &&
      Boolean(window.CanDoItAll?.webglSandbox?.runPlayback?.reference), null, { timeout: timeoutMs });

    await page.getByRole("button", { name: /^Play$/ }).click();
    const finalDiagnostics = await waitForDiagnostics(page, diagnostics =>
      diagnostics.observer?.observerProofValid === true &&
      diagnostics.observer?.claimStatus === "observer-valid" &&
      diagnostics.observer?.runtimeIdle === true &&
      Number(diagnostics.currentFrameIndex || 0) >= 3, timeoutMs);

    await page.evaluate(() => window.CanDoItAll.webglSandbox.runPlayback.snapshot());
    const stableDiagnostics = await waitForDiagnostics(page, diagnostics =>
      diagnostics.observer?.observerProofValid === true &&
      diagnostics.observer?.runtimeIdle === true, timeoutMs);

    await page.screenshot({ path: screenshot, fullPage: true });

    const assertions = {
      browserRuntimeValid: stableDiagnostics.observer?.browserRuntimeValid === true,
      uiValid: stableDiagnostics.observer?.uiValid === true,
      observerProofValid: stableDiagnostics.observer?.observerProofValid === true,
      documentHashesMatch: stableDiagnostics.observer?.documentHashesMatch === true,
      sceneContentHashesMatch: stableDiagnostics.observer?.sceneContentHashesMatch === true,
      driverHashesMatch: stableDiagnostics.observer?.driverHashesMatch === true,
      runtimeIdle: stableDiagnostics.observer?.runtimeIdle === true,
      finalPositionsCompared: Number(stableDiagnostics.observer?.metadata?.finalObjectPositionCount || 0) > 0,
      cancellationStoppedRuntime: Number(cancellationDiagnostics.runtimeStopGeneration || 0) > 0 &&
        cancellationDiagnostics.idle?.idle === true,
      consoleErrorsEmpty: consoleErrors.length === 0
    };

    const report = {
      schemaVersion: "webgl-browser-observer-proof/v1",
      generatedAtUtc: new Date().toISOString(),
      route: url,
      screenshotPath: path.relative(process.cwd(), screenshot).replaceAll(path.sep, "/"),
      consoleErrors,
      cancellation: {
        currentFrameIndex: cancellationDiagnostics.currentFrameIndex,
        runtimeStopGeneration: cancellationDiagnostics.runtimeStopGeneration,
        idle: cancellationDiagnostics.idle,
        observer: cancellationDiagnostics.observer
      },
      final: {
        currentFrameIndex: stableDiagnostics.currentFrameIndex,
        runtimeStopGeneration: stableDiagnostics.runtimeStopGeneration,
        hashes: stableDiagnostics.hashes,
        idle: stableDiagnostics.idle,
        batch: stableDiagnostics.batch,
        proofSnapshot: stableDiagnostics.proofSnapshot,
        observer: stableDiagnostics.observer
      },
      assertions
    };

    fs.writeFileSync(output, `${JSON.stringify(report, null, 2)}\n`);

    const failed = Object.entries(assertions).filter(([, value]) => value !== true);
    if (failed.length > 0) {
      throw new Error(`Browser observer proof assertions failed: ${failed.map(([key]) => key).join(", ")}`);
    }

    console.log(`Browser observer proof passed: ${path.relative(process.cwd(), output)}`);
  } finally {
    await browser.close();
  }
}

async function waitForDiagnostics(page, predicate, timeoutMs) {
  const started = Date.now();
  let lastDiagnostics = null;
  let lastError = null;
  while (Date.now() - started < timeoutMs) {
    try {
      lastDiagnostics = await readDiagnostics(page);
      if (predicate(lastDiagnostics)) {
        return lastDiagnostics;
      }
    } catch (error) {
      lastError = error;
    }

    await page.waitForTimeout(200);
  }

  throw new Error(`Timed out waiting for diagnostics. Last error: ${lastError?.message || "none"}. Last diagnostics: ${JSON.stringify(lastDiagnostics)}`);
}

async function readDiagnostics(page) {
  const text = await page.locator("[data-testid='webgl-run-diagnostics-json']").textContent({ timeout: 5000 });
  return JSON.parse(text || "{}");
}

function parseArgs(values) {
  const parsed = {};
  for (let index = 0; index < values.length; index += 1) {
    const value = values[index];
    if (!value.startsWith("--")) {
      continue;
    }

    const key = value.slice(2);
    const next = values[index + 1];
    if (!next || next.startsWith("--")) {
      parsed[key] = "true";
    } else {
      parsed[key] = next;
      index += 1;
    }
  }

  return parsed;
}
