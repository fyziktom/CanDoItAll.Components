import { chromium } from "playwright";
import { mkdirSync, writeFileSync } from "node:fs";
import { dirname, resolve } from "node:path";

const baseUrl = process.env.SB08_BASE_URL ?? "http://127.0.0.1:5225";
const proofRoot = resolve("codex/bundles/StandardComponents_PublishingReadiness_v1/proof/SB08");
const reportPath = resolve(proofRoot, "data/sb08-layout-navigation-overlays-validation.json");

mkdirSync(dirname(reportPath), { recursive: true });

const browser = await chromium.launch({ channel: "msedge", headless: true });
const context = await browser.newContext();
const page = await context.newPage();
const consoleErrors = [];
const checks = [];

page.on("console", message => {
  if (message.type() === "error") {
    consoleErrors.push(message.text());
  }
});

async function check(condition, name, details = {}) {
  checks.push({ name, passed: Boolean(condition), details });
  if (!condition) {
    throw new Error(`${name} failed: ${JSON.stringify(details)}`);
  }
}

async function goto(path, viewport, readyTestId) {
  await page.setViewportSize(viewport);
  await page.goto(`${baseUrl}${path}`, { waitUntil: "networkidle" });
  await page.locator(`[data-testid='${readyTestId}']`).waitFor({ state: "visible" });
  await page.waitForTimeout(250);
}

async function documentOverflowDetails() {
  return page.evaluate(() => ({
    viewportWidth: window.innerWidth,
    documentScrollWidth: document.documentElement.scrollWidth,
    bodyScrollWidth: document.body.scrollWidth
  }));
}

async function elementMetrics(testId) {
  const selector = `[data-testid='${testId}']`;
  for (let attempt = 0; attempt < 2; attempt++) {
    const locator = page.locator(selector).first();
    await locator.waitFor({ state: "attached" });
    try {
      await locator.scrollIntoViewIfNeeded();
      return locator.evaluate(element => {
        const rect = element.getBoundingClientRect();
        const style = getComputedStyle(element);
        return {
          testId: element.getAttribute("data-testid"),
          tagName: element.tagName.toLowerCase(),
          display: style.display,
          position: style.position,
          overflowX: style.overflowX,
          viewport: {
            width: window.innerWidth,
            height: window.innerHeight
          },
          rect: {
            left: rect.left,
            right: rect.right,
            top: rect.top,
            bottom: rect.bottom,
            width: rect.width,
            height: rect.height
          },
          clientWidth: element.clientWidth,
          scrollWidth: element.scrollWidth
        };
      });
    } catch (error) {
      if (attempt === 1 || !String(error).includes("not attached")) {
        throw error;
      }

      await page.waitForTimeout(250);
    }
  }

  throw new Error(`Unable to measure ${testId}`);
}

async function assertPageNoHorizontalOverflow(path, viewport, readyTestId, componentTestIds) {
  await goto(path, viewport, readyTestId);
  const documentOverflow = await documentOverflowDetails();
  await check(
    documentOverflow.documentScrollWidth <= documentOverflow.viewportWidth + 1,
    `No page horizontal overflow at ${path} ${viewport.width}x${viewport.height}`,
    documentOverflow);

  for (const testId of componentTestIds) {
    const metrics = await elementMetrics(testId);
    const scrollTolerance = metrics.overflowX === "visible" ? 1 : 24;
    await check(
      metrics.scrollWidth <= metrics.clientWidth + scrollTolerance && metrics.rect.right <= metrics.viewport.width + 1 && metrics.rect.left >= -1,
      `No component horizontal overflow for ${testId} at ${path}`,
      { ...metrics, scrollTolerance });
  }
}

async function assertViewportContained(selector, name) {
  const locator = page.locator(selector).first();
  await locator.waitFor({ state: "visible" });
  const metrics = await locator.evaluate((element, selectorValue) => {
    const rect = element.getBoundingClientRect();
    return {
      selector: selectorValue,
      text: element.textContent?.trim(),
      viewport: {
        width: window.innerWidth,
        height: window.innerHeight
      },
      rect: {
        left: rect.left,
        right: rect.right,
        top: rect.top,
        bottom: rect.bottom,
        width: rect.width,
        height: rect.height
      }
    };
  }, selector);

  await check(
    metrics.rect.left >= -1
      && metrics.rect.right <= metrics.viewport.width + 1
      && metrics.rect.top >= -1
      && metrics.rect.bottom <= metrics.viewport.height + 1,
    name,
    metrics);
}

