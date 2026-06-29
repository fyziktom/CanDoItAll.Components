import fs from "node:fs";
import path from "node:path";
import { chromium } from "playwright";

const repoRoot = process.cwd();
const bundleRoot = path.join(repoRoot, "codex", "bundles", "StandardComponents_PublishingReadiness_v1");
const proofRoot = path.join(bundleRoot, "proof", "SB11");
const screenshotRoot = process.env.CDA_SB11_SCREENSHOT_ROOT
  ? path.resolve(repoRoot, process.env.CDA_SB11_SCREENSHOT_ROOT)
  : path.join(proofRoot, "screenshots", "matrix");
const dataRoot = path.join(proofRoot, "data");
const baseUrl = process.env.CDA_SB11_BASE_URL ?? "http://127.0.0.1:5225";

const viewports = [
  { id: "max-desktop", width: 1600, height: 1000, screenshot: false },
  { id: "desktop", width: 1366, height: 900, screenshot: true },
  { id: "tablet", width: 1024, height: 768, screenshot: false },
  { id: "mobile", width: 390, height: 844, screenshot: true },
];

const routes = [
  route("home", "/"),
  route("coverage", "/groups/coverage"),
  ...groupScenarios("foundations"),
  ...groupScenarios("inputs"),
  route("inputs-disabled", "/groups/inputs?scenario=disabled-state"),
  route("inputs-long", "/groups/inputs?scenario=long-text"),
  route("inputs-loading", "/groups/inputs?scenario=loading-state"),
  ...groupScenarios("actions"),
  route("actions-disabled", "/groups/actions?scenario=disabled-state"),
  route("actions-long", "/groups/actions?scenario=long-text"),
  ...groupScenarios("navigation"),
  route("navigation-long", "/groups/navigation?scenario=long-text"),
  route("navigation-tabs", "/groups/navigation/tabs"),
  route("navigation-tabs-long", "/groups/navigation/tabs?scenario=long-text"),
  route("navigation-tabs-disabled", "/groups/navigation/tabs?scenario=disabled-state"),
  ...groupScenarios("feedback"),
  route("feedback-long", "/groups/feedback?scenario=long-text"),
  route("feedback-loading", "/groups/feedback?scenario=loading-state"),
  ...groupScenarios("layout"),
  route("layout-long", "/groups/layout?scenario=long-text"),
  route("layout-composition", "/groups/layout/composition"),
  route("layout-composition-long", "/groups/layout/composition?scenario=long-text"),
  route("layout-composition-dense", "/groups/layout/composition?scenario=dense-content"),
  ...groupScenarios("data-display"),
  route("data-display-long", "/groups/data-display?scenario=long-text"),
  ...groupScenarios("charts"),
  route("charts-long", "/groups/charts?scenario=long-text"),
  ...groupScenarios("mermaid"),
  route("mermaid-long", "/groups/mermaid?scenario=long-text"),
  ...groupScenarios("overlays"),
  route("overlays-long", "/groups/overlays?scenario=long-text"),
];

const checks = [];
const consoleErrors = [];
const screenshotRows = [];

function route(id, urlPath) {
  return { id, path: urlPath };
}

function groupScenarios(group) {
  return [
    route(`${group}-happy`, `/groups/${group}?scenario=happy-path`),
    route(`${group}-dense`, `/groups/${group}?scenario=dense-content`),
    route(`${group}-empty`, `/groups/${group}?scenario=empty-state`),
  ];
}

function ensureDir(dir) {
  fs.mkdirSync(dir, { recursive: true });
}

function appendProof(urlPath, proofId) {
  const separator = urlPath.includes("?") ? "&" : "?";
  return `${urlPath}${separator}proof=${proofId}`;
}

async function check(condition, name, details = {}) {
  checks.push({ name, passed: Boolean(condition), details });
}

