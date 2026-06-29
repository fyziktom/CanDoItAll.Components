import { chromium } from "playwright";
import fs from "node:fs/promises";
import path from "node:path";

const mode = process.argv[2] ?? "after";
const strict = process.argv.includes("--strict");
const baseUrl = process.env.SB02_BASE_URL ?? "http://127.0.0.1:55174";
const bundleRoot = path.resolve("codex/bundles/StandardComponents_PublishingReadiness_v1");
const screenshotRoot = path.join(bundleRoot, "proof", "SB02", "screenshots", mode);
const dataRoot = path.join(bundleRoot, "proof", "SB02", "data");

const viewports = [
  { name: "1366", width: 1366, height: 900 },
  { name: "390", width: 390, height: 844 },
];

const routes = [
  { slug: "inputs", path: "/groups/inputs", ready: ".sandbox-page" },
  { slug: "actions", path: "/groups/actions", ready: "[data-testid='sandbox-actions-copy-source']" },
  { slug: "tabs", path: "/groups/navigation/tabs", ready: "[data-testid='tabs-lab-basic']" },
];

function screenshotPath(slug, viewport, suffix = "default") {
  return path.join(screenshotRoot, `${slug}-${viewport.name}-${suffix}.png`);
}

async function stabilize(page, readySelector) {
  await page.waitForLoadState("domcontentloaded");
  await page.waitForSelector(readySelector, { timeout: 30000 });
  await page.evaluate(() => document.fonts?.ready);
  await page.waitForTimeout(350);
}

async function scanLayout(page) {
  return page.evaluate(() => {
    const ignoredTags = new Set(["HTML", "BODY", "SCRIPT", "STYLE", "LINK", "META"]);
    const makeSelector = (element) => {
      const testId = element.getAttribute("data-testid");
      if (testId) {
        return `[data-testid="${testId}"]`;
      }

      const id = element.id;
      if (id) {
        return `#${CSS.escape(id)}`;
      }

      const className = [...element.classList].slice(0, 4).map((name) => `.${CSS.escape(name)}`).join("");
      return `${element.tagName.toLowerCase()}${className}`;
    };

    const visibleElements = [...document.querySelectorAll("body *")].filter((element) => {
      if (ignoredTags.has(element.tagName)) {
        return false;
      }

      if (element.classList.contains("sr-only")) {
        return false;
      }

      const style = getComputedStyle(element);
      const rect = element.getBoundingClientRect();
      return style.display !== "none"
        && style.visibility !== "hidden"
        && rect.width > 0
        && rect.height > 0;
    });

    const horizontalOverflows = [];
    const clippedText = [];
    const viewportOverflows = [];

    for (const element of visibleElements) {
      const style = getComputedStyle(element);
      const rect = element.getBoundingClientRect();
      const selector = makeSelector(element);
      const text = (element.textContent ?? "").replace(/\s+/g, " ").trim().slice(0, 120);
      const overflowX = style.overflowX;
      const overflowY = style.overflowY;
      const scrollableX = overflowX === "auto" || overflowX === "scroll";
      const intentionallyScrollable = element.closest(".cad-tabs__list, pre, textarea, code");
      const isLeafText = text && element.children.length === 0;

      if (!scrollableX && !intentionallyScrollable && element.scrollWidth > element.clientWidth + 2) {
        horizontalOverflows.push({
          selector,
          className: element.className?.toString?.() ?? "",
          text,
          clientWidth: element.clientWidth,
          scrollWidth: element.scrollWidth,
          overflowX,
        });
      }

      if ((overflowX === "hidden" || overflowY === "hidden") && element.scrollWidth > element.clientWidth + 2 && isLeafText) {
        clippedText.push({
          selector,
          text,
          clientWidth: element.clientWidth,
          scrollWidth: element.scrollWidth,
          overflowX,
          overflowY,
        });
      }

      if (!intentionallyScrollable && rect.right > window.innerWidth + 2 && rect.width < window.innerWidth * 3) {
        viewportOverflows.push({
          selector,
          className: element.className?.toString?.() ?? "",
          text,
          right: Math.round(rect.right),
          viewportWidth: window.innerWidth,
        });
      }
    }

    return {
      url: location.href,
      title: document.title,
      viewport: { width: window.innerWidth, height: window.innerHeight },
      pageHorizontalOverflow: document.documentElement.scrollWidth > window.innerWidth + 2
        || document.body.scrollWidth > window.innerWidth + 2,
      documentScrollWidth: document.documentElement.scrollWidth,
      bodyScrollWidth: document.body.scrollWidth,
      horizontalOverflows,
      clippedText,
      viewportOverflows,
    };
  });
}

