const fs = require("fs/promises");
const path = require("path");
const { chromium } = require("playwright");

const proofDir = process.env.PROOF_DIR || path.resolve(__dirname, "..", "proof", "SB11");
const route = process.env.SMOKE_ROUTE || "http://127.0.0.1:5198/economy/simulation-sandbox";
const screenshotPath = path.join(proofDir, "economy-browser-smoke-1440x900.png");
const viewport = { width: 1440, height: 900 };

const failedResponses = [];
const consoleMessages = [];
const pageErrors = [];
const actions = [];

async function main() {
  await fs.mkdir(proofDir, { recursive: true });
  const browser = await chromium.launch({ headless: true });
  const context = await browser.newContext({ viewport, deviceScaleFactor: 1 });
  const page = await context.newPage();

  page.on("response", response => {
    if (response.status() >= 400) {
      failedResponses.push({ url: response.url(), status: response.status() });
    }
  });
  page.on("console", message => {
    consoleMessages.push({ type: message.type(), text: message.text() });
  });
  page.on("pageerror", error => {
    pageErrors.push({ name: error.name, message: error.message });
  });

  try {
    actions.push({ action: "goto", route });
    await page.goto(route, { waitUntil: "domcontentloaded", timeout: 60000 });
    await page.waitForSelector("[data-testid=\"economy-simulation-sandbox-page\"]", { timeout: 60000 });
    await page.waitForSelector("[data-testid=\"sandbox-webgl-view\"] canvas", { timeout: 60000 });
    await waitForButtonEnabled(page, "Apply frame");
    await page.waitForTimeout(1200);

    const initial = await collectState(page, "initial-scene-loaded");
    await writeJson("initial-scene-proof.json", {
      proofKind: "initial-scene",
      route,
      viewport,
      observedAtUtc: new Date().toISOString(),
      actions: [...actions],
      observed: initial,
      assertions: {
        viewportIsDesktopOnly: viewport.width >= 1440 && viewport.height >= 900,
        routeLoaded: initial.pagePresent,
        sceneCanvasPresent: initial.canvas.present,
        sceneHasStableSize: initial.sceneRect.width >= 900 && initial.sceneRect.height >= 600,
        initialSceneHasObjects: Number(initial.summary.Objects || 0) > 0,
        projectedFramesExist: Number(initial.summary.Frames || 0) > 0,
        browserApplyPendingBeforeApply: initial.summary["Browser apply"] === "pending"
      }
    });

    actions.push({ action: "click", control: "Last", effect: "seek last frame and apply browser frame" });
    await clickButton(page, "Last");
    await page.waitForFunction(() => {
      const runtime = document.querySelector("[data-testid=\"sandbox-browser-runtime\"]");
      const summary = Object.fromEntries(Array.from(document.querySelectorAll("[data-testid=\"sandbox-summary\"] > div"))
        .map(item => [item.querySelector("span")?.textContent?.trim() || "", item.querySelector("strong")?.textContent?.trim() || ""]));
      return runtime && runtime.textContent && runtime.textContent.includes("applied") && summary.Step === "2";
    }, null, { timeout: 60000 });
    await page.waitForTimeout(1000);
    const applied = await collectState(page, "frame-applied");
    await writeJson("applied-frame-proof.json", {
      proofKind: "applied-frame",
      route,
      viewport,
      observedAtUtc: new Date().toISOString(),
      actions: [...actions],
      observed: applied,
      assertions: {
        frameApplied: applied.summary["Browser apply"] === "applied",
        initialSceneAppliedToBrowser: applied.browserRuntime.appliedInitialScene === "True",
        appliedStageCountPositive: Number(applied.browserRuntime.appliedStageCount || 0) > 0,
        appliedPatchCountPositive: Number(applied.browserRuntime.appliedPatchCount || 0) > 0,
        browserRuntimeErrorsZero: applied.browserRuntime.runtimeErrorCount === "0",
        diagnosticsErrorsZero: applied.diagnostics.diagnosticErrorCount === "0",
        canvasStillPresent: applied.canvas.present
      }
    });

    actions.push({ action: "click", control: "Snapshot" });
    await clickButton(page, "Snapshot");
    await page.waitForFunction(() => document.body.textContent && document.body.textContent.includes("Snapshot "), null, { timeout: 60000 });
    await waitForButtonEnabled(page, "Analyze");
    actions.push({ action: "click", control: "Analyze" });
    await clickButton(page, "Analyze");
    await page.waitForFunction(() => {
      const analysis = document.querySelector("[data-testid=\"sandbox-analysis\"]");
      return analysis && analysis.querySelectorAll("li").length > 0;
    }, null, { timeout: 60000 });
    await page.waitForTimeout(800);
    const analysis = await collectState(page, "snapshot-analysis-visible");
    await page.screenshot({ path: screenshotPath, fullPage: true });

    await writeJson("snapshot-analysis-proof.json", {
      proofKind: "snapshot-analysis",
      route,
      viewport,
      observedAtUtc: new Date().toISOString(),
      actions: [...actions],
      observed: analysis,
      assertions: {
        snapshotCaptured: Boolean(analysis.diagnostics.currentSnapshotId),
        analysisVisible: analysis.analysis.findings.length > 0,
        analysisFindingsGeneric: analysis.analysis.findings.every(finding => finding.code && finding.title && finding.explanation),
        browserRuntimeStillApplied: analysis.summary["Browser apply"] === "applied",
        pageErrorsZero: pageErrors.length === 0
      }
    });

    const readinessAssertions = {
      viewportIsDesktopOnly: viewport.width >= 1440 && viewport.height >= 900,
      noMobileProofClaimed: true,
      browserRuntimeExercised: analysis.summary["Browser apply"] === "applied",
      initialSceneLoaded: Number(initial.summary.Objects || 0) > 0 && initial.canvas.present,
      frameApplied: applied.summary["Browser apply"] === "applied",
      snapshotAnalysisVisible: analysis.analysis.findings.length > 0,
      failedResponsesZero: failedResponses.length === 0,
      pageErrorsZero: pageErrors.length === 0,
      fullUiDemoNotClaimed: true
    };

    await writeJson("browser-smoke-readiness.json", {
      schemaVersion: "candoitall.economy.browser-smoke-readiness/v1",
      route,
      viewport,
      desktopOnly: true,
      mobileProofProduced: false,
      browserRuntimeExercised: true,
      result: Object.values(readinessAssertions).every(Boolean) ? "passed" : "failed",
      readinessMode: "large-screen-browser-smoke",
      fullUiDemoReady: false,
      finalReadiness: "browser smoke passed; full UI demo remains out of scope for this bundle",
      screenshotPath,
      actions,
      failedResponses,
      consoleMessages,
      pageErrors,
      assertions: readinessAssertions
    });

    const failures = Object.entries(readinessAssertions).filter(([, value]) => !value);
    if (failures.length > 0) {
      throw new Error(`Browser smoke assertions failed: ${failures.map(([key]) => key).join(", ")}`);
    }

    console.log(JSON.stringify({
      result: "passed",
      route,
      viewport,
      screenshotPath,
      appliedFrameIndex: applied.browserRuntime.frameIndex,
      appliedStageCount: applied.browserRuntime.appliedStageCount,
      appliedPatchCount: applied.browserRuntime.appliedPatchCount,
      analysisFindingCount: analysis.analysis.findings.length
    }, null, 2));
  } catch (error) {
    await writeJson("browser-smoke-readiness.json", {
      schemaVersion: "candoitall.economy.browser-smoke-readiness/v1",
      route,
      viewport,
      desktopOnly: true,
      mobileProofProduced: false,
      browserRuntimeExercised: false,
      result: "blocked",
      readinessMode: "large-screen-browser-smoke",
      fullUiDemoReady: false,
      finalReadiness: "browser smoke next",
      blockers: [{ message: error.message, stack: error.stack }],
      actions,
      failedResponses,
      consoleMessages,
      pageErrors
    });
    throw error;
  } finally {
    await context.close();
    await browser.close();
  }
}

