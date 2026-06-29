const fs = require("node:fs");
const path = require("node:path");
const { chromium } = require("playwright");

const baseUrl = process.env.FLOATING_WINDOW_BASE_URL || "http://127.0.0.1:5088";
const proofRoot = __dirname;
const screenshotRoot = path.join(proofRoot, "screenshots");
const transcriptPath = path.join(proofRoot, "transcripts", "playwright-floating-windows.txt");
const actionsPath = path.join(proofRoot, "browser-actions.json");
const consolePath = path.join(proofRoot, "console-log.txt");

fs.mkdirSync(screenshotRoot, { recursive: true });
fs.mkdirSync(path.dirname(transcriptPath), { recursive: true });

const viewports = [
  { name: "max-desktop", width: 1920, height: 1080 },
  { name: "desktop-1366", width: 1366, height: 900 },
  { name: "tablet-1024", width: 1024, height: 768 },
  { name: "mobile-390", width: 390, height: 844 }
];

const surfaces = [
  {
    name: "overlay",
    route: "/groups/overlays?scenario=long-text",
    windowSelector: "[data-testid='sandbox-overlay-window']",
    frameSelector: "[data-testid='sandbox-overlay-window-frame']",
    captureSelector: "[data-testid='sandbox-overlay-window-frame']",
    safeTopSelector: "[data-testid='sandbox-overlay-window-safe-top']",
    dragSelector: "[data-testid='sandbox-overlay-window'] [data-cda-overlay-drag]",
    showSelector: "[data-testid='show-overlay-window']",
    resetSelector: "[data-testid='reset-overlay-window']",
    expectedTitle: "Inspector window",
    invariantIds: ["SB07-INV-OVERLAY-LIFECYCLE", "SB07-INV-SAFE-TOP-CONTAINER"]
  },
  {
    name: "canvas",
    route: "/groups/canvas?scenario=long-text",
    windowSelector: "[data-testid='sandbox-canvas-inspector']",
    frameSelector: ".cw-stage-surface",
    captureSelector: ".cw-workbench-shell",
    safeTopSelector: ".cw-toolbar",
    dragSelector: "[data-testid='sandbox-canvas-inspector'] [data-cw-window-drag]",
    showSelector: "[data-testid='show-canvas-inspector']",
    resetSelector: "[data-testid='reset-canvas-inspector']",
    expectedTitle: "Selection context",
    invariantIds: ["SB07-INV-CANVAS-LIFECYCLE", "SB07-INV-RUNTIME-OWNERSHIP", "SB07-INV-SAFE-TOP-CONTAINER"]
  }
];

function scenarioUrl(surface) {
  return `${baseUrl}${surface.route}`;
}

function roundedBox(box) {
  if (!box) {
    return null;
  }

  return {
    x: Math.round(box.x),
    y: Math.round(box.y),
    width: Math.round(box.width),
    height: Math.round(box.height),
    top: Math.round(box.top),
    right: Math.round(box.right),
    bottom: Math.round(box.bottom),
    left: Math.round(box.left)
  };
}