async function assertElementInsideElement(childSelector, parentSelector, name) {
  const child = page.locator(childSelector).first();
  const parent = page.locator(parentSelector).first();
  await child.waitFor({ state: "visible" });
  await parent.waitFor({ state: "visible" });
  const metrics = await page.evaluate(({ childSelectorValue, parentSelectorValue }) => {
    const childElement = document.querySelector(childSelectorValue);
    const parentElement = document.querySelector(parentSelectorValue);
    const childRect = childElement.getBoundingClientRect();
    const parentRect = parentElement.getBoundingClientRect();
    return {
      childSelector: childSelectorValue,
      parentSelector: parentSelectorValue,
      childRect: {
        left: childRect.left,
        right: childRect.right,
        top: childRect.top,
        bottom: childRect.bottom,
        width: childRect.width,
        height: childRect.height
      },
      parentRect: {
        left: parentRect.left,
        right: parentRect.right,
        top: parentRect.top,
        bottom: parentRect.bottom,
        width: parentRect.width,
        height: parentRect.height
      }
    };
  }, { childSelectorValue: childSelector, parentSelectorValue: parentSelector });

  await check(
    metrics.childRect.left >= metrics.parentRect.left - 1
      && metrics.childRect.right <= metrics.parentRect.right + 1
      && metrics.childRect.top >= metrics.parentRect.top - 1
      && metrics.childRect.bottom <= metrics.parentRect.bottom + 1,
    name,
    metrics);
}

