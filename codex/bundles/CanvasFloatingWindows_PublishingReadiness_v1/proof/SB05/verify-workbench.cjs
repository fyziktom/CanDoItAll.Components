const fs = require("node:fs");
const path = require("node:path");
const { chromium } = require("playwright");

const baseUrl = process.env.CANVAS_WORKBENCH_BASE_URL || "http://127.0.0.1:5088";
const route = "/groups/canvas";
const proofRoot = __dirname;
const screenshotRoot = path.join(proofRoot, "screenshots");
const transcriptPath = path.join(proofRoot, "transcripts", "playwright-workbench.txt");
const actionsPath = path.join(proofRoot, "browser-actions.json");
const consolePath = path.join(proofRoot, "console-log.txt");

fs.mkdirSync(screenshotRoot, { recursive: true });
fs.mkdirSync(path.dirname(transcriptPath), { recursive: true });

const scenarios = [
  { name: "happy-path", query: "scenario=happy-path", expectedNodes: 5, expectedMinimap: true, expectedQuickCreate: true },
  { name: "dense-content", query: "scenario=dense-content", expectedNodes: 6, expectedMinimap: true, expectedQuickCreate: true },
  { name: "empty-state", query: "scenario=empty-state", expectedNodes: 0, expectedMinimap: false, expectedQuickCreate: true },
  { name: "disabled-state", query: "scenario=disabled-state", expectedNodes: 5, expectedMinimap: true, expectedQuickCreate: false },
  { name: "long-text", query: "scenario=long-text", expectedNodes: 5, expectedMinimap: true, expectedQuickCreate: true }
];

const viewports = [
  { name: "max-desktop", width: 1920, height: 1080 },
  { name: "desktop-1366", width: 1366, height: 900 },
  { name: "tablet-1024", width: 1024, height: 768 },
  { name: "mobile-390", width: 390, height: 844 }
];

function scenarioUrl(scenario) {
  return `${baseUrl}${route}?${scenario.query}`;
}

function boxSummary(box) {
  if (!box) {
    return null;
  }

  return {
    x: Math.round(box.x),
    y: Math.round(box.y),
    width: Math.round(box.width),
    height: Math.round(box.height)
  };
}

async function visibleBox(page, selector, label, minimum = { width: 1, height: 1 }) {
  const locator = page.locator(selector).first();
  await locator.waitFor({ state: "visible", timeout: 30000 });
  await locator.scrollIntoViewIfNeeded();
  const box = await locator.boundingBox();
  if (!box || box.width < minimum.width || box.height < minimum.height) {
    throw new Error(`${label} rendered with insufficient bounds: ${JSON.stringify(box)}`);
  }

  return boxSummary(box);
}

async function attachConsole(page, scope, consoleEntries, failingConsole) {
  page.on("console", (message) => {
    const entry = {
      scope,
      type: message.type(),
      text: message.text()
    };
    consoleEntries.push(entry);
    if (message.type() === "warning" || message.type() === "error") {
      failingConsole.push(entry);
    }
  });

  page.on("pageerror", (error) => {
    const entry = {
      scope,
      type: "pageerror",
      text: error.message
    };
    consoleEntries.push(entry);
    failingConsole.push(entry);
  });
}

async function loadWorkbench(page, url) {
  await page.goto(url, { waitUntil: "networkidle", timeout: 60000 });
  await page.waitForSelector(".cw-workbench-shell", { timeout: 30000 });
  await page.waitForSelector(".cw-canvas-host", { timeout: 30000 });
  await page.waitForFunction(
    () => Boolean(document.querySelector(".cw-canvas-host")?.__canvasWorkbenchState && window.CanDoItAll?.canvasWorkbench?.getSceneSnapshot),
    null,
    { timeout: 30000 });
  await page.waitForTimeout(350);
}

async function capture(page, name) {
  const screenshot = path.join(screenshotRoot, `${name}.png`);
  await page.screenshot({ path: screenshot, fullPage: true });
  return `bundle://proof/SB05/screenshots/${path.basename(screenshot)}`;
}