async function collectVisualMetrics(page) {
  return page.evaluate(() => {
    const viewportWidth = window.innerWidth;
    const viewportHeight = window.innerHeight;
    const documentScrollWidth = Math.max(
      document.documentElement.scrollWidth,
      document.body?.scrollWidth ?? 0);
    const bodyTextLength = (document.body?.innerText ?? "").replace(/\s+/g, " ").trim().length;
    const visibleElements = Array.from(document.body.querySelectorAll("*"))
      .filter((element) => {
        const style = getComputedStyle(element);
        const rect = element.getBoundingClientRect();
        return style.display !== "none"
          && style.visibility !== "hidden"
          && rect.width > 0
          && rect.height > 0;
      });
    const clipsOverflow = (element) => {
      const style = getComputedStyle(element);
      return ["auto", "scroll", "hidden", "clip"].includes(style.overflowX)
        || ["auto", "scroll", "hidden", "clip"].includes(style.overflowY);
    };
    const hasClippingAncestor = (element) => {
      for (let ancestor = element.parentElement; ancestor; ancestor = ancestor.parentElement) {
        if (clipsOverflow(ancestor)) {
          return true;
        }
      }

      return false;
    };
    const hasDirectText = (element) => Array.from(element.childNodes)
      .some((node) => node.nodeType === Node.TEXT_NODE && (node.textContent ?? "").trim().length > 0);
    const isScreenReaderOnly = (element, rect, style) => element.classList.contains("sr-only")
      || (style.position === "absolute" && rect.width <= 2 && rect.height <= 2);

    const overflowElements = visibleElements
      .map((element) => {
        const rect = element.getBoundingClientRect();
        return {
          tag: element.tagName.toLowerCase(),
          testId: element.getAttribute("data-testid"),
          id: element.id,
          className: typeof element.className === "string" ? element.className : "",
          text: (element.textContent ?? "").replace(/\s+/g, " ").trim().slice(0, 140),
          left: rect.left,
          right: rect.right,
          width: rect.width,
          locallyClipped: hasClippingAncestor(element),
        };
      })
      .filter((item) => !item.locallyClipped && (item.right > viewportWidth + 3 || item.left < -3))
      .slice(0, 20);

    const clippedTextElements = visibleElements
      .filter((element) => {
        const tagName = element.tagName.toLowerCase();
        if (["svg", "canvas", "path", "rect", "circle", "line", "polyline", "text", "textarea", "input", "select"].includes(tagName)) {
          return false;
        }

        const style = getComputedStyle(element);
        const rect = element.getBoundingClientRect();
        if (isScreenReaderOnly(element, rect, style) || !hasDirectText(element)) {
          return false;
        }

        if (style.overflowX === "auto" || style.overflowX === "scroll") {
          return false;
        }

        return element.scrollWidth > element.clientWidth + 4
          && (element.textContent ?? "").trim().length > 0;
      })
      .map((element) => ({
        tag: element.tagName.toLowerCase(),
        testId: element.getAttribute("data-testid"),
        id: element.id,
        className: typeof element.className === "string" ? element.className : "",
        text: (element.textContent ?? "").replace(/\s+/g, " ").trim().slice(0, 140),
        clientWidth: element.clientWidth,
        scrollWidth: element.scrollWidth,
      }))
      .slice(0, 20);

    const visibleMain = Boolean(document.querySelector("main, [data-testid='sandbox-demo-surface'], .sandbox-page"));

    return {
      viewportWidth,
      viewportHeight,
      documentScrollWidth,
      bodyTextLength,
      visibleMain,
      overflowElements,
      clippedTextElements,
    };
  });
}

async function runMatrix() {
  ensureDir(screenshotRoot);
  ensureDir(dataRoot);

  const browser = await chromium.launch({ channel: "msedge", headless: true });
  const page = await browser.newPage();
  page.on("console", (message) => {
    if (message.type() === "error") {
      consoleErrors.push(message.text());
    }
  });

  try {
    for (const routeEntry of routes) {
      for (const viewport of viewports) {
        await page.setViewportSize({ width: viewport.width, height: viewport.height });
        const proofId = `sb11-${routeEntry.id}-${viewport.id}`;
        const url = `${baseUrl}${appendProof(routeEntry.path, proofId)}`;
        await page.goto(url, { waitUntil: "domcontentloaded", timeout: 45000 });
        await page.waitForTimeout(900);

        const metrics = await collectVisualMetrics(page);
        await check(
          metrics.visibleMain && metrics.bodyTextLength > 60,
          `Route ${routeEntry.id} renders meaningful content at ${viewport.id}`,
          { route: routeEntry.path, viewport, bodyTextLength: metrics.bodyTextLength, visibleMain: metrics.visibleMain });
        await check(
          metrics.documentScrollWidth <= viewport.width + 3,
          `Route ${routeEntry.id} has no page horizontal overflow at ${viewport.id}`,
          { route: routeEntry.path, viewport, documentScrollWidth: metrics.documentScrollWidth });
        await check(
          metrics.overflowElements.length === 0,
          `Route ${routeEntry.id} has no visible element escaping viewport at ${viewport.id}`,
          { route: routeEntry.path, viewport, overflowElements: metrics.overflowElements });
        await check(
          metrics.clippedTextElements.length === 0,
          `Route ${routeEntry.id} has no clipped text containers at ${viewport.id}`,
          { route: routeEntry.path, viewport, clippedTextElements: metrics.clippedTextElements });

        if (viewport.screenshot) {
          const fileName = `${routeEntry.id}-${viewport.id}.png`;
          const absolutePath = path.join(screenshotRoot, fileName);
          await page.screenshot({ path: absolutePath, fullPage: true });
          screenshotRows.push({
            routeId: routeEntry.id,
            route: routeEntry.path,
            viewport: viewport.id,
            screenshot: path.relative(bundleRoot, absolutePath).replaceAll(path.sep, "/"),
          });
        }
      }
    }

    await check(
      consoleErrors.length === 0,
      "No browser console errors during SB11 visual matrix",
      { consoleErrors });
  } finally {
    await browser.close();
  }
}

try {
  await runMatrix();
} catch (error) {
  checks.push({
    name: "SB11 visual matrix fatal error",
    passed: false,
    details: {
      message: error instanceof Error ? error.message : String(error),
      stack: error instanceof Error ? error.stack : undefined,
    },
  });
  console.error(error);
}

const failed = checks.filter((check) => !check.passed);
const report = {
  generatedAt: new Date().toISOString(),
  baseUrl,
  routeCount: routes.length,
  viewportCount: viewports.length,
  screenshotCount: screenshotRows.length,
  checks,
  screenshots: screenshotRows,
  consoleErrors,
};
const reportPath = path.join(dataRoot, "sb11-visual-matrix.json");
fs.writeFileSync(reportPath, JSON.stringify(report, null, 2) + "\n");

console.log(JSON.stringify({
  reportPath,
  routes: routes.length,
  viewports: viewports.length,
  screenshots: screenshotRows.length,
  checks: checks.length,
  failed: failed.length,
  consoleErrors: consoleErrors.length,
}, null, 2));

if (failed.length > 0) {
  process.exitCode = 1;
}