async function captureRoute(page, route, viewport) {
  await page.setViewportSize({ width: viewport.width, height: viewport.height });
  await page.goto(`${baseUrl}${route.path}`);
  await stabilize(page, route.ready);

  const captures = [];
  const defaultPath = screenshotPath(route.slug, viewport, "default");
  await page.screenshot({ path: defaultPath, fullPage: true, scale: "css" });
  captures.push({ kind: "default", path: defaultPath, scan: await scanLayout(page) });

  if (route.slug === "actions") {
    const copyButton = page.locator("[data-testid='sandbox-actions-copy-icon-button']").first();
    if (await copyButton.count()) {
      await copyButton.click({ trial: true }).catch(() => {});
      await copyButton.click().catch(() => {});
      await page.waitForTimeout(250);
      const actionPath = screenshotPath(route.slug, viewport, "copy-clicked");
      await page.screenshot({ path: actionPath, fullPage: true, scale: "css" });
      captures.push({ kind: "copy-clicked", path: actionPath, scan: await scanLayout(page) });
    }
  }

  if (route.slug === "tabs") {
    const basicTabs = page.locator("[data-testid='tabs-lab-basic'] [role='tab']");
    if (await basicTabs.count()) {
      await basicTabs.first().click();
      await page.waitForTimeout(250);
      const selectedPath = screenshotPath(route.slug, viewport, "basic-overview-selected");
      await page.screenshot({ path: selectedPath, fullPage: true, scale: "css" });
      captures.push({ kind: "basic-overview-selected", path: selectedPath, scan: await scanLayout(page) });
    }
  }

  return captures;
}

await fs.mkdir(screenshotRoot, { recursive: true });
await fs.mkdir(dataRoot, { recursive: true });

const browser = await chromium.launch({ headless: true });
const page = await browser.newPage();
const results = [];

try {
  for (const viewport of viewports) {
    for (const route of routes) {
      const captures = await captureRoute(page, route, viewport);
      for (const capture of captures) {
        results.push({
          mode,
          route: route.slug,
          viewport: viewport.name,
          kind: capture.kind,
          screenshot: path.relative(bundleRoot, capture.path).replaceAll("\\", "/"),
          scan: capture.scan,
        });
      }
    }
  }
} finally {
  await browser.close();
}

const summary = results.map((result) => ({
  mode: result.mode,
  route: result.route,
  viewport: result.viewport,
  kind: result.kind,
  screenshot: result.screenshot,
  pageHorizontalOverflow: result.scan.pageHorizontalOverflow,
  horizontalOverflows: result.scan.horizontalOverflows.length,
  clippedText: result.scan.clippedText.length,
  viewportOverflows: result.scan.viewportOverflows.length,
}));

const output = {
  mode,
  strict,
  baseUrl,
  capturedAt: new Date().toISOString(),
  summary,
  results,
};

const outputPath = path.join(dataRoot, `sb02-visual-${mode}.json`);
await fs.writeFile(outputPath, `${JSON.stringify(output, null, 2)}\n`, "utf8");

console.log(`SB02-VISUAL-MODE=${mode}`);
console.log(`SB02-VISUAL-OUTPUT=${path.relative(process.cwd(), outputPath)}`);
for (const item of summary) {
  console.log(`SB02-VISUAL ${item.route}/${item.viewport}/${item.kind} pageHorizontal=${item.pageHorizontalOverflow} horizontal=${item.horizontalOverflows} clipped=${item.clippedText} viewport=${item.viewportOverflows} screenshot=${item.screenshot}`);
}

const failures = summary.filter((item) => item.pageHorizontalOverflow || item.clippedText);
if (strict && failures.length > 0) {
  console.error(`SB02-VISUAL-STRICT-FAIL count=${failures.length}`);
  process.exitCode = 1;
}