async function clickButton(page, label) {
  await page.locator("button", { hasText: label }).click({ timeout: 60000 });
}

async function waitForButtonEnabled(page, label) {
  await page.waitForFunction(text => {
    return Array.from(document.querySelectorAll("button"))
      .some(button => button.textContent && button.textContent.includes(text) && !button.disabled);
  }, label, { timeout: 60000 });
}

async function collectState(page, phase) {
  return await page.evaluate(phaseName => {
    const text = selector => document.querySelector(selector)?.textContent?.trim() || "";
    const rect = selector => {
      const element = document.querySelector(selector);
      if (!element) {
        return { x: 0, y: 0, width: 0, height: 0 };
      }
      const box = element.getBoundingClientRect();
      return { x: box.x, y: box.y, width: box.width, height: box.height };
    };
    const readSummary = () => Object.fromEntries(Array.from(document.querySelectorAll("[data-testid=\"sandbox-summary\"] > div"))
      .map(item => [item.querySelector("span")?.textContent?.trim() || "", item.querySelector("strong")?.textContent?.trim() || ""])
      .filter(([key]) => key));
    const readDefinitionList = testId => Object.fromEntries(Array.from(document.querySelectorAll(`[data-testid="${testId}"] dl > div`))
      .map(item => [item.querySelector("dt")?.textContent?.trim() || "", item.querySelector("dd")?.textContent?.trim() || ""])
      .filter(([key]) => key));
    const canvas = document.querySelector("[data-testid=\"sandbox-webgl-view\"] canvas");
    const gl = canvas ? (canvas.getContext("webgl2") || canvas.getContext("webgl") || canvas.getContext("experimental-webgl")) : null;
    const analysisItems = Array.from(document.querySelectorAll("[data-testid=\"sandbox-analysis\"] li")).map(item => ({
      code: item.querySelector("strong")?.textContent?.trim() || "",
      title: item.querySelector("span")?.textContent?.trim() || "",
      explanation: item.querySelector("small")?.textContent?.trim() || ""
    }));

    return {
      phase: phaseName,
      url: window.location.href,
      pagePresent: Boolean(document.querySelector("[data-testid=\"economy-simulation-sandbox-page\"]")),
      statusText: text(".cda-economy-simulation-sandbox__header p"),
      summary: readSummary(),
      diagnostics: readDefinitionList("sandbox-diagnostics"),
      browserRuntime: readDefinitionList("sandbox-browser-runtime"),
      analysis: {
        text: text("[data-testid=\"sandbox-analysis\"]"),
        findings: analysisItems
      },
      sceneRect: rect("[data-testid=\"sandbox-webgl-view\"]"),
      canvas: {
        present: Boolean(canvas),
        width: canvas?.width || 0,
        height: canvas?.height || 0,
        clientWidth: canvas?.clientWidth || 0,
        clientHeight: canvas?.clientHeight || 0,
        webglContextAvailable: Boolean(gl)
      }
    };
  }, phase);
}

async function writeJson(fileName, value) {
  await fs.writeFile(path.join(proofDir, fileName), `${JSON.stringify(value, null, 2)}\n`, "utf8");
}

main().catch(error => {
  console.error(error);
  process.exitCode = 1;
});