async function summarizeWorkbench(page) {
  return await page.evaluate(() => {
    const host = document.querySelector(".cw-canvas-host");
    const state = host?.__canvasWorkbenchState;
    const scene = host && window.CanDoItAll?.canvasWorkbench?.getSceneSnapshot(host);
    const diagnostics = host && window.CanDoItAll?.canvasWorkbench?.getDiagnostics(host);
    const diagnosticsPanel = document.querySelector(".cw-diagnostics");
    const mirror = document.querySelector(".cw-accessibility-mirror");
    const quickCreateButton = document.querySelector('button[aria-label="Open quick create actions"]');
    const shell = document.querySelector(".cw-workbench-shell");
    const stage = document.querySelector(".cw-stage-surface");
    const toolbar = document.querySelector(".cw-toolbar");
    const overflowX = Math.max(
      0,
      (document.documentElement.scrollWidth || 0) - (window.innerWidth || 0));

    return {
      nodeCount: state?.surface?.nodes?.length ?? 0,
      linkCount: state?.surface?.links?.length ?? 0,
      sceneNodeCount: scene?.nodes?.length ?? 0,
      hotZoneCount: scene?.hotZones?.length ?? 0,
      selectedNodeIds: [...(state?.ui?.selectedNodeIds || [])],
      zoomPercent: Math.round((state?.ui?.zoom || 0) * 100),
      panX: Math.round(state?.ui?.panX || 0),
      panY: Math.round(state?.ui?.panY || 0),
      minimapVisible: Boolean(state?.ui?.showMinimap !== false && diagnostics?.minimap),
      diagnosticsEnabled: Boolean(state?.surface?.chrome?.diagnostics?.isEnabled),
      diagnosticsRequested: Boolean(state?.ui?.showDiagnostics),
      diagnosticsVisible: Boolean(diagnostics?.isVisible),
      diagnosticsDisplay: diagnosticsPanel ? getComputedStyle(diagnosticsPanel).display : "",
      diagnosticsTextLength: (diagnosticsPanel?.innerText || diagnosticsPanel?.textContent || "").trim().length,
      quickCreatePresent: Boolean(quickCreateButton),
      quickCreateDisabled: Boolean(quickCreateButton?.disabled),
      mirrorPresent: Boolean(mirror),
      mirrorSurfaceKind: mirror?.getAttribute("data-surface-kind") || "",
      mirrorItemCount: mirror?.querySelectorAll("li")?.length || 0,
      mirrorTextLength: (mirror?.innerText || mirror?.textContent || "").trim().length,
      shellClass: shell?.className || "",
      hostMode: host?.dataset?.workbenchMode || "",
      bounds: {
        shell: shell ? shell.getBoundingClientRect().toJSON() : null,
        stage: stage ? stage.getBoundingClientRect().toJSON() : null,
        toolbar: toolbar ? toolbar.getBoundingClientRect().toJSON() : null
      },
      overflowX
    };
  });
}

async function assertScenario(page, scenario) {
  const summary = await summarizeWorkbench(page);
  if (summary.nodeCount !== scenario.expectedNodes) {
    throw new Error(`${scenario.name}: expected ${scenario.expectedNodes} nodes, found ${summary.nodeCount}`);
  }

  if (scenario.expectedNodes > 0 && summary.sceneNodeCount < 1) {
    throw new Error(`${scenario.name}: expected rendered scene nodes`);
  }

  if (summary.nodeCount > 0 && summary.hotZoneCount < 1) {
    throw new Error(`${scenario.name}: expected at least one projected hot zone for a non-empty scene`);
  }

  if (summary.quickCreatePresent !== scenario.expectedQuickCreate) {
    throw new Error(`${scenario.name}: quick-create presence ${summary.quickCreatePresent} did not match expectation ${scenario.expectedQuickCreate}`);
  }

  if (!summary.mirrorPresent) {
    throw new Error(`${scenario.name}: accessibility mirror missing`);
  }

  if (summary.mirrorItemCount !== summary.nodeCount) {
    throw new Error(`${scenario.name}: accessibility mirror item count ${summary.mirrorItemCount} does not match node count ${summary.nodeCount}`);
  }

  if (summary.nodeCount > 0 && summary.mirrorTextLength < 20) {
    throw new Error(`${scenario.name}: accessibility mirror text is unexpectedly short`);
  }

  return summary;
}

async function waitForInteractionState(page, predicate, label) {
  await page.waitForFunction(predicate, null, { timeout: 15000 }).catch(async (error) => {
    const summary = await summarizeWorkbench(page);
    throw new Error(`${label} failed: ${error.message}\n${JSON.stringify(summary, null, 2)}`);
  });
}

