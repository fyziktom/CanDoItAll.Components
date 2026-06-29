const fs = require("fs");
const path = require("path");
const { chromium } = require("playwright");

const root = path.resolve(__dirname);
const screenshotsDir = path.join(root, "screenshots");
fs.mkdirSync(screenshotsDir, { recursive: true });

const viewports = [
  { name: "max-desktop", width: 1920, height: 1080 },
  { name: "desktop-1366", width: 1366, height: 900 },
  { name: "mobile-390", width: 390, height: 844 }
];

function assert(condition, message) {
  if (!condition) {
    throw new Error(message);
  }
}

async function measure(page) {
  return page.evaluate(() => {
    const frame = document.querySelector("[data-testid='sandbox-overlay-window-frame']");
    const safeTop = document.querySelector("[data-testid='sandbox-overlay-window-safe-top']");
    const win = document.querySelector("[data-testid='sandbox-overlay-window']");
    if (!frame || !safeTop || !win) {
      return null;
    }

    const f = frame.getBoundingClientRect();
    const s = safeTop.getBoundingClientRect();
    const w = win.getBoundingClientRect();
    const actions = [...win.querySelectorAll("button")].map(button => ({
      label: button.getAttribute("aria-label") || button.textContent.trim(),
      rect: button.getBoundingClientRect().toJSON()
    }));

    return {
      frame: f.toJSON(),
      safeTop: s.toJSON(),
      window: w.toJSON(),
      className: win.className,
      text: win.textContent.replace(/\s+/g, " ").trim(),
      actions
    };
  });
}

function assertWindowInsideFrame(snapshot, label) {
  assert(snapshot, `${label}: missing overlay window snapshot`);
  const { frame, safeTop, window: win, actions } = snapshot;
  assert(win.left >= frame.left - 1, `${label}: window left escapes frame`);
  assert(win.right <= frame.right + 1, `${label}: window right escapes frame`);
  assert(win.top >= safeTop.bottom - frame.top - 8 || win.top >= frame.top, `${label}: window top violates safe-top/frame`);
  assert(win.bottom <= frame.bottom + 1, `${label}: window bottom escapes frame`);
  assert(actions.length > 0, `${label}: no header actions found`);
  for (const action of actions) {
    assert(action.rect.width >= 28 && action.rect.height >= 28, `${label}: action ${action.label} is too small`);
  }
  assert(snapshot.text.includes("Inspector window"), `${label}: title missing`);
}

async function screenshot(page, viewportName, stateName) {
  const file = path.join(screenshotsDir, `${viewportName}-${stateName}.png`);
  await page.screenshot({ path: file, fullPage: true });
  return `bundle://proof/SB02/screenshots/${path.basename(file)}`;
}