function attachConsole(page, scope, consoleEntries, failingConsole) {
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

async function capture(page, name, surface) {
  const screenshot = path.join(screenshotRoot, `${name}.png`);
  await page.locator(surface.captureSelector || surface.frameSelector).first().screenshot({ path: screenshot });
  return `bundle://proof/SB07/screenshots/${path.basename(screenshot)}`;
}

async function loadSurface(page, surface) {
  await page.goto(scenarioUrl(surface), { waitUntil: "networkidle", timeout: 60000 });
  await page.locator(surface.frameSelector).first().waitFor({ state: "visible", timeout: 30000 });
  await page.locator(surface.safeTopSelector).first().waitFor({ state: "visible", timeout: 30000 });
  if (surface.name === "canvas") {
    await page.waitForSelector(".cw-workbench-shell", { timeout: 30000 });
    await page.waitForSelector(".cw-canvas-host", { timeout: 30000 });
    await page.waitForFunction(
      () => Boolean(document.querySelector(".cw-canvas-host")?.__canvasWorkbenchState),
      null,
      { timeout: 30000 });
  }

  await page.locator(surface.windowSelector).first().waitFor({ state: "visible", timeout: 30000 });
  await page.waitForFunction(
    () => Boolean(window.CanDoItAll?.overlayWindow && window.CanDoItAll?.canvasFloatingWindow),
    null,
    { timeout: 30000 });
  await page.waitForTimeout(350);
}

async function measureWindow(page, surface) {
  await page.locator(surface.frameSelector).first().scrollIntoViewIfNeeded();
  await page.locator(surface.windowSelector).first().scrollIntoViewIfNeeded();
  await page.waitForTimeout(100);
  return await page.evaluate(({ surface }) => {
    const frame = document.querySelector(surface.frameSelector);
    const safeTop = document.querySelector(surface.safeTopSelector);
    const win = document.querySelector(surface.windowSelector);
    const overflowX = Math.max(0, (document.documentElement.scrollWidth || 0) - (window.innerWidth || 0));

    const text = (element) => (element?.innerText || element?.textContent || "").trim().replace(/\s+/g, " ");
    const box = (element) => element ? element.getBoundingClientRect().toJSON() : null;
    const style = win ? getComputedStyle(win) : null;

    return {
      api: {
        overlayPresent: Boolean(window.CanDoItAll?.overlayWindow),
        canvasPresent: Boolean(window.CanDoItAll?.canvasFloatingWindow),
        canvasAliasesOverlay: window.CanDoItAll?.canvasFloatingWindow === window.CanDoItAll?.overlayWindow
      },
      frame: box(frame),
      safeTop: box(safeTop),
      window: box(win),
      className: win?.className || "",
      text: text(win),
      overflowX,
      resize: style?.resize || "",
      zIndex: style?.zIndex || "",
      actions: [...(win?.querySelectorAll("button") || [])].map((button) => ({
        label: button.getAttribute("aria-label") || text(button),
        disabled: Boolean(button.disabled),
        rect: box(button)
      }))
    };
  }, { surface });
}

function assertWindow(snapshot, label, options = {}) {
  if (!snapshot?.frame || !snapshot?.safeTop || !snapshot?.window) {
    throw new Error(`${label}: missing frame, safe-top, or window snapshot`);
  }

  const frame = snapshot.frame;
  const safeTop = snapshot.safeTop;
  const win = snapshot.window;
  const tolerance = options.mobile ? 5 : 2;

  if (!snapshot.api.overlayPresent || !snapshot.api.canvasPresent) {
    throw new Error(`${label}: floating-window runtime facade missing`);
  }

  if (!snapshot.api.canvasAliasesOverlay) {
    throw new Error(`${label}: canvasFloatingWindow is not aliasing overlayWindow`);
  }

  if (win.width < (snapshot.className.includes("is-minimized") ? 120 : 180)) {
    throw new Error(`${label}: window width collapsed to ${win.width}`);
  }

  if (win.height < (snapshot.className.includes("is-minimized") ? 42 : 120)) {
    throw new Error(`${label}: window height collapsed to ${win.height}`);
  }

  if (win.left < frame.left - tolerance) {
    throw new Error(`${label}: window left escapes frame: ${win.left} < ${frame.left}`);
  }

  if (win.right > frame.right + tolerance) {
    throw new Error(`${label}: window right escapes frame: ${win.right} > ${frame.right}`);
  }

  const safeTopFloor = Math.max(frame.top, safeTop.bottom + (options.canvas ? 4 : 0));
  if (win.top < safeTopFloor - tolerance) {
    throw new Error(`${label}: window top ${win.top} violates safe top ${safeTopFloor}`);
  }

  if (win.bottom > frame.bottom + tolerance) {
    throw new Error(`${label}: window bottom escapes frame: ${win.bottom} > ${frame.bottom}`);
  }

  if (snapshot.overflowX > 24) {
    throw new Error(`${label}: lateral overflow ${snapshot.overflowX}px`);
  }

  if (!snapshot.text.includes(options.expectedTitle)) {
    throw new Error(`${label}: missing expected title ${options.expectedTitle}`);
  }

  if (snapshot.actions.length < 2) {
    throw new Error(`${label}: missing header action buttons`);
  }

  for (const action of snapshot.actions) {
    if (!action.rect || action.rect.width < 28 || action.rect.height < 28) {
      throw new Error(`${label}: action ${action.label} has insufficient hit area`);
    }
  }
}

async function dragWindow(page, surface, viewport) {
  const handle = page.locator(surface.dragSelector).first();
  await handle.waitFor({ state: "visible", timeout: 10000 });
  const box = await handle.boundingBox();
  if (!box) {
    throw new Error(`${surface.name} ${viewport.name}: drag handle missing`);
  }

  const deltaX = viewport.width < 720 ? -32 : -92;
  const deltaY = viewport.width < 720 ? 38 : 72;
  await page.mouse.move(box.x + box.width / 2, box.y + box.height / 2);
  await page.mouse.down();
  await page.mouse.move(box.x + box.width / 2 + deltaX, box.y + box.height / 2 + deltaY, { steps: 8 });
  await page.mouse.up();
  await page.waitForTimeout(300);
}

async function resizeWindow(page, surface, viewport) {
  const target = page.locator(surface.windowSelector).first();
  const box = await target.boundingBox();
  if (!box) {
    throw new Error(`${surface.name} ${viewport.name}: window missing before resize`);
  }

  const delta = viewport.width < 720 ? 10 : 42;
  await page.mouse.move(box.x + box.width - 3, box.y + box.height - 3);
  await page.mouse.down();
  await page.mouse.move(box.x + box.width + delta, box.y + box.height + delta, { steps: 6 });
  await page.mouse.up();
  await page.waitForTimeout(400);
}

async function runSurfaceViewport(browser, surface, viewport, consoleEntries, failingConsole) {
  const context = await browser.newContext({ viewport: { width: viewport.width, height: viewport.height } });
  const page = await context.newPage();
  attachConsole(page, `${surface.name}:${viewport.name}`, consoleEntries, failingConsole);

  const records = [];
  const mobile = viewport.width < 720;
  const assertOptions = {
    mobile,
    canvas: surface.name === "canvas",
    expectedTitle: surface.expectedTitle
  };

  await loadSurface(page, surface);
  let snapshot = await measureWindow(page, surface);
  assertWindow(snapshot, `${surface.name} ${viewport.name} initial`, assertOptions);
  records.push({
    surface: surface.name,
    viewport: viewport.name,
    state: "initial",
    invariantIds: surface.invariantIds,
    snapshot,
    screenshot: await capture(page, `${surface.name}-${viewport.name}-initial`, surface)
  });

  await page.getByLabel("Minimize window").first().click();
  await page.locator(`${surface.windowSelector}.is-minimized`).waitFor({ state: "visible", timeout: 10000 });
  snapshot = await measureWindow(page, surface);
  if (!snapshot.className.includes("is-minimized")) {
    throw new Error(`${surface.name} ${viewport.name}: minimized class missing`);
  }

  assertWindow(snapshot, `${surface.name} ${viewport.name} minimized`, assertOptions);
  records.push({
    surface: surface.name,
    viewport: viewport.name,
    state: "minimized",
    invariantIds: surface.invariantIds,
    snapshot,
    screenshot: await capture(page, `${surface.name}-${viewport.name}-minimized`, surface)
  });

  await page.getByLabel("Expand window").first().click();
  await page.locator(`${surface.windowSelector}:not(.is-minimized)`).waitFor({ state: "visible", timeout: 10000 });
  snapshot = await measureWindow(page, surface);
  assertWindow(snapshot, `${surface.name} ${viewport.name} restored`, assertOptions);
  records.push({
    surface: surface.name,
    viewport: viewport.name,
    state: "restored",
    invariantIds: surface.invariantIds,
    snapshot,
    screenshot: await capture(page, `${surface.name}-${viewport.name}-restored`, surface)
  });

  const beforeDrag = snapshot;
  await dragWindow(page, surface, viewport);
  snapshot = await measureWindow(page, surface);
  assertWindow(snapshot, `${surface.name} ${viewport.name} dragged`, assertOptions);
  const dragMoved = Math.abs(snapshot.window.left - beforeDrag.window.left) >= (mobile ? 1 : 8)
    || Math.abs(snapshot.window.top - beforeDrag.window.top) >= (mobile ? 1 : 8);
  if (!dragMoved) {
    throw new Error(`${surface.name} ${viewport.name}: drag did not move the window`);
  }

  records.push({
    surface: surface.name,
    viewport: viewport.name,
    state: "dragged",
    invariantIds: surface.invariantIds,
    snapshot,
    screenshot: await capture(page, `${surface.name}-${viewport.name}-dragged`, surface)
  });

  const beforeResize = snapshot;
  await resizeWindow(page, surface, viewport);
  snapshot = await measureWindow(page, surface);
  assertWindow(snapshot, `${surface.name} ${viewport.name} resized`, assertOptions);
  const resizeChanged = Math.abs(snapshot.window.width - beforeResize.window.width) >= (mobile ? 0 : 4)
    || Math.abs(snapshot.window.height - beforeResize.window.height) >= (mobile ? 0 : 4);
  if (!mobile && !resizeChanged) {
    throw new Error(`${surface.name} ${viewport.name}: resize did not change desktop geometry`);
  }

  records.push({
    surface: surface.name,
    viewport: viewport.name,
    state: "resized",
    invariantIds: surface.invariantIds,
    snapshot,
    screenshot: await capture(page, `${surface.name}-${viewport.name}-resized`, surface)
  });

  await page.locator(surface.resetSelector).click();
  await page.waitForTimeout(500);
  snapshot = await measureWindow(page, surface);
  assertWindow(snapshot, `${surface.name} ${viewport.name} reset`, assertOptions);
  records.push({
    surface: surface.name,
    viewport: viewport.name,
    state: "reset",
    invariantIds: surface.invariantIds,
    snapshot,
    screenshot: await capture(page, `${surface.name}-${viewport.name}-reset`, surface)
  });

  await page.getByLabel("Hide window").first().click();
  await page.locator(surface.windowSelector).waitFor({ state: "detached", timeout: 10000 });
  records.push({
    surface: surface.name,
    viewport: viewport.name,
    state: "hidden",
    invariantIds: surface.invariantIds,
    screenshot: await capture(page, `${surface.name}-${viewport.name}-hidden`, surface)
  });

  await page.locator(surface.showSelector).click();
  await page.locator(surface.windowSelector).waitFor({ state: "visible", timeout: 10000 });
  await page.waitForTimeout(300);
  snapshot = await measureWindow(page, surface);
  assertWindow(snapshot, `${surface.name} ${viewport.name} shown`, assertOptions);
  records.push({
    surface: surface.name,
    viewport: viewport.name,
    state: "shown",
    invariantIds: surface.invariantIds,
    snapshot,
    screenshot: await capture(page, `${surface.name}-${viewport.name}-shown`, surface)
  });

  await context.close();
  return records;
}

function writeOutputs(actions, consoleEntries, failingConsole) {
  fs.writeFileSync(actionsPath, JSON.stringify(actions, null, 2));
  fs.writeFileSync(consolePath, consoleEntries.map((entry) => {
    return `${entry.scope} ${entry.type}: ${entry.text}`;
  }).join("\n"));

  const lines = [
    "Command: node codex\\bundles\\CanvasFloatingWindows_PublishingReadiness_v1\\proof\\SB07\\verify-floating-windows.cjs",
    "Routes: /groups/overlays, /groups/canvas",
    `Viewports: ${viewports.map((viewport) => `${viewport.name}:${viewport.width}x${viewport.height}`).join(", ")}`,
    `Surfaces: ${surfaces.map((surface) => surface.name).join(", ")}`,
    `Actions: ${actions.length}`,
    `Console entries: ${consoleEntries.length}`,
    `Console warnings/errors/pageerrors: ${failingConsole.length}`,
    ""
  ];

  for (const surface of surfaces) {
    for (const viewport of viewports) {
      const records = actions.filter((record) => record.surface === surface.name && record.viewport === viewport.name);
      lines.push(`Result: PASS ${surface.name} ${viewport.name}`);
      lines.push(`InvariantIds: ${surface.invariantIds.join(", ")}`);
      lines.push(`States: ${records.map((record) => record.state).join(", ")}`);
      const initial = records.find((record) => record.state === "initial")?.snapshot;
      if (initial) {
        lines.push(`RuntimeAlias: ${initial.api.canvasAliasesOverlay}`);
        lines.push(`InitialBounds: ${JSON.stringify({
          frame: roundedBox(initial.frame),
          safeTop: roundedBox(initial.safeTop),
          window: roundedBox(initial.window),
          overflowX: initial.overflowX
        })}`);
      }

      lines.push(`Screenshots: ${records.map((record) => record.screenshot).join(", ")}`);
      lines.push("");
    }
  }

  lines.push("Result: PASS overlay and canvas floating windows validated for lifecycle, safe-top/container bounds, runtime ownership alias, viewport containment, and console quality.");
  fs.writeFileSync(transcriptPath, lines.join("\n"));
  console.log(lines.join("\n"));
}

(async () => {
  const browser = await chromium.launch({ headless: true });
  const consoleEntries = [];
  const failingConsole = [];
  const actions = [];

  try {
    for (const surface of surfaces) {
      for (const viewport of viewports) {
        actions.push(...await runSurfaceViewport(browser, surface, viewport, consoleEntries, failingConsole));
        console.log(`Completed ${surface.name} ${viewport.name}`);
      }
    }

    if (failingConsole.length > 0) {
      throw new Error(`Unexpected console failures:\n${JSON.stringify(failingConsole, null, 2)}`);
    }

    writeOutputs(actions, consoleEntries, failingConsole);
  }
  finally {
    await browser.close();
  }
})().catch((error) => {
  fs.writeFileSync(transcriptPath, `Result: FAIL\n${error.stack || error.message}`);
  console.error(error);
  process.exit(1);
});