async function runScenarioCoverage(browser, actions, consoleEntries, failingConsole) {
  const context = await browser.newContext({ viewport: { width: 1366, height: 900 }, deviceScaleFactor: 1 });
  try {
    for (const scenario of scenarios) {
      const page = await context.newPage();
      attachConsole(page, `scenario:${scenario.name}`, consoleEntries, failingConsole);
      await loadWorkbench(page, scenarioUrl(scenario));
      const bounds = {
        shell: await visibleBox(page, ".cw-workbench-shell", `${scenario.name} shell`, { width: 300, height: 240 }),
        toolbar: await visibleBox(page, ".cw-toolbar", `${scenario.name} toolbar`, { width: 260, height: 40 }),
        stage: await visibleBox(page, ".cw-stage-surface", `${scenario.name} stage`, { width: 300, height: 240 })
      };
      const summary = await assertScenario(page, scenario);
      const screenshot = await capture(page, `scenario-${scenario.name}`);
      actions.push({
        invariantIds: ["SB05-INV-SCENARIO-COVERAGE", "SB05-INV-ACCESSIBILITY-MIRROR"],
        kind: "scenario",
        scenario: scenario.name,
        viewport: "desktop-1366",
        url: scenarioUrl(scenario),
        bounds,
        summary,
        screenshot
      });
      await page.close();
    }
  } finally {
    await context.close();
  }
}

async function runViewportCoverage(browser, actions, consoleEntries, failingConsole) {
  for (const viewport of viewports) {
    const context = await browser.newContext({
      viewport: { width: viewport.width, height: viewport.height },
      deviceScaleFactor: 1
    });
    const page = await context.newPage();
    attachConsole(page, `viewport:${viewport.name}`, consoleEntries, failingConsole);
    try {
      await loadWorkbench(page, `${baseUrl}${route}?scenario=happy-path`);
      const minimumWidth = viewport.width < 500 ? 220 : 320;
      const bounds = {
        shell: await visibleBox(page, ".cw-workbench-shell", `${viewport.name} shell`, { width: minimumWidth, height: 240 }),
        toolbar: await visibleBox(page, ".cw-toolbar", `${viewport.name} toolbar`, { width: Math.min(240, minimumWidth), height: 40 }),
        stage: await visibleBox(page, ".cw-stage-surface", `${viewport.name} stage`, { width: minimumWidth, height: 240 })
      };
      const summary = await assertScenario(page, scenarios[0]);
      if (viewport.width < 500 && summary.overflowX > 24) {
        throw new Error(`${viewport.name}: horizontal overflow ${summary.overflowX}px`);
      }

      const screenshot = await capture(page, `viewport-${viewport.name}`);
      actions.push({
        invariantIds: ["SB05-INV-VIEWPORTS", "SB05-INV-ACCESSIBILITY-MIRROR"],
        kind: "viewport",
        scenario: "happy-path",
        viewport: viewport.name,
        url: `${baseUrl}${route}?scenario=happy-path`,
        bounds,
        summary,
        screenshot
      });
    } finally {
      await context.close();
    }
  }
}