async function runViewport(browser, viewport) {
  const context = await browser.newContext({ viewport: { width: viewport.width, height: viewport.height } });
  const page = await context.newPage();
  const consoleMessages = [];
  const pageErrors = [];
  page.on("console", message => consoleMessages.push(`${message.type()}: ${message.text()}`));
  page.on("pageerror", error => pageErrors.push(error.stack || error.message));

  await page.goto("http://127.0.0.1:5088/groups/overlays", { waitUntil: "networkidle" });
  await page.locator("[data-testid='sandbox-overlay-window']").waitFor({ state: "visible", timeout: 15000 });

  const evidence = [];
  let snapshot = await measure(page);
  assertWindowInsideFrame(snapshot, `${viewport.name} initial`);
  evidence.push({ viewport: viewport.name, state: "initial", screenshot: await screenshot(page, viewport.name, "initial"), snapshot });

  await page.getByLabel("Minimize window").click();
  await page.locator("[data-testid='sandbox-overlay-window'].is-minimized").waitFor({ state: "visible", timeout: 5000 });
  snapshot = await measure(page);
  assert(snapshot.className.includes("is-minimized"), `${viewport.name}: minimized class missing`);
  assertWindowInsideFrame(snapshot, `${viewport.name} minimized`);
  evidence.push({ viewport: viewport.name, state: "minimized", screenshot: await screenshot(page, viewport.name, "minimized"), snapshot });

  await page.getByLabel("Expand window").click();
  await page.locator("[data-testid='sandbox-overlay-window']:not(.is-minimized)").waitFor({ state: "visible", timeout: 5000 });
  snapshot = await measure(page);
  assertWindowInsideFrame(snapshot, `${viewport.name} restored`);
  evidence.push({ viewport: viewport.name, state: "restored", screenshot: await screenshot(page, viewport.name, "restored"), snapshot });

  const dragHandle = page.locator("[data-testid='sandbox-overlay-window'] [data-cda-overlay-drag]").first();
  const dragBox = await dragHandle.boundingBox();
  assert(dragBox, `${viewport.name}: drag handle missing`);
  await page.mouse.move(dragBox.x + dragBox.width / 2, dragBox.y + dragBox.height / 2);
  await page.mouse.down();
  await page.mouse.move(dragBox.x + dragBox.width / 2 - 80, dragBox.y + dragBox.height / 2 + 70, { steps: 8 });
  await page.mouse.up();
  await page.waitForTimeout(250);
  snapshot = await measure(page);
  assertWindowInsideFrame(snapshot, `${viewport.name} dragged`);
  evidence.push({ viewport: viewport.name, state: "dragged", screenshot: await screenshot(page, viewport.name, "dragged"), snapshot });

  const windowBox = await page.locator("[data-testid='sandbox-overlay-window']").boundingBox();
  assert(windowBox, `${viewport.name}: window missing before resize`);
  await page.mouse.move(windowBox.x + windowBox.width - 3, windowBox.y + windowBox.height - 3);
  await page.mouse.down();
  await page.mouse.move(windowBox.x + windowBox.width + 35, windowBox.y + windowBox.height + 30, { steps: 6 });
  await page.mouse.up();
  await page.waitForTimeout(350);
  snapshot = await measure(page);
  assertWindowInsideFrame(snapshot, `${viewport.name} resized`);
  evidence.push({ viewport: viewport.name, state: "resized", screenshot: await screenshot(page, viewport.name, "resized"), snapshot });

  await page.getByLabel("Hide window").click();
  await page.locator("[data-testid='sandbox-overlay-window']").waitFor({ state: "detached", timeout: 5000 });
  evidence.push({ viewport: viewport.name, state: "hidden", screenshot: await screenshot(page, viewport.name, "hidden") });

  await page.locator("[data-testid='show-overlay-window']").click();
  await page.locator("[data-testid='sandbox-overlay-window']").waitFor({ state: "visible", timeout: 5000 });
  snapshot = await measure(page);
  assertWindowInsideFrame(snapshot, `${viewport.name} shown`);
  evidence.push({ viewport: viewport.name, state: "shown", screenshot: await screenshot(page, viewport.name, "shown"), snapshot });

  const unexpectedConsole = consoleMessages.filter(line => /^(warning|error):/i.test(line));
  assert(pageErrors.length === 0, `${viewport.name}: page errors: ${pageErrors.join("\n")}`);
  assert(unexpectedConsole.length === 0, `${viewport.name}: console messages: ${unexpectedConsole.join("\n")}`);

  await context.close();
  return { viewport, evidence, consoleMessages, pageErrors };
}

(async () => {
  const browser = await chromium.launch({ headless: true });
  try {
    const results = [];
    for (const viewport of viewports) {
      results.push(await runViewport(browser, viewport));
    }

    fs.writeFileSync(path.join(root, "browser-actions.json"), JSON.stringify(results, null, 2));
    fs.writeFileSync(path.join(root, "console-log.txt"), results.map(result => {
      return [
        `Viewport: ${result.viewport.name}`,
        ...result.consoleMessages,
        ...result.pageErrors.map(error => `pageerror: ${error}`)
      ].join("\n");
    }).join("\n\n"));
    console.log("SB02 overlay browser validation passed");
  }
  finally {
    await browser.close();
  }
})().catch(error => {
  console.error(error);
  process.exit(1);
});
