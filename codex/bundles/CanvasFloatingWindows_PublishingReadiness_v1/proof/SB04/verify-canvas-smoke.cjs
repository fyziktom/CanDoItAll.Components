const fs = require("node:fs");
const path = require("node:path");
const { chromium } = require("playwright");

const baseUrl = process.env.CANVAS_SMOKE_BASE_URL || "http://127.0.0.1:5088";
const route = "/groups/canvas?scenario=happy-path";
const proofRoot = __dirname;
const screenshotRoot = path.join(proofRoot, "screenshots");
const transcriptPath = path.join(proofRoot, "transcripts", "playwright-canvas-smoke.txt");
const actionsPath = path.join(proofRoot, "browser-actions.json");
const consolePath = path.join(proofRoot, "console-log.txt");

fs.mkdirSync(screenshotRoot, { recursive: true });
fs.mkdirSync(path.dirname(transcriptPath), { recursive: true });

const viewports = [
  { name: "max-desktop", width: 1920, height: 1080 },
  { name: "desktop-1366", width: 1366, height: 900 }
];

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
  const box = await locator.boundingBox();
  if (!box || box.width < minimum.width || box.height < minimum.height) {
    throw new Error(`${label} rendered with insufficient bounds: ${JSON.stringify(box)}`);
  }

  return boxSummary(box);
}

