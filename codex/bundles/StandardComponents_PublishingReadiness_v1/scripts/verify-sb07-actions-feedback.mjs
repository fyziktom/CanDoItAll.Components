import { chromium } from "playwright";
import { mkdirSync, writeFileSync } from "node:fs";
import { dirname, resolve } from "node:path";

const baseUrl = process.env.SB07_BASE_URL ?? "http://127.0.0.1:5225";
const proofRoot = resolve("codex/bundles/StandardComponents_PublishingReadiness_v1/proof/SB07");
const reportPath = resolve(proofRoot, "data/sb07-actions-feedback-validation.json");

mkdirSync(dirname(reportPath), { recursive: true });

const browser = await chromium.launch({ channel: "msedge", headless: true });
const context = await browser.newContext();
await context.grantPermissions(["clipboard-read", "clipboard-write"], { origin: baseUrl });
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
}

async function documentOverflowDetails() {
  return page.evaluate(() => ({
    viewportWidth: window.innerWidth,
    documentScrollWidth: document.documentElement.scrollWidth,
    bodyScrollWidth: document.body.scrollWidth
  }));
}

async function elementMetrics(testId) {
  const locator = page.locator(`[data-testid='${testId}']`).first();
  await locator.scrollIntoViewIfNeeded();
  return locator.evaluate(element => {
    const rect = element.getBoundingClientRect();
    const style = getComputedStyle(element);
    return {
      testId: element.getAttribute("data-testid"),
      tagName: element.tagName.toLowerCase(),
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
    await check(
      metrics.scrollWidth <= metrics.clientWidth + 1 && metrics.rect.right <= metrics.viewport.width + 1 && metrics.rect.left >= -1,
      `No component horizontal overflow for ${testId} at ${path}`,
      metrics);
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

try {
  await assertPageNoHorizontalOverflow(
    "/groups/actions?proof=script",
    { width: 1366, height: 900 },
    "sandbox-actions-icon-badge-surface",
    [
      "sandbox-actions-icon-badge-surface",
      "sandbox-actions-long-button",
      "sandbox-actions-badge-group",
      "sandbox-actions-badge-button",
      "sandbox-actions-chip-row",
      "sandbox-actions-copy-text-icon",
      "sandbox-actions-copy-icon-button",
      "sandbox-actions-copy-icon-only"
    ]);

  await page.locator("[data-testid='sandbox-actions-copy-text-icon']").click();
  await page.waitForFunction(() => {
    const button = document.querySelector("[data-testid='sandbox-actions-copy-text-icon']");
    return button?.getAttribute("data-copy-state") === "copied";
  });
  const clipboardText = await page.evaluate(() => navigator.clipboard.readText());
  await check(
    clipboardText === "review-token-4Q26-copy-button",
    "CopyButton copies target input value and reports copied state",
    { clipboardText });

  await assertPageNoHorizontalOverflow(
    "/groups/actions?scenario=long-text&proof=script",
    { width: 390, height: 844 },
    "sandbox-actions-icon-badge-surface",
    [
      "sandbox-actions-icon-badge-surface",
      "sandbox-actions-long-button",
      "sandbox-actions-badge-group",
      "sandbox-actions-badge-button",
      "sandbox-actions-chip-row",
      "sandbox-actions-copy-text-icon",
      "sandbox-actions-copy-icon-button",
      "sandbox-actions-copy-icon-only"
    ]);

  await goto(
    "/groups/actions?scenario=disabled-state&proof=script",
    { width: 390, height: 844 },
    "sandbox-actions-icon-badge-surface");
  const disabledStates = await page.evaluate(() => ({
    iconView: document.querySelector("[data-testid='sandbox-actions-icon-only-view']")?.disabled ?? false,
    badgeButton: document.querySelector("[data-testid='sandbox-actions-badge-button']")?.disabled ?? false,
    copyText: document.querySelector("[data-testid='sandbox-actions-copy-text']")?.disabled ?? false,
    copyIconOnly: document.querySelector("[data-testid='sandbox-actions-copy-icon-only']")?.disabled ?? false,
    copySource: document.querySelector("[data-testid='sandbox-actions-copy-source']")?.disabled ?? false
  }));
  await check(
    Object.values(disabledStates).every(Boolean),
    "Disabled action scenario disables interactive actions and copy source",
    disabledStates);

  await assertPageNoHorizontalOverflow(
    "/groups/feedback?proof=script",
    { width: 1366, height: 900 },
    "sandbox-feedback-badge-chip-surface",
    [
      "sandbox-feedback-badge-chip-surface",
      "sandbox-feedback-callout-ok",
      "sandbox-feedback-callout-default",
      "sandbox-feedback-status-check-list",
      "sandbox-feedback-verification-list"
    ]);

  await page.locator("[data-testid='sandbox-feedback-tooltip-trigger']").hover();
  await assertViewportContained(
    "[data-testid='sandbox-feedback-tooltip-open']",
    "Tooltip opens on hover and stays inside the desktop viewport");

  await page.locator("[data-testid='sandbox-feedback-help-popover'] button").click();
  await assertViewportContained(
    "[data-testid='sandbox-feedback-help-popover-panel']",
    "HelpPopover opens on click and stays inside the desktop viewport");
  await page.keyboard.press("Escape");
  await page.locator("[data-testid='sandbox-feedback-help-popover-panel']").waitFor({ state: "detached" });

  await page.locator("[data-testid='feedback-persistent-toast']").click();
  await assertViewportContained(
    "[data-testid='feedback-persistent-notification']",
    "Persistent notification stays inside the desktop viewport");
  await page.locator("[data-testid='feedback-clear-toasts']").click();
  await page.locator("[data-testid='feedback-persistent-notification']").waitFor({ state: "detached" });

  await assertPageNoHorizontalOverflow(
    "/groups/feedback?scenario=long-text&proof=script",
    { width: 390, height: 844 },
    "sandbox-feedback-badge-chip-surface",
    [
      "sandbox-feedback-badge-chip-surface",
      "sandbox-feedback-callout-ok",
      "sandbox-feedback-callout-default",
      "sandbox-feedback-status-check-list",
      "sandbox-feedback-verification-list"
    ]);

  await page.locator("[data-testid='sandbox-feedback-help-popover'] button").click();
  const mobilePanelMetrics = await page.locator("[data-testid='sandbox-feedback-help-popover-panel']").evaluate(element => {
    const rect = element.getBoundingClientRect();
    const style = getComputedStyle(element);
    return {
      position: style.position,
      rect: {
        left: rect.left,
        right: rect.right,
        top: rect.top,
        bottom: rect.bottom,
        width: rect.width,
        height: rect.height
      },
      viewport: {
        width: window.innerWidth,
        height: window.innerHeight
      }
    };
  });
  await check(
    mobilePanelMetrics.position === "fixed"
      && mobilePanelMetrics.rect.left >= 0
      && mobilePanelMetrics.rect.right <= mobilePanelMetrics.viewport.width
      && mobilePanelMetrics.rect.bottom <= mobilePanelMetrics.viewport.height,
    "Mobile HelpPopover uses viewport-bound fixed sheet without clipping",
    mobilePanelMetrics);

  await check(consoleErrors.length === 0, "No browser console errors during SB07 verifier", { consoleErrors });
} catch (error) {
  checks.push({
    name: "SB07 verifier fatal error",
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
