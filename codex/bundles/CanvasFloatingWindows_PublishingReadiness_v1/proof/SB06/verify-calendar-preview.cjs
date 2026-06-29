const fs = require("node:fs");
const path = require("node:path");
const { chromium } = require("playwright");

const baseUrl = process.env.CANVAS_CALENDAR_BASE_URL || "http://127.0.0.1:5088";
const route = "/groups/canvas";
const proofRoot = __dirname;
const screenshotRoot = path.join(proofRoot, "screenshots");
const transcriptPath = path.join(proofRoot, "transcripts", "playwright-calendar-preview.txt");
const actionsPath = path.join(proofRoot, "browser-actions.json");
const consolePath = path.join(proofRoot, "console-log.txt");

fs.mkdirSync(screenshotRoot, { recursive: true });
fs.mkdirSync(path.dirname(transcriptPath), { recursive: true });

const scenarios = [
  { name: "happy-path", query: "scenario=happy-path", expectedEvents: 2, expectedInitialView: "day", expectedTimeGrid: true, allowCreate: true },
  { name: "dense-content", query: "scenario=dense-content", expectedEvents: 3, expectedInitialView: "week", expectedTimeGrid: true, allowCreate: true },
  { name: "empty-state", query: "scenario=empty-state", expectedEvents: 0, expectedInitialView: "day", expectedTimeGrid: false, allowCreate: true },
  { name: "disabled-state", query: "scenario=disabled-state", expectedEvents: 2, expectedInitialView: "day", expectedTimeGrid: true, allowCreate: false },
  { name: "long-text", query: "scenario=long-text", expectedEvents: 2, expectedInitialView: "day", expectedTimeGrid: true, allowCreate: true }
];

const viewports = [
  { name: "max-desktop", width: 1920, height: 1080 },
  { name: "desktop-1366", width: 1366, height: 900 },
  { name: "mobile-390", width: 390, height: 844 }
];

const calendarBoundaryIds = [
  "calendar-selection-panel",
  "calendar-export-menu",
  "calendar-mini-month-navigator",
  "calendar-time-grid-renderer"
];