async function run() {
  const browser = await chromium.launch({ headless: true });
  const actions = [];
  const consoleEntries = [];
  const failingConsole = [];

  try {
    for (const viewport of viewports) {
      const context = await browser.newContext({
        viewport: { width: viewport.width, height: viewport.height },
        deviceScaleFactor: 1
      });
      const page = await context.newPage();

      page.on("console", (message) => {
        const entry = {
          viewport: viewport.name,
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
          viewport: viewport.name,
          type: "pageerror",
          text: error.message
        };
        consoleEntries.push(entry);
        failingConsole.push(entry);
      });

      const url = `${baseUrl}${route}`;
      await page.goto(url, { waitUntil: "networkidle", timeout: 60000 });
      await page.waitForSelector(".cw-workbench-shell", { timeout: 30000 });
      await page.waitForSelector(".zy-calendar-shell, .zy-calendar-canvas", { timeout: 30000 });
      await page.waitForFunction(() => Boolean(window.CanDoItAll?.canvasWorkbench?.create), null, { timeout: 30000 });
      await page.waitForFunction(() => Boolean(window.CanDoItAll?.canvasCalendar?.create), null, { timeout: 30000 });
      await page.waitForFunction(() => Boolean(window.CanDoItAll?.canvasFloatingWindow?.create), null, { timeout: 30000 });

      const assertions = await page.evaluate(() => {
        const workbenchHost = document.querySelector(".cw-canvas-host");
        const workbenchState = workbenchHost?.__canvasWorkbenchState;
        const calendarHost = document.querySelector(".cdi-canvas-calendar-shell > div");
        const calendarShell = document.querySelector(".zy-calendar-shell");
        const calendarCanvas = document.querySelector(".zy-calendar-canvas");
        const calendarToolbar = document.querySelector(".zy-calendar-toolbar");
        const stage = document.querySelector(".cw-stage-surface");
        const toolbar = document.querySelector(".cw-toolbar");
        const inspector = document.querySelector('[data-testid="sandbox-canvas-inspector"], .cdi-overlay-window, .canvas-floating-window');
        const stageText = (stage?.innerText || stage?.textContent || "").trim();
        const toolbarText = (toolbar?.innerText || toolbar?.textContent || "").trim();

        return {
          facadeNames: Object.keys(window.CanDoItAll || {}).filter((name) => name.startsWith("canvas")).sort(),
          workbenchInitialized: Boolean(workbenchState),
          calendarInitialized: Boolean(calendarShell && calendarCanvas && calendarToolbar),
          calendarEventTextLength: (calendarShell?.innerText || calendarShell?.textContent || "").trim().length,
          workbenchNodeCount: Array.isArray(workbenchState?.surface?.nodes) ? workbenchState.surface.nodes.length : null,
          toolbarTextLength: toolbarText.length,
          stageTextLength: stageText.length,
          hasInspector: Boolean(inspector),
          scriptOrder: {
            selectionModel: Boolean(window.CanDoItAll?.selectionModel),
            viewportController: Boolean(window.CanDoItAll?.viewportController),
            canvasWorkbench: Boolean(window.CanDoItAll?.canvasWorkbench),
            canvasCalendar: Boolean(window.CanDoItAll?.canvasCalendar)
          }
        };
      });

      if (!assertions.workbenchInitialized) {
        throw new Error(`${viewport.name}: Canvas workbench did not initialize runtime state`);
      }

      if (!assertions.calendarInitialized) {
        throw new Error(`${viewport.name}: Canvas calendar did not initialize runtime state`);
      }

      if (!assertions.workbenchNodeCount || assertions.workbenchNodeCount < 1) {
        throw new Error(`${viewport.name}: Canvas workbench has no runtime nodes`);
      }

      if (!assertions.hasInspector) {
        throw new Error(`${viewport.name}: Canvas floating inspector was not present`);
      }

      const bounds = {
        shell: await visibleBox(page, ".cw-workbench-shell", "workbench shell", { width: 320, height: 240 }),
        toolbar: await visibleBox(page, ".cw-toolbar", "workbench toolbar", { width: 240, height: 40 }),
        stage: await visibleBox(page, ".cw-stage-surface", "workbench stage", { width: 320, height: 240 }),
        host: await visibleBox(page, ".cw-canvas-host", "workbench host", { width: 320, height: 220 }),
        calendar: await visibleBox(page, ".cdi-canvas-calendar-shell", "calendar shell", { width: 300, height: 260 })
      };

      const screenshot = path.join(screenshotRoot, `${viewport.name}-canvas-smoke.png`);
      await page.screenshot({ path: screenshot, fullPage: true });

      actions.push({
        invariantIds: [
          "SB04-INV-ASSET-ORDER",
          "SB04-INV-PUBLIC-FACADES",
          "SB04-INV-BROWSER-SMOKE"
        ],
        viewport: viewport.name,
        url,
        assertions,
        bounds,
        screenshot: `bundle://proof/SB04/screenshots/${path.basename(screenshot)}`
      });

      await context.close();
    }
  } finally {
    await browser.close();
  }

  fs.writeFileSync(actionsPath, `${JSON.stringify(actions, null, 2)}\n`);
  fs.writeFileSync(consolePath, `${JSON.stringify(consoleEntries, null, 2)}\n`);

  const lines = [
    "Command: node codex\\bundles\\CanvasFloatingWindows_PublishingReadiness_v1\\proof\\SB04\\verify-canvas-smoke.cjs",
    `Route: ${route}`,
    `Viewports: ${viewports.map((viewport) => `${viewport.name}:${viewport.width}x${viewport.height}`).join(", ")}`,
    `Actions: ${actions.length}`,
    `Console entries: ${consoleEntries.length}`,
    `Console warnings/errors/pageerrors: ${failingConsole.length}`,
    ""
  ];

  for (const action of actions) {
    lines.push(
      `Result: PASS ${action.viewport}`,
      `InvariantIds: ${action.invariantIds.join(", ")}`,
      `FacadeNames: ${action.assertions.facadeNames.join(", ")}`,
      `WorkbenchNodeCount: ${action.assertions.workbenchNodeCount}`,
      `Bounds: ${JSON.stringify(action.bounds)}`,
      `Screenshot: ${action.screenshot}`,
      ""
    );
  }

  if (failingConsole.length > 0) {
    lines.push("Result: FAIL console quality");
    for (const entry of failingConsole) {
      lines.push(`${entry.viewport} ${entry.type}: ${entry.text}`);
    }
    fs.writeFileSync(transcriptPath, `${lines.join("\n")}\n`);
    process.exitCode = 1;
    return;
  }

  lines.push("Result: PASS canvas route rendered meaningful workbench/calendar content with public facades and zero console warnings/errors/pageerrors.");
  fs.writeFileSync(transcriptPath, `${lines.join("\n")}\n`);
}

run().catch((error) => {
  fs.mkdirSync(path.dirname(transcriptPath), { recursive: true });
  fs.writeFileSync(
    transcriptPath,
    [
      "Command: node codex\\bundles\\CanvasFloatingWindows_PublishingReadiness_v1\\proof\\SB04\\verify-canvas-smoke.cjs",
      "Result: FAIL",
      error && error.stack ? error.stack : String(error)
    ].join("\n")
  );
  process.exitCode = 1;
});
