import fs from "node:fs";
import path from "node:path";
import { chromium } from "playwright";

const repoRoot = process.cwd();
const proofRoot = path.join(repoRoot, "codex", "bundles", "WebGlEngine_Stabilization_v14", "proof", "SB14");
const screenshotPath = path.join(proofRoot, "screenshots", "run-playback-1920x1080.png");
const reportPath = path.join(proofRoot, "browser-observer-proof.json");

const chromeCandidates = [
  "C:/Program Files/Google/Chrome/Application/chrome.exe",
  "C:/Program Files (x86)/Google/Chrome/Application/chrome.exe",
  path.join(process.env.LOCALAPPDATA || "", "Google", "Chrome", "Application", "chrome.exe"),
  "C:/Program Files/Microsoft/Edge/Application/msedge.exe",
  "C:/Program Files (x86)/Microsoft/Edge/Application/msedge.exe"
];
const executablePath = chromeCandidates.find(candidate => candidate && fs.existsSync(candidate));
if (!executablePath) {
  throw new Error("No system Chrome or Edge executable was found for browser proof.");
}

const browser = await chromium.launch({ headless: true, executablePath });
const page = await browser.newPage({ viewport: { width: 1920, height: 1080 }, deviceScaleFactor: 1 });
const consoleMessages = [];
const pageErrors = [];
page.on("console", message => consoleMessages.push({ type: message.type(), text: message.text() }));
page.on("pageerror", error => pageErrors.push(error.message));

try {
  await page.goto("http://127.0.0.1:5284/run-playback", { waitUntil: "networkidle", timeout: 60000 });
  await page.waitForSelector("[data-testid='webgl-run-playback-stage'] [data-testid='webgl-scene-host']", { timeout: 60000 });
  await page.waitForFunction(() => !!window.CanDoItAll?.webglSandbox?.runPlayback?.reference, null, { timeout: 60000 });

  for (let index = 0; index < 3; index += 1) {
    await page.getByRole("button", { name: /Step/i }).click();
    await page.waitForFunction(
      ({ expectedFrame }) => document.querySelector("[data-testid='webgl-run-frame']")?.textContent?.trim() === String(expectedFrame),
      { expectedFrame: index + 1 },
      { timeout: 30000 });
    await page.waitForTimeout(1500);
  }
  await page.getByRole("button", { name: /Snapshot/i }).click();
  await page.waitForTimeout(1000);

  const runtimeProof = await page.evaluate(async () => {
    const stage = document.querySelector("[data-testid='webgl-run-playback-stage']");
    const host = stage?.querySelector("[data-testid='webgl-scene-host']");
    if (!host || !window.CanDoItAll?.webglScene) {
      throw new Error("WebGL scene host or runtime API not found.");
    }

    const idle = await window.CanDoItAll.webglScene.waitForRuntimeIdle(host, {
      timeoutMs: 5000,
      pollIntervalMs: 25,
      reason: "sb14-browser-observer-proof",
      policyMode: "visualStrict"
    });
    const diagnostics = window.CanDoItAll.webglScene.getDiagnostics(host);
    const snapshot = window.CanDoItAll.webglScene.getProofSnapshot(host);
    const diagnosticsText = document.querySelector("[data-testid='webgl-run-diagnostics-json']")?.textContent || "{}";
    const panelDiagnostics = JSON.parse(diagnosticsText);
    const canvas = host.querySelector("canvas");
    let pixelSamples = [];
    if (canvas) {
      const gl = canvas.getContext("webgl2") || canvas.getContext("webgl");
      if (gl) {
        const points = [
          [Math.floor(canvas.width / 2), Math.floor(canvas.height / 2)],
          [Math.floor(canvas.width / 3), Math.floor(canvas.height / 3)],
          [Math.floor(canvas.width * 2 / 3), Math.floor(canvas.height * 2 / 3)]
        ];
        pixelSamples = points.map(([x, y]) => {
          const pixel = new Uint8Array(4);
          gl.readPixels(x, y, 1, 1, gl.RGBA, gl.UNSIGNED_BYTE, pixel);
          return { x, y, rgba: Array.from(pixel) };
        });
      }
    }

    return {
      route: location.pathname,
      viewport: `${window.innerWidth}x${window.innerHeight}`,
      hostFound: !!host,
      canvas: canvas ? { width: canvas.width, height: canvas.height, clientWidth: canvas.clientWidth, clientHeight: canvas.clientHeight } : null,
      pixelSamples,
      idle,
      diagnostics,
      snapshot,
      panelDiagnostics
    };
  });

  await page.screenshot({ path: screenshotPath, fullPage: true });
  const nonTransparentPixels = runtimeProof.pixelSamples.filter(sample => sample.rgba[3] > 0);
  const nonBlackPixels = runtimeProof.pixelSamples.filter(sample => sample.rgba[0] !== 0 || sample.rgba[1] !== 0 || sample.rgba[2] !== 0);
  const assertions = {
    routeLoaded: runtimeProof.route === "/run-playback",
    hostFound: runtimeProof.hostFound === true,
    canvasSized: !!runtimeProof.canvas && runtimeProof.canvas.width > 0 && runtimeProof.canvas.height > 0,
    strictVisualIdle: runtimeProof.idle?.idle === true && runtimeProof.idle?.policyMode === "visualStrict",
    diagnosticsCaptured: !!runtimeProof.diagnostics && runtimeProof.diagnostics.renderCount >= 1,
    proofSnapshotCaptured: !!runtimeProof.snapshot && runtimeProof.snapshot.objectCount >= 2,
    panelObserverCaptured: !!runtimeProof.panelDiagnostics?.observer,
    canvasPixelsReadable: runtimeProof.pixelSamples.length > 0,
    canvasHasVisiblePixels: nonTransparentPixels.length > 0 && nonBlackPixels.length > 0,
    noPageErrors: pageErrors.length === 0
  };
  const pass = Object.values(assertions).every(Boolean);
  const report = {
    generatedAtUtc: new Date().toISOString(),
    url: page.url(),
    browserExecutablePath: executablePath,
    screenshotPath: "bundle://proof/SB14/screenshots/run-playback-1920x1080.png",
    assertions,
    pass,
    runtimeProof,
    consoleMessages,
    pageErrors
  };
  fs.writeFileSync(reportPath, `${JSON.stringify(report, null, 2)}\n`, "utf8");
  if (!pass) {
    throw new Error(`Browser observer proof failed: ${JSON.stringify(assertions)}`);
  }
  console.log(`Browser observer proof passed: ${reportPath}`);
} finally {
  await browser.close();
}