const corePreviewIds = [
  "canvas-scene-host",
  "layer-stack",
  "viewport-controller-preview",
  "serialization-persistence-pack",
  "text-measure-service-preview",
  "js-interop-bridge",
  "command-history-store",
  "animation-timeline-preview"
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

async function loadCalendar(page, url) {
  await page.goto(url, { waitUntil: "networkidle", timeout: 60000 });
  await page.waitForSelector(".cdi-canvas-calendar-host", { timeout: 30000 });
  await page.waitForSelector(".zy-calendar-canvas", { timeout: 30000 });
  await page.waitForFunction(
    () => Boolean(window.CanDoItAll?.canvasCalendar && window.ZyCanvasCalendar && window.ZyCanvasPrimitives),
    null,
    { timeout: 30000 });
  await page.waitForFunction(
    () => getComputedStyle(document.querySelector(".zy-calendar-loading")).display === "none",
    null,
    { timeout: 30000 });
  await page.waitForTimeout(350);
}

async function capture(page, name) {
  const screenshot = path.join(screenshotRoot, `${name}.png`);
  await page.screenshot({ path: screenshot, fullPage: true });
  return `bundle://proof/SB06/screenshots/${path.basename(screenshot)}`;
}

async function summarizeCalendar(page) {
  return await page.evaluate(({ boundaryIds, previewIds }) => {
    const host = document.querySelector(".cdi-canvas-calendar-host");
    const shell = document.querySelector(".cdi-canvas-calendar-shell");
    const canvas = document.querySelector(".zy-calendar-canvas");
    const panel = document.querySelector(".zy-calendar-panel");
    const listShell = document.querySelector(".zy-calendar-list-shell");
    const utility = document.querySelector('[data-role="utility-backdrop"]');
    const editor = document.querySelector('[data-role="modal-backdrop"]');
    const choice = document.querySelector('[data-role="playlist-choice-backdrop"]');
    const toolbarMenu = document.querySelector('[data-role="toolbar-menu-popover"]');
    const mirror = document.querySelector('[data-testid="calendar-accessibility-mirror-layer"]');
    const periodSubtitle = document.querySelector('[data-role="period-subtitle"]');
    const periodLabel = document.querySelector('[data-role="period-label"]');
    const activeView = document.querySelector('[data-role="view-switcher"] .is-active');
    const selectedPanelTitle = document.querySelector('[data-role="panel-title"]');
    const exportStatus = document.querySelector('[data-testid="calendar-export-status"]');
    const listRows = document.querySelectorAll(".zy-calendar-list-table tbody tr");
    const addEventButton = document.querySelector('[data-action="add-event"]');
    const overflowX = Math.max(
      0,
      (document.documentElement.scrollWidth || 0) - (window.innerWidth || 0));

    const text = (element) => (element?.innerText || element?.textContent || "").trim().replace(/\s+/g, " ");
    const box = (element) => element ? element.getBoundingClientRect().toJSON() : null;
    const visible = (element) => {
      if (!element) {
        return false;
      }

      const style = getComputedStyle(element);
      return style.display !== "none" && style.visibility !== "hidden" && Number(style.opacity || "1") !== 0;
    };

    const boundaryCards = Object.fromEntries(boundaryIds.map((id) => {
      const element = document.querySelector(`[data-testid="${id}"]`);
      return [id, {
        present: Boolean(element),
        textLength: text(element).length,
        bounds: box(element)
      }];
    }));

    const previewCards = Object.fromEntries(previewIds.map((id) => {
      const element = document.querySelector(`[data-testid="${id}"]`);
      return [id, {
        present: Boolean(element),
        textLength: text(element).length,
        bounds: box(element)
      }];
    }));

    const visibleMatch = text(periodSubtitle).match(/(\d+)\s+visible events/i);

    return {
      facadePresent: Boolean(window.CanDoItAll?.canvasCalendar),
      primitiveRuntimePresent: Boolean(window.ZyCanvasPrimitives),
      controllerRuntimePresent: Boolean(window.ZyCanvasCalendar),
      hostTextLength: text(host).length,
      periodLabel: text(periodLabel),
      periodSubtitle: text(periodSubtitle),
      visibleEventCount: visibleMatch ? Number(visibleMatch[1]) : null,
      selectedPanelTitle: text(selectedPanelTitle),
      activeView: text(activeView),
      mobileViewValue: document.querySelector('[data-role="mobile-view-select"]')?.value || "",
      listVisible: visible(listShell),
      listRows: listRows.length,
      addEventPresent: Boolean(addEventButton),
      mirrorPresent: Boolean(mirror),
      mirrorItemCount: mirror?.querySelectorAll("li")?.length || 0,
      mirrorTextLength: text(mirror).length,
      exportStatus: text(exportStatus),
      utilityOpen: utility?.classList.contains("is-open") || false,
      utilityTitle: text(document.querySelector('[data-role="utility-title"]')),
      editorOpen: editor?.classList.contains("is-open") || false,
      editorDisplay: editor ? getComputedStyle(editor).display : "",
      editorTitle: text(document.querySelector('[data-role="editor-title"]')),
      choiceOpen: choice?.classList.contains("is-open") || false,
      toolbarMenuVisible: visible(toolbarMenu),
      boundaryCards,
      previewCards,
      bounds: {
        shell: box(shell),
        host: box(host),
        canvas: box(canvas),
        panel: box(panel)
      },
      overflowX
    };
  }, { boundaryIds: calendarBoundaryIds, previewIds: corePreviewIds });
}

async function waitForCalendarState(page, predicate, label) {
  await page.waitForFunction(predicate, null, { timeout: 15000 }).catch(async (error) => {
    const summary = await summarizeCalendar(page);
    throw new Error(`${label} failed: ${error.message}\n${JSON.stringify(summary, null, 2)}`);
  });
}

function assertCardMap(cards, ids, label, options = {}) {
  const missing = [];
  const short = [];
  for (const id of ids) {
    const card = cards[id];
    if (!card?.present) {
      missing.push(id);
      continue;
    }

    if ((card.textLength || 0) < (options.minimumTextLength || 40)) {
      short.push(`${id}:${card.textLength || 0}`);
    }
  }

  if (missing.length || short.length) {
    throw new Error(`${label} card assertion failed. Missing: ${missing.join(", ")} Short: ${short.join(", ")}`);
  }
}

async function assertScenario(page, scenario) {
  const summary = await summarizeCalendar(page);
  if (!summary.facadePresent || !summary.primitiveRuntimePresent || !summary.controllerRuntimePresent) {
    throw new Error(`${scenario.name}: calendar runtime facades missing: ${JSON.stringify(summary)}`);
  }

  if (summary.visibleEventCount !== scenario.expectedEvents) {
    throw new Error(`${scenario.name}: expected ${scenario.expectedEvents} visible events, found ${summary.visibleEventCount}`);
  }

  if (summary.activeView.toLowerCase() !== scenario.expectedInitialView) {
    throw new Error(`${scenario.name}: expected initial view ${scenario.expectedInitialView}, found ${summary.activeView}`);
  }

  if (summary.addEventPresent !== scenario.allowCreate) {
    throw new Error(`${scenario.name}: add event presence ${summary.addEventPresent} did not match allowCreate ${scenario.allowCreate}`);
  }

  if (!summary.mirrorPresent) {
    throw new Error(`${scenario.name}: calendar accessibility mirror missing`);
  }

  if (summary.mirrorItemCount !== scenario.expectedEvents) {
    throw new Error(`${scenario.name}: mirror count ${summary.mirrorItemCount} did not match event count ${scenario.expectedEvents}`);
  }

  if (scenario.expectedEvents > 0 && summary.mirrorTextLength < 80) {
    throw new Error(`${scenario.name}: mirror text unexpectedly short`);
  }

  const expectedBoundaryIds = scenario.expectedTimeGrid
    ? calendarBoundaryIds
    : calendarBoundaryIds.filter((id) => id !== "calendar-time-grid-renderer");
  assertCardMap(summary.boundaryCards, expectedBoundaryIds, `${scenario.name} calendar boundary`, { minimumTextLength: 60 });
  if (!scenario.expectedTimeGrid && summary.boundaryCards["calendar-time-grid-renderer"]?.present) {
    throw new Error(`${scenario.name}: time-grid boundary should not render for empty event surface`);
  }

  assertCardMap(summary.previewCards, corePreviewIds, `${scenario.name} core preview`, { minimumTextLength: 60 });
  return summary;
}

async function runScenarioCoverage(browser, actions, consoleEntries, failingConsole) {
  const context = await browser.newContext({ viewport: { width: 1366, height: 900 }, deviceScaleFactor: 1 });
  try {
    for (const scenario of scenarios) {
      const page = await context.newPage();
      attachConsole(page, `scenario:${scenario.name}`, consoleEntries, failingConsole);
      await loadCalendar(page, scenarioUrl(scenario));
      const bounds = {
        shell: await visibleBox(page, ".cdi-canvas-calendar-shell", `${scenario.name} calendar shell`, { width: 320, height: 360 }),
        host: await visibleBox(page, ".cdi-canvas-calendar-host", `${scenario.name} calendar host`, { width: 300, height: 360 }),
        canvas: await visibleBox(page, ".zy-calendar-canvas", `${scenario.name} calendar canvas`, { width: 220, height: 320 }),
        panel: await visibleBox(page, ".zy-calendar-panel", `${scenario.name} calendar panel`, { width: 220, height: 180 })
      };
      const summary = await assertScenario(page, scenario);
      const screenshot = await capture(page, `scenario-${scenario.name}-calendar-preview`);
      actions.push({
        invariantIds: ["SB06-INV-CALENDAR-SCENARIOS", "SB06-INV-PREVIEW-CARDS", "SB06-INV-CALENDAR-A11Y"],
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
      await loadCalendar(page, `${baseUrl}${route}?scenario=happy-path`);
      const minimumWidth = viewport.width < 500 ? 220 : 320;
      const bounds = {
        shell: await visibleBox(page, ".cdi-canvas-calendar-shell", `${viewport.name} calendar shell`, { width: minimumWidth, height: 360 }),
        host: await visibleBox(page, ".cdi-canvas-calendar-host", `${viewport.name} calendar host`, { width: minimumWidth, height: 360 }),
        canvas: await visibleBox(page, ".zy-calendar-canvas", `${viewport.name} calendar canvas`, { width: viewport.width < 500 ? 180 : 220, height: 300 })
      };
      const summary = await assertScenario(page, scenarios[0]);
      if (viewport.width < 500 && summary.overflowX > 24) {
        throw new Error(`${viewport.name}: horizontal overflow ${summary.overflowX}px`);
      }

      const screenshot = await capture(page, `viewport-${viewport.name}-calendar-preview`);
      actions.push({
        invariantIds: ["SB06-INV-CALENDAR-VIEWPORTS", "SB06-INV-PREVIEW-CARDS", "SB06-INV-CALENDAR-A11Y"],
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

async function runCalendarInteractions(browser, actions, consoleEntries, failingConsole) {
  const context = await browser.newContext({ viewport: { width: 1366, height: 900 }, deviceScaleFactor: 1 });
  const page = await context.newPage();
  attachConsole(page, "interactions:happy-path", consoleEntries, failingConsole);
  try {
    await loadCalendar(page, `${baseUrl}${route}?scenario=happy-path`);
    await page.locator(".cdi-canvas-calendar-shell").scrollIntoViewIfNeeded();
    await page.evaluate(() => { window.confirm = () => true; });

    const before = await summarizeCalendar(page);

    await page.locator('.cdi-canvas-calendar-host [data-action="next"]').click();
    await waitForCalendarState(
      page,
      () => !document.querySelector('[data-role="period-label"]')?.textContent?.includes("Apr 6, 2026"),
      "calendar next range");
    const afterNext = await summarizeCalendar(page);

    await page.locator('.cdi-canvas-calendar-host [data-action="previous"]').click();
    await waitForCalendarState(
      page,
      () => document.querySelector('[data-role="period-label"]')?.textContent?.includes("Apr 6, 2026"),
      "calendar previous range");

    await page.locator('.cdi-canvas-calendar-host [data-view="week"]').click();
    await waitForCalendarState(
      page,
      () => document.querySelector('[data-role="view-switcher"] .is-active')?.textContent?.trim() === "Week",
      "calendar week view");
    const weekScreenshot = await capture(page, "action-week-view");

    await page.locator('.cdi-canvas-calendar-host [data-view="list"]').click();
    await waitForCalendarState(
      page,
      () => document.querySelector('[data-role="view-switcher"] .is-active')?.textContent?.trim() === "List" &&
        document.querySelectorAll(".zy-calendar-list-table tbody tr").length >= 2,
      "calendar list view");
    const listScreenshot = await capture(page, "action-list-view");

    await page.locator('.cdi-canvas-calendar-host [data-action="select-row"][data-event-id="canvas-proof"]').click();
    await waitForCalendarState(
      page,
      () => document.querySelector('[data-role="panel-title"]')?.textContent?.includes("Canvas boundary proof"),
      "calendar list row selection");
    const afterListSelection = await summarizeCalendar(page);

    await page.locator('.cdi-canvas-calendar-host [data-action="open-settings"]').click();
    await waitForCalendarState(
      page,
      () => document.querySelector('[data-role="utility-backdrop"]')?.classList.contains("is-open") &&
        document.querySelector('[data-role="utility-title"]')?.textContent?.includes("Display preferences"),
      "calendar settings open");
    await page.locator('.cdi-canvas-calendar-host [data-role="utility-timezone-input"]').fill("America/New_York");
    const settingsScreenshot = await capture(page, "action-settings-timezone");
    await page.locator('.cdi-canvas-calendar-host [data-action="apply-utility-settings"]').click();
    await waitForCalendarState(
      page,
      () => !document.querySelector('[data-role="utility-backdrop"]')?.classList.contains("is-open") &&
        document.querySelector('[data-role="period-subtitle"]')?.textContent?.includes("America/New_York"),
      "calendar timezone apply");
    const afterTimezone = await summarizeCalendar(page);

    await page.locator('.cdi-canvas-calendar-host [data-action="open-help"]').click();
    await waitForCalendarState(
      page,
      () => document.querySelector('[data-role="utility-backdrop"]')?.classList.contains("is-open") &&
        document.querySelector('[data-role="utility-title"]')?.textContent?.includes("Using the canvas"),
      "calendar help open");
    const helpTextLength = await page.locator('.cdi-canvas-calendar-host [data-role="utility-backdrop"]').evaluate((element) => (element.innerText || element.textContent || "").trim().length);
    if (helpTextLength < 120) {
      throw new Error(`Calendar help content too short: ${helpTextLength}`);
    }
    const helpScreenshot = await capture(page, "action-help");
    await page.locator('.cdi-canvas-calendar-host [data-action="close-utility"]').click();
    await waitForCalendarState(
      page,
      () => !document.querySelector('[data-role="utility-backdrop"]')?.classList.contains("is-open"),
      "calendar help close");

    await page.locator('.cdi-canvas-calendar-host [data-view="week"]').click();
    await waitForCalendarState(
      page,
      () => document.querySelector('[data-role="view-switcher"] .is-active')?.textContent?.trim() === "Week" &&
        /[1-9]\d*\s+visible events/i.test(document.querySelector('[data-role="period-subtitle"]')?.textContent || ""),
      "calendar export week scope");

    await page.locator('.cdi-canvas-calendar-host [data-action="toggle-export-menu"]').click();
    await waitForCalendarState(
      page,
      () => getComputedStyle(document.querySelector('[data-role="toolbar-menu-popover"]')).display !== "none",
      "calendar export menu open");
    const exportMenuScreenshot = await capture(page, "action-export-menu");
    await page.locator('.cdi-canvas-calendar-host [data-action="export-csv"]').click();
    await waitForCalendarState(
      page,
      () => /CSV \([1-9]\d* events\)/i.test(document.querySelector('[data-testid="calendar-export-status"]')?.textContent || ""),
      "calendar csv export callback");
    const afterExport = await summarizeCalendar(page);

    await page.locator('.cdi-canvas-calendar-host [data-action="edit-selected"]').click();
    await waitForCalendarState(
      page,
      () => document.querySelector('[data-role="modal-backdrop"]')?.classList.contains("is-open") &&
        document.querySelector('[data-role="editor-title"]')?.textContent?.includes("Canvas boundary proof"),
      "calendar editor open");
    await page.locator('.cdi-canvas-calendar-host [data-role="editor-playlist-search"]').fill("Release");
    await waitForCalendarState(
      page,
      () => document.querySelector('[data-role="editor-playlist-results"]')?.textContent?.includes("Release proof playlist"),
      "calendar playlist search");
    await page.locator('.cdi-canvas-calendar-host [data-action="link-playlist"][data-playlist-id="playlist-release-proof"]').click();
    await waitForCalendarState(
      page,
      () => document.querySelector('[data-role="playlist-choice-backdrop"]')?.classList.contains("is-open"),
      "calendar playlist choice open");
    const playlistChoiceScreenshot = await capture(page, "action-playlist-choice");
    await page.locator('.cdi-canvas-calendar-host [data-action="playlist-choice-copy"]').click();
    await waitForCalendarState(
      page,
      () => document.querySelector('[data-role="editor-message"]')?.textContent?.includes("Playlist copy created") &&
        document.querySelector('[data-role="editor-playlists"]')?.textContent?.includes("copy"),
      "calendar playlist clone");
    const playlistCloneScreenshot = await capture(page, "action-playlist-clone");

    await page.locator('.cdi-canvas-calendar-host [data-role="editor-title-input"]').fill("SB06 updated calendar proof");
    await page.locator('.cdi-canvas-calendar-host [data-role="editor-form"] button[type="submit"]').click();
    await waitForCalendarState(
      page,
      () => !document.querySelector('[data-role="modal-backdrop"]')?.classList.contains("is-open") &&
        document.querySelector('[data-role="panel-title"]')?.textContent?.includes("SB06 updated calendar proof"),
      "calendar update save");
    const afterUpdate = await summarizeCalendar(page);
    const updateScreenshot = await capture(page, "action-updated-event");

    await page.locator('.cdi-canvas-calendar-host [data-action="add-event"]').click();
    await waitForCalendarState(
      page,
      () => document.querySelector('[data-role="modal-backdrop"]')?.classList.contains("is-open") &&
        document.querySelector('[data-role="editor-title"]')?.textContent?.includes("Create event"),
      "calendar create editor open");
    await page.locator('.cdi-canvas-calendar-host [data-role="editor-title-input"]').fill("SB06 transient event");
    await page.locator('.cdi-canvas-calendar-host [data-role="editor-form"] button[type="submit"]').click();
    await waitForCalendarState(
      page,
      () => !document.querySelector('[data-role="modal-backdrop"]')?.classList.contains("is-open") &&
        document.querySelector('[data-role="panel-title"]')?.textContent?.includes("SB06 transient event"),
      "calendar create save");
    const afterCreate = await summarizeCalendar(page);
    const createScreenshot = await capture(page, "action-created-event");

    await page.locator('.cdi-canvas-calendar-host [data-action="delete-selected"]').click();
    await waitForCalendarState(
      page,
      () => !document.querySelector('[data-role="panel-title"]')?.textContent?.includes("SB06 transient event"),
      "calendar delete selected");
    const afterDelete = await summarizeCalendar(page);
    const deleteScreenshot = await capture(page, "action-deleted-event");

    actions.push({
      invariantIds: [
        "SB06-INV-CALENDAR-ACTIONS",
        "SB06-INV-CALENDAR-CRUD",
        "SB06-INV-CALENDAR-EXPORT",
        "SB06-INV-PLAYLISTS"
      ],
      kind: "interactions",
      scenario: "happy-path",
      viewport: "desktop-1366",
      before,
      afterNext,
      afterListSelection,
      afterTimezone,
      afterExport,
      afterUpdate,
      afterCreate,
      afterDelete,
      screenshots: {
        week: weekScreenshot,
        list: listScreenshot,
        settings: settingsScreenshot,
        help: helpScreenshot,
        exportMenu: exportMenuScreenshot,
        playlistChoice: playlistChoiceScreenshot,
        playlistClone: playlistCloneScreenshot,
        update: updateScreenshot,
        create: createScreenshot,
        delete: deleteScreenshot
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
    await runCalendarInteractions(browser, actions, consoleEntries, failingConsole);
  } finally {
    await browser.close();
  }

  fs.writeFileSync(actionsPath, `${JSON.stringify(actions, null, 2)}\n`);
  fs.writeFileSync(consolePath, `${JSON.stringify(consoleEntries, null, 2)}\n`);

  const lines = [
    "Command: node codex\\bundles\\CanvasFloatingWindows_PublishingReadiness_v1\\proof\\SB06\\verify-calendar-preview.cjs",
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
      `VisibleEvents: ${action.summary?.visibleEventCount ?? action.before?.visibleEventCount ?? ""}`,
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

  lines.push("Result: PASS calendar scenarios, viewports, CRUD, playlist, export, preview cards, accessibility mirror, and console quality validated.");
  fs.writeFileSync(transcriptPath, `${lines.join("\n")}\n`);
}

run().catch((error) => {
  fs.mkdirSync(path.dirname(transcriptPath), { recursive: true });
  fs.writeFileSync(
    transcriptPath,
    [
      "Command: node codex\\bundles\\CanvasFloatingWindows_PublishingReadiness_v1\\proof\\SB06\\verify-calendar-preview.cjs",
      "Result: FAIL",
      error && error.stack ? error.stack : String(error)
    ].join("\n")
  );
  process.exitCode = 1;
});