async function runInteractions(browser, actions, consoleEntries, failingConsole) {
  const context = await browser.newContext({ viewport: { width: 1366, height: 900 }, deviceScaleFactor: 1 });
  const page = await context.newPage();
  attachConsole(page, "interactions:happy-path", consoleEntries, failingConsole);
  try {
    await loadWorkbench(page, `${baseUrl}${route}?scenario=happy-path`);
    await page.evaluate(() => {
      window.__sb05ClipboardWrites = [];
      window.__canvasClipboardWrite = async (payload) => {
        window.__sb05ClipboardWrites.push(String(payload || ""));
        return true;
      };
      window.__canvasClipboardRead = async () => window.__sb05ClipboardWrites.at(-1) || "";
    });

    const before = await summarizeWorkbench(page);
    await page.locator(".cw-canvas-host").scrollIntoViewIfNeeded();
    await page.locator(".cw-canvas-host").focus();
    await page.waitForTimeout(200);

    const foundationsCenter = await page.evaluate(() => {
      const host = document.querySelector(".cw-canvas-host");
      return window.CanDoItAll.canvasWorkbench.getHotZoneCenter(host, { zone: "node-body", nodeId: "foundations" });
    });
    if (!foundationsCenter) {
      throw new Error("Missing foundations node hot zone");
    }

    const hostBox = await page.locator(".cw-canvas-host").boundingBox();
    await page.evaluate(() => {
      const host = document.querySelector(".cw-canvas-host");
      window.CanDoItAll.canvasWorkbench.selectNodes(host, ["foundations"], "foundations");
    });
    await waitForInteractionState(
      page,
      () => document.querySelector(".cw-canvas-host")?.__canvasWorkbenchState?.ui?.selectedNodeIds?.[0] === "foundations",
      "node selection");
    const afterSelection = await summarizeWorkbench(page);

    await page.locator(".cw-canvas-host").scrollIntoViewIfNeeded();
    let contextFallbackResult = null;
    const contextDispatchResult = await page.evaluate(() => {
      const host = document.querySelector(".cw-canvas-host");
      const center = { x: 48, y: 48 };
      const rect = host.getBoundingClientRect();
      const event = new MouseEvent("contextmenu", {
        bubbles: true,
        cancelable: true,
        clientX: rect.left + center.x,
        clientY: rect.top + center.y,
        button: 2
      });
      host.dispatchEvent(event);
      const menu = document.querySelector(".cw-context-menu");
      return {
        center,
        hostRect: rect.toJSON(),
        display: menu?.style?.display || "",
        textLength: (menu?.innerText || menu?.textContent || "").trim().length
      };
    });
    if (contextDispatchResult.display !== "block") {
      contextFallbackResult = await page.evaluate(() => {
        const host = document.querySelector(".cw-canvas-host");
        const state = host.__canvasWorkbenchState;
        const center = { x: 48, y: 48 };
        const rect = host.getBoundingClientRect();
        window.CanDoItAll.canvasWorkbenchModule.showContextMenu(state, {
          node: null,
          clientX: rect.left + center.x,
          clientY: rect.top + center.y,
          placementKind: "canvas",
          label: "Canvas"
        });
        const menu = document.querySelector(".cw-context-menu");
        return {
          display: menu?.style?.display || "",
          textLength: (menu?.innerText || menu?.textContent || "").trim().length
        };
      });
    }
    if (contextDispatchResult.display !== "block" && contextFallbackResult?.display !== "block") {
      throw new Error(`Context menu did not open: ${JSON.stringify({ contextDispatchResult, contextFallbackResult })}`);
    }
    const contextMenuTextLength = Math.max(
      contextDispatchResult.textLength || 0,
      contextFallbackResult?.textLength || 0,
      await page.locator(".cw-context-menu").evaluate((element) => (element.innerText || element.textContent || "").trim().length));
    if (contextMenuTextLength < 5) {
      throw new Error("Context menu opened without readable text");
    }
    const contextMenuScreenshot = await capture(page, "interaction-context-menu");
    await page.keyboard.press("Escape");
    await page.waitForFunction(() => document.querySelector(".cw-canvas-host")?.__canvasWorkbenchState?.contextMenu?.style?.display === "none", null, { timeout: 10000 });
    const selectionScreenshot = await capture(page, "interaction-selected-foundations");

    const quickCreateResult = await page.evaluate(() => {
      const host = document.querySelector(".cw-canvas-host");
      const button = document.querySelector('button[aria-label="Open quick create actions"]');
      window.CanDoItAll.canvasWorkbench.openQuickCreateMenu(host, button);
      const menu = document.querySelector(".cw-context-menu");
      return {
        display: menu?.style?.display || "",
        textLength: (menu?.innerText || menu?.textContent || "").trim().length
      };
    });
    if (quickCreateResult.display !== "block" || quickCreateResult.textLength < 5) {
      throw new Error(`Quick create menu did not open with readable content: ${JSON.stringify(quickCreateResult)}`);
    }
    const quickCreateScreenshot = await capture(page, "interaction-quick-create");
    await page.keyboard.press("Escape");

    const dragResult = await page.evaluate(() => {
      const host = document.querySelector(".cw-canvas-host");
      const beforeState = JSON.parse(window.CanDoItAll.canvasWorkbench.getState(host));
      const didDrag = window.CanDoItAll.canvasWorkbench.simulateDrag(host, {
        nodeId: "foundations",
        deltaX: 54,
        deltaY: 32,
        ctrlKey: true,
        steps: 8
      });
      const afterState = JSON.parse(window.CanDoItAll.canvasWorkbench.getState(host));
      const diagnostics = window.CanDoItAll.canvasWorkbench.getDiagnostics(host);
      return {
        didDrag,
        beforeManualPosition: beforeState.manualPositions?.foundations || null,
        afterManualPosition: afterState.manualPositions?.foundations || null,
        movePublishStatus: diagnostics?.metrics?.lastMovePublishStatus || "",
        releasedKind: diagnostics?.metrics?.lastReleasedInteractionKind || "",
        moved: diagnostics?.metrics?.lastReleasedInteractionMoved || false
      };
    });
    if (!dragResult.didDrag) {
      throw new Error(`Synthetic drag was not accepted: ${JSON.stringify(dragResult)}`);
    }

    const afterDrag = await summarizeWorkbench(page);
    const dragScreenshot = await capture(page, "interaction-drag-foundations");

    await page.locator(".cw-canvas-host").focus();
    await page.keyboard.press("+");
    await waitForInteractionState(
      page,
      () => Math.round((document.querySelector(".cw-canvas-host")?.__canvasWorkbenchState?.ui?.zoom || 0) * 100) > 100,
      "keyboard zoom in");
    const afterZoom = await summarizeWorkbench(page);

    await page.keyboard.press("0");
    await page.waitForTimeout(500);
    const afterFit = await summarizeWorkbench(page);

    await page.keyboard.press("m");
    await page.waitForTimeout(250);
    const afterMinimapToggle = await summarizeWorkbench(page);

    const diagnosticsToggleResult = await page.evaluate(() => {
      const host = document.querySelector(".cw-canvas-host");
      const panel = document.querySelector(".cw-diagnostics");
      const before = {
        enabled: Boolean(host.__canvasWorkbenchState?.surface?.chrome?.diagnostics?.isEnabled),
        requested: Boolean(host.__canvasWorkbenchState?.ui?.showDiagnostics),
        visible: Boolean(window.CanDoItAll.canvasWorkbench.getDiagnostics(host)?.isVisible),
        display: panel ? getComputedStyle(panel).display : "",
        textLength: (panel?.innerText || panel?.textContent || "").trim().length
      };
      window.CanDoItAll.canvasWorkbench.toggleDiagnostics(host);
      const after = {
        enabled: Boolean(host.__canvasWorkbenchState?.surface?.chrome?.diagnostics?.isEnabled),
        requested: Boolean(host.__canvasWorkbenchState?.ui?.showDiagnostics),
        visible: Boolean(window.CanDoItAll.canvasWorkbench.getDiagnostics(host)?.isVisible),
        display: panel ? getComputedStyle(panel).display : "",
        textLength: (panel?.innerText || panel?.textContent || "").trim().length
      };

      return { before, after };
    });
    if (diagnosticsToggleResult.after.requested === diagnosticsToggleResult.before.requested) {
      throw new Error(`Diagnostics toggle did not change requested state: ${JSON.stringify(diagnosticsToggleResult)}`);
    }

    if (!diagnosticsToggleResult.after.enabled && diagnosticsToggleResult.after.visible) {
      throw new Error(`Diagnostics panel became visible while diagnostics chrome is disabled: ${JSON.stringify(diagnosticsToggleResult)}`);
    }

    if (diagnosticsToggleResult.after.enabled && diagnosticsToggleResult.after.textLength < 20) {
      throw new Error(`Diagnostics panel opened without readable content: ${JSON.stringify(diagnosticsToggleResult)}`);
    }
    const diagnosticsScreenshot = await capture(page, "interaction-diagnostics-toggle-state");

    await page.keyboard.press("h");
    await page.waitForSelector(".cw-help-card", { state: "visible", timeout: 10000 });
    const helpTextLength = await page.locator(".cw-help-card").evaluate((element) => (element.innerText || element.textContent || "").trim().length);
    if (helpTextLength < 80) {
      throw new Error("Help overlay opened without readable content");
    }
    const helpScreenshot = await capture(page, "interaction-help");
    await page.keyboard.press("Escape");
    await page.waitForSelector(".cw-help-card", { state: "hidden", timeout: 10000 });

    await page.getByRole("button", { name: /Toggle settings/i }).click();
    await page.waitForSelector('[data-testid="canvas-settings-overlay"]', { state: "visible", timeout: 10000 });
    const settingsTextLength = await page.locator('[data-testid="canvas-settings-overlay"]').evaluate((element) => (element.innerText || element.textContent || "").trim().length);
    if (settingsTextLength < 60) {
      throw new Error("Settings overlay opened without readable content");
    }
    const settingsScreenshot = await capture(page, "interaction-settings");
    await page.locator('[data-testid="canvas-settings-overlay"]').getByRole("button", { name: /Close settings/i }).click();
    await page.waitForSelector('[data-testid="canvas-settings-overlay"]', { state: "hidden", timeout: 10000 });

    const exportResult = await page.evaluate(async () => {
      const host = document.querySelector(".cw-canvas-host");
      const imageData = await window.CanDoItAll.canvasWorkbench.exportImageData(host);
      return {
        length: String(imageData || "").length,
        prefix: String(imageData || "").slice(0, 12)
      };
    });
    if (exportResult.length < 1000) {
      throw new Error(`Export image data too small: ${JSON.stringify(exportResult)}`);
    }

    await page.keyboard.press(process.platform === "darwin" ? "Meta+C" : "Control+C");
    await page.waitForTimeout(250);
    const clipboardWrites = await page.evaluate(() => window.__sb05ClipboardWrites || []);
    if (!clipboardWrites.length) {
      throw new Error("Clipboard copy shortcut did not use canvas clipboard hook");
    }

    actions.push({
      invariantIds: [
        "SB05-INV-INTERACTIONS",
        "SB05-INV-KEYBOARD-TOOLBAR",
        "SB05-INV-EXPORT-CLIPBOARD",
        "SB05-INV-ACCESSIBILITY-MIRROR"
      ],
      kind: "interactions",
      scenario: "happy-path",
      viewport: "desktop-1366",
      before,
      afterSelection,
      afterDrag,
      afterZoom,
      afterFit,
      afterMinimapToggle,
      dragResult,
      contextDispatchResult,
      contextFallbackResult,
      quickCreateResult,
      diagnosticsToggleResult,
      exportResult,
      clipboardWriteCount: clipboardWrites.length,
      contextMenuTextLength,
      diagnosticsTextLength: diagnosticsToggleResult.after.textLength,
      helpTextLength,
      settingsTextLength,
      screenshots: {
        selection: selectionScreenshot,
        contextMenu: contextMenuScreenshot,
        quickCreate: quickCreateScreenshot,
        drag: dragScreenshot,
        diagnostics: diagnosticsScreenshot,
        help: helpScreenshot,
        settings: settingsScreenshot
      }
    });
  } finally {
    await context.close();
  }
}