try {
  await assertPageNoHorizontalOverflow(
    "/groups/layout?proof=script",
    { width: 1366, height: 900 },
    "sandbox-layout-scaffold",
    [
      "sandbox-demo-surface",
      "sandbox-layout-scaffold",
      "sandbox-layout-primary-grid",
      "sandbox-layout-list-detail",
      "sandbox-layout-sticky-footer"
    ]);

  await assertPageNoHorizontalOverflow(
    "/groups/layout?scenario=long-text&proof=script",
    { width: 390, height: 844 },
    "sandbox-layout-scaffold",
    [
      "sandbox-demo-surface",
      "sandbox-layout-scaffold",
      "sandbox-layout-primary-grid",
      "sandbox-layout-list-detail",
      "sandbox-layout-sticky-footer"
    ]);

  await assertPageNoHorizontalOverflow(
    "/groups/layout/composition?proof=script",
    { width: 1366, height: 900 },
    "layout-composition-stack",
    [
      "layout-composition-stack",
      "layout-composition-grid",
      "layout-composition-row-column-fixed",
      "layout-composition-row-column-responsive"
    ]);

  await assertPageNoHorizontalOverflow(
    "/groups/layout/composition?scenario=long-text&proof=script",
    { width: 390, height: 844 },
    "layout-composition-stack",
    [
      "layout-composition-stack",
      "layout-composition-grid",
      "layout-composition-row-column-fixed",
      "layout-composition-row-column-responsive"
    ]);

  await assertPageNoHorizontalOverflow(
    "/groups/navigation?proof=script",
    { width: 1366, height: 900 },
    "sandbox-navigation-toolbar",
    [
      "sandbox-navigation-basic-tabs",
      "sandbox-navigation-advanced-tabs",
      "sandbox-navigation-toolbar",
      "sandbox-navigation-tree-panel",
      "sandbox-navigation-tree-detail",
      "sandbox-navigation-steps",
      "sandbox-navigation-list-detail"
    ]);

  const basicTabs = page.locator("[data-testid='sandbox-navigation-basic-tabs'] [role='tab']");
  await basicTabs.nth(1).click();
  await page.waitForFunction(() => {
    const selected = document.querySelector("[data-testid='sandbox-navigation-basic-tabs'] [role='tab'][aria-selected='true']");
    return selected?.textContent?.includes("Activity log");
  });
  const selectedTabText = await page.locator("[data-testid='sandbox-navigation-basic-tabs'] [role='tab'][aria-selected='true']").textContent();
  await check(
    selectedTabText?.includes("Activity log"),
    "Navigation tabs switch selected state",
    { selectedTabText });

  await page.locator("[data-testid='sandbox-navigation-steps-next']").click();
  await page.waitForFunction(() => {
    const current = document.querySelector("[data-testid='sandbox-navigation-steps'] [aria-current='step']");
    return current?.textContent?.includes("Proof");
  });
  const currentStepText = await page.locator("[data-testid='sandbox-navigation-steps'] [aria-current='step']").textContent();
  await check(
    currentStepText?.includes("Proof"),
    "Steps bottom next button advances to Proof",
    { currentStepText });

  await page.locator("[data-testid='sandbox-navigation-toolbar-query']").fill("overlay");
  await page.locator("[data-testid='sandbox-navigation-tree-overlay-window']").waitFor({ state: "visible" });
  await check(
    await page.locator("[data-testid='sandbox-navigation-tree-overlay-window']").isVisible(),
    "Toolbar filter exposes matching overlay tree node");

  await page.locator("[data-testid='sandbox-navigation-toolbar-reset']").click();
  await page.getByLabel(/Expand Overlay dependencies/).click();
  await page.locator("[data-testid='sandbox-navigation-tree-context-node']").click({ button: "right" });
  await assertViewportContained(
    "[data-testid='sandbox-navigation-tree-context-menu']",
    "Tree context menu opens on right-click and stays inside the viewport");
  await page.locator("[data-testid='sandbox-navigation-tree-context-close']").click();
  await page.locator("[data-testid='sandbox-navigation-tree-context-menu']").waitFor({ state: "detached" });

  const contextHost = await page.locator("[data-testid='sandbox-navigation-context-menu-host']").evaluate(element => ({
    ariaHidden: element.getAttribute("aria-hidden"),
    hasHostClass: element.classList.contains("rz-context-menu-host"),
    hasHiddenClass: element.classList.contains("hidden")
  }));
  await check(
    contextHost.ariaHidden === "true" && contextHost.hasHostClass && contextHost.hasHiddenClass,
    "ContextMenu host remains mounted as hidden shared infrastructure",
    contextHost);

  await assertPageNoHorizontalOverflow(
    "/groups/navigation?scenario=long-text&proof=script",
    { width: 390, height: 844 },
    "sandbox-navigation-toolbar",
    [
      "sandbox-navigation-basic-tabs",
      "sandbox-navigation-advanced-tabs",
      "sandbox-navigation-toolbar",
      "sandbox-navigation-tree-panel",
      "sandbox-navigation-tree-detail",
      "sandbox-navigation-steps",
      "sandbox-navigation-list-detail"
    ]);

  await assertPageNoHorizontalOverflow(
    "/groups/navigation/tabs?scenario=long-text&proof=script",
    { width: 390, height: 844 },
    "tabs-lab-basic",
    [
      "tabs-lab-basic",
      "tabs-lab-wrap-shell",
      "tabs-lab-scroll-shell",
      "tabs-lab-vertical"
    ]);

  const scrollModeMetrics = await page.locator("[data-testid='tabs-lab-scroll'] .cad-tabs__list").evaluate(element => {
    const style = getComputedStyle(element);
    return {
      overflowX: style.overflowX,
      clientWidth: element.clientWidth,
      scrollWidth: element.scrollWidth
    };
  });
  await check(
    scrollModeMetrics.overflowX !== "visible",
    "Scrollable tabs use an overflow-managed tab list",
    scrollModeMetrics);

  await assertPageNoHorizontalOverflow(
    "/groups/overlays?proof=script",
    { width: 1366, height: 900 },
    "sandbox-overlay-window-frame",
    [
      "sandbox-overlay-window-frame",
      "sandbox-overlay-window-safe-top",
      "sandbox-overlays-sticky-footer"
    ]);

  await page.locator("[data-testid='open-compact-dialog']").click();
  await assertViewportContained(
    "[data-testid='service-dialog-compact']",
    "Compact service dialog stays inside the viewport");
  await page.locator("[data-testid='service-dialog-compact'] [data-testid='close-sized-dialog']").click();
  await page.locator("[data-testid='service-dialog-compact']").waitFor({ state: "detached" });

  await page.locator("[data-testid='open-backdrop-dialog']").click();
  await page.locator("[data-testid='service-dialog-backdrop-locked']").waitFor({ state: "visible" });
  await page.mouse.click(8, 8);
  await check(
    await page.locator("[data-testid='service-dialog-backdrop-locked']").isVisible(),
    "Backdrop-locked dialog remains open after outside click");
  await page.locator("[data-testid='service-dialog-backdrop-locked'] [data-testid='close-backdrop-dialog']").click();
  await page.locator("[data-testid='service-dialog-backdrop-locked']").waitFor({ state: "detached" });

  await page.locator("[data-testid='open-result-dialog']").click();
  await page.locator("[data-testid='return-dialog-object']").click();
  await page.waitForFunction(() => document.querySelector("[data-testid='dialog-result']")?.textContent?.includes("Approved"));
  const dialogResultText = await page.locator("[data-testid='dialog-result']").textContent();
  await check(
    dialogResultText?.includes("Dialog returned: Approved"),
    "DialogService returns object results to caller",
    { dialogResultText });

  await page.locator("[data-testid='tooltip-trigger']").hover();
  await assertViewportContained(
    "[data-testid='service-tooltip']",
    "TooltipService tooltip opens on hover and stays inside the viewport");

  await page.locator("[data-testid='show-overlay-toast']").click();
  await assertViewportContained(
    "[data-testid='overlay-service-toast']",
    "Overlay NotificationService toast stays inside the viewport");

  await assertElementInsideElement(
    "[data-testid='sandbox-overlay-window']",
    "[data-testid='sandbox-overlay-window-frame']",
    "OverlayWindow initial geometry stays inside host frame");
  await page.getByLabel("Minimize window").click();
  await page.waitForFunction(() => document.querySelector("[data-testid='sandbox-overlay-window']")?.classList.contains("is-minimized"));
  await assertElementInsideElement(
    "[data-testid='sandbox-overlay-window']",
    "[data-testid='sandbox-overlay-window-frame']",
    "OverlayWindow minimized chip stays inside host frame");
  await page.getByLabel("Expand window").click();
  await page.getByLabel("Hide window").click();
  await page.locator("[data-testid='sandbox-overlay-window']").waitFor({ state: "detached" });
  await page.locator("[data-testid='show-overlay-window']").click();
  await assertElementInsideElement(
    "[data-testid='sandbox-overlay-window']",
    "[data-testid='sandbox-overlay-window-frame']",
    "OverlayWindow can be shown again after hidden state");

  await assertPageNoHorizontalOverflow(
    "/groups/overlays?scenario=long-text&proof=script",
    { width: 390, height: 844 },
    "sandbox-overlay-window-frame",
    [
      "sandbox-overlay-window-frame",
      "sandbox-overlay-window-safe-top",
      "sandbox-overlays-sticky-footer"
    ]);
  await assertElementInsideElement(
    "[data-testid='sandbox-overlay-window']",
    "[data-testid='sandbox-overlay-window-frame']",
    "Mobile OverlayWindow stays inside host frame");

  await check(consoleErrors.length === 0, "No browser console errors during SB08 verifier", { consoleErrors });
} catch (error) {
  checks.push({
    name: "SB08 verifier fatal error",
    passed: false,
    details: {
      message: error instanceof Error ? error.message : String(error),
      stack: error instanceof Error ? error.stack : undefined
    }
  });
  console.error(error);
} finally {
  await browser.close();

  const failed = checks.filter(checkResult => !checkResult.passed);
  const report = {
    generatedAt: new Date().toISOString(),
    baseUrl,
    browserChannel: "msedge",
    checks,
    consoleErrors,
    passed: failed.length === 0
  };

  writeFileSync(reportPath, `${JSON.stringify(report, null, 2)}\n`, "utf8");
  console.log(JSON.stringify({
    reportPath,
    checks: checks.length,
    failed: failed.length,
    consoleErrors: consoleErrors.length
  }, null, 2));

  if (failed.length > 0) {
    process.exit(1);
  }
}