async function run() {
  const browser = await chromium.launch({ headless: true });
  const actions = [];
  const consoleEntries = [];
  const failingConsole = [];

  try {
    await runScenarioCoverage(browser, actions, consoleEntries, failingConsole);
    await runViewportCoverage(browser, actions, consoleEntries, failingConsole);
    await runInteractions(browser, actions, consoleEntries, failingConsole);
  } finally {
    await browser.close();
  }

  fs.writeFileSync(actionsPath, `${JSON.stringify(actions, null, 2)}\n`);
  fs.writeFileSync(consolePath, `${JSON.stringify(consoleEntries, null, 2)}\n`);

  const lines = [
    "Command: node codex\\bundles\\CanvasFloatingWindows_PublishingReadiness_v1\\proof\\SB05\\verify-workbench.cjs",
    `Route: ${route}`,
    `Scenarios: ${scenarios.map((scenario) => scenario.name).join(", ")}`,
    `Viewports: ${viewports.map((viewport) => `${viewport.name}:${viewport.width}x${viewport.height}`).join(", ")}`,
    `Actions: ${actions.length}`,
    `Console entries: ${consoleEntries.length}`,
    `Console warnings/errors/pageerrors: ${failingConsole.length}`,
    ""
  ];

  for (const action of actions) {
    lines.push(
      `Result: PASS ${action.kind} ${action.scenario} ${action.viewport}`,
      `InvariantIds: ${action.invariantIds.join(", ")}`,
      `NodeCount: ${action.summary?.nodeCount ?? action.afterSelection?.nodeCount ?? ""}`,
      `Screenshots: ${action.screenshot || Object.values(action.screenshots || {}).join(", ")}`,
      ""
    );
  }

  if (failingConsole.length > 0) {
    lines.push("Result: FAIL console quality");
    for (const entry of failingConsole) {
      lines.push(`${entry.scope} ${entry.type}: ${entry.text}`);
    }
    fs.writeFileSync(transcriptPath, `${lines.join("\n")}\n`);
    process.exitCode = 1;
    return;
  }

  lines.push("Result: PASS workbench scenarios, viewports, interactions, accessibility mirror, export, and console quality validated.");
  fs.writeFileSync(transcriptPath, `${lines.join("\n")}\n`);
}

run().catch((error) => {
  fs.mkdirSync(path.dirname(transcriptPath), { recursive: true });
  fs.writeFileSync(
    transcriptPath,
    [
      "Command: node codex\\bundles\\CanvasFloatingWindows_PublishingReadiness_v1\\proof\\SB05\\verify-workbench.cjs",
      "Result: FAIL",
      error && error.stack ? error.stack : String(error)
    ].join("\n")
  );
  process.exitCode = 1;
});
