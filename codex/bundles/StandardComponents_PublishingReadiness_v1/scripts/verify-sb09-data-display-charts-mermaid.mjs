import { chromium } from "playwright";
import { mkdirSync, writeFileSync } from "node:fs";
import { dirname, resolve } from "node:path";

const baseUrl = process.env.SB09_BASE_URL ?? "http://127.0.0.1:5225";
const proofRoot = resolve("codex/bundles/StandardComponents_PublishingReadiness_v1/proof/SB09");
const reportPath = resolve(proofRoot, "data/sb09-data-display-charts-mermaid-validation.json");

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
  await page.waitForTimeout(350);
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
  await locator.waitFor({ state: "visible" });
  await locator.scrollIntoViewIfNeeded();
  return locator.evaluate(element => {
    const rect = element.getBoundingClientRect();
    const style = getComputedStyle(element);
    return {
      testId: element.getAttribute("data-testid"),
      tagName: element.tagName.toLowerCase(),
      display: style.display,
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
    const scrollTolerance = metrics.overflowX === "visible" ? 1 : 24;
    await check(
      metrics.scrollWidth <= metrics.clientWidth + scrollTolerance
        && metrics.rect.right <= metrics.viewport.width + 1
        && metrics.rect.left >= -1,
      `No component horizontal overflow for ${testId} at ${path}`,
      { ...metrics, scrollTolerance });
  }
}

async function assertSelectionTitlesWrap(path, viewport) {
  await goto(path, viewport, "data-display-selection-rows");
  const titleMetrics = await page.locator(".cda-selection-list-item__title").evaluateAll(elements =>
    elements.map(element => {
      const rect = element.getBoundingClientRect();
      const style = getComputedStyle(element);
      return {
        text: element.textContent?.trim(),
        whiteSpace: style.whiteSpace,
        overflow: style.overflow,
        textOverflow: style.textOverflow,
        rectHeight: rect.height,
        lineHeight: parseFloat(style.lineHeight),
        clientWidth: element.clientWidth,
        scrollWidth: element.scrollWidth
      };
    }));

  await check(
    titleMetrics.length > 0
      && titleMetrics.every(metric => metric.whiteSpace !== "nowrap" && metric.textOverflow !== "ellipsis"),
    "SelectionListItem titles wrap instead of hard-truncating long labels",
    { titleMetrics });
}

async function waitForApexChart(testId) {
  await page.waitForFunction(selector => {
    const root = document.querySelector(selector);
    return Boolean(root?.querySelector(".apexcharts-svg"));
  }, `[data-testid='${testId}']`, { timeout: 20000 });
}

async function assertApexChartNonBlank(testId) {
  await waitForApexChart(testId);
  const chartInfo = await page.locator(`[data-testid='${testId}']`).evaluate(element => {
    const svg = element.querySelector(".apexcharts-svg");
    const svgRect = svg?.getBoundingClientRect();
    const geometrySelector = [
      ".apexcharts-series path",
      ".apexcharts-pie-series path",
      ".apexcharts-bar-area",
      ".apexcharts-line",
      ".apexcharts-area",
      ".apexcharts-marker",
      ".apexcharts-datalabel"
    ].join(",");
    const geometry = Array.from(element.querySelectorAll(geometrySelector));
    const visibleGeometry = geometry.filter(node => {
      const rect = node.getBoundingClientRect();
      const style = getComputedStyle(node);
      return rect.width > 0 || rect.height > 0 || style.stroke !== "none" || style.fill !== "none";
    });
    const textLabels = Array.from(element.querySelectorAll(".apexcharts-text, .apexcharts-legend-text"))
      .map(node => node.textContent?.trim())
      .filter(Boolean);

    return {
      testId: element.getAttribute("data-testid"),
      state: element.getAttribute("data-cda-chart-state"),
      svg: svg ? {
        width: svgRect?.width ?? 0,
        height: svgRect?.height ?? 0,
        viewBox: svg.getAttribute("viewBox")
      } : null,
      geometryCount: geometry.length,
      visibleGeometryCount: visibleGeometry.length,
      textLabelCount: textLabels.length,
      textLabels: textLabels.slice(0, 12)
    };
  });

  await check(
    chartInfo.state === "ready"
      && chartInfo.svg?.width > 80
      && chartInfo.svg?.height > 80
      && chartInfo.visibleGeometryCount > 0
      && chartInfo.textLabelCount > 0,
    `Apex chart ${testId} renders nonblank SVG output`,
    chartInfo);
}

async function assertChartEmptyState() {
  await goto(
    "/groups/charts?scenario=empty-state&proof=script",
    { width: 1366, height: 900 },
    "chart-empty");

  const emptyInfo = await page.locator("[data-testid='chart-empty']").evaluate(element => ({
    state: element.getAttribute("data-cda-chart-state"),
    text: element.textContent?.replace(/\s+/g, " ").trim(),
    hasApexSvg: Boolean(element.querySelector(".apexcharts-svg")),
    width: element.getBoundingClientRect().width,
    height: element.getBoundingClientRect().height
  }));

  await check(
    emptyInfo.state === "empty"
      && emptyInfo.text?.includes("Empty chart")
      && emptyInfo.text?.includes("No chart data")
      && !emptyInfo.hasApexSvg
      && emptyInfo.width > 300
      && emptyInfo.height > 120,
    "CdaChart empty state preserves attributes, heading, and informative copy",
    emptyInfo);
}

async function waitForMermaidSvg(testId) {
  await page.waitForFunction(selector => {
    const root = document.querySelector(selector);
    const svg = root?.querySelector("svg[data-cda-mermaid-svg]");
    const rect = svg?.getBoundingClientRect();
    const geometryCount = svg?.querySelectorAll("path, rect, circle, ellipse, line, polygon, polyline, text, g").length ?? 0;
    return Boolean(svg && rect.width > 0 && rect.height > 0 && geometryCount > 0);
  }, `[data-testid='${testId}']`, { timeout: 30000 });
}

async function assertMermaidNonBlank(testId) {
  await waitForMermaidSvg(testId);
  const diagramInfo = await page.locator(`[data-testid='${testId}']`).evaluate(element => {
    const svg = element.querySelector("svg[data-cda-mermaid-svg]");
    const rect = svg?.getBoundingClientRect();
    const geometry = Array.from(svg?.querySelectorAll("path, rect, circle, ellipse, line, polygon, polyline, text, g") ?? []);
    const text = svg?.textContent?.replace(/\s+/g, " ").trim() ?? "";
    return {
      testId: element.getAttribute("data-testid"),
      svgId: svg?.id ?? null,
      viewBox: svg?.getAttribute("viewBox") ?? null,
      width: rect?.width ?? 0,
      height: rect?.height ?? 0,
      geometryCount: geometry.length,
      textLength: text.length,
      textSample: text.slice(0, 120),
      errorText: element.querySelector("[data-testid='mermaid-error']")?.textContent?.trim() ?? null
    };
  });

  await check(
    diagramInfo.width > 80
      && diagramInfo.height > 80
      && diagramInfo.geometryCount > 0
      && diagramInfo.errorText === null,
    `Mermaid diagram ${testId} renders nonblank SVG output`,
    diagramInfo);
}

async function assertAllGalleryMermaidExamplesRender() {
  await page.waitForFunction(() => {
    const items = Array.from(document.querySelectorAll("[data-testid^='mermaid-example-']"));
    return items.length >= 20 && items.every(item => item.querySelector("svg[data-cda-mermaid-svg]") || item.querySelector("[data-testid='mermaid-error']"));
  }, null, { timeout: 45000 });

  const galleryInfo = await page.locator("[data-testid^='mermaid-example-']").evaluateAll(items =>
    items.map(item => {
      const svg = item.querySelector("svg[data-cda-mermaid-svg]");
      const rect = svg?.getBoundingClientRect();
      const status = item.querySelector(".sandbox-mermaid-gallery__status")?.textContent?.replace(/\s+/g, " ").trim() ?? "";
      const geometryCount = svg?.querySelectorAll("path, rect, circle, ellipse, line, polygon, polyline, text, g").length ?? 0;
      return {
        testId: item.getAttribute("data-testid"),
        status,
        hasSvg: Boolean(svg),
        width: rect?.width ?? 0,
        height: rect?.height ?? 0,
        geometryCount,
        error: item.querySelector("[data-testid='mermaid-error']")?.textContent?.trim() ?? null
      };
    }));

  await check(
    galleryInfo.length >= 20
      && galleryInfo.every(item => item.hasSvg
        && item.width > 80
        && item.height > 60
        && item.geometryCount > 0
        && item.error === null),
    "All bundled Mermaid gallery examples render nonblank SVGs",
    { galleryInfo });
}

async function assertMermaidClickZoomAndPan() {
  await goto(
    "/groups/mermaid?proof=script",
    { width: 1366, height: 900 },
    "mermaid-flowchart");

  await assertMermaidNonBlank("mermaid-flowchart");
  await assertMermaidNonBlank("mermaid-architecture");

  const firstNode = page.locator("[data-testid='mermaid-flowchart'] [data-cda-mermaid-node='true']").first();
  await firstNode.waitFor({ state: "visible" });
  const beforeClickText = await page.locator("[data-testid='mermaid-click-result']").textContent();
  await firstNode.click();
  await page.waitForFunction(before => {
    const result = document.querySelector("[data-testid='mermaid-click-result']");
    return result && result.textContent !== before && !result.textContent?.includes("No node selected");
  }, beforeClickText);
  const clickText = await page.locator("[data-testid='mermaid-click-result']").textContent();
  await check(
    clickText && !clickText.includes("No node selected") && clickText.includes("Id:"),
    "Mermaid flowchart node click updates the Blazor event panel",
    { clickText });

  const flowchartRoot = page.locator("[data-testid='mermaid-flowchart']");
  const svg = flowchartRoot.locator("svg[data-cda-mermaid-svg]");
  const initialViewBox = await svg.getAttribute("viewBox");
  await flowchartRoot.getByLabel("Zoom in").click();
  await page.waitForFunction(({ selector, previous }) => {
    const element = document.querySelector(selector);
    return element?.getAttribute("viewBox") !== previous;
  }, { selector: "[data-testid='mermaid-flowchart'] svg[data-cda-mermaid-svg]", previous: initialViewBox });
  const zoomedViewBox = await svg.getAttribute("viewBox");
  await check(
    zoomedViewBox !== initialViewBox,
    "Mermaid zoom control changes the SVG viewBox",
    { initialViewBox, zoomedViewBox });

  const box = await svg.boundingBox();
  await check(Boolean(box), "Mermaid flowchart SVG has a measurable box before pan");
  await page.mouse.move(box.x + 18, box.y + 18);
  await page.mouse.down();
  await page.mouse.move(box.x + 72, box.y + 46);
  await page.mouse.up();
  await page.waitForFunction(({ selector, previous }) => {
    const element = document.querySelector(selector);
    return element?.getAttribute("viewBox") !== previous;
  }, { selector: "[data-testid='mermaid-flowchart'] svg[data-cda-mermaid-svg]", previous: zoomedViewBox });
  const pannedViewBox = await svg.getAttribute("viewBox");
  await check(
    pannedViewBox !== zoomedViewBox,
    "Mermaid pan interaction changes the SVG viewBox",
    { zoomedViewBox, pannedViewBox });

  await assertAllGalleryMermaidExamplesRender();
}

async function assertMermaidEmptyAndErrorStates() {
  await goto(
    "/groups/mermaid?scenario=empty-state&proof=script",
    { width: 1366, height: 900 },
    "mermaid-empty");

  const emptyInfo = await page.locator("[data-testid='mermaid-empty']").evaluate(element => ({
    text: element.textContent?.replace(/\s+/g, " ").trim(),
    hasSvg: Boolean(element.querySelector("svg")),
    stateVisible: Boolean(Array.from(element.querySelectorAll(".cda-mermaid__state")).find(node => node.textContent?.includes("No Mermaid source")))
  }));
  await check(
    emptyInfo.text?.includes("Empty Mermaid source")
      && emptyInfo.stateVisible
      && !emptyInfo.hasSvg,
    "Mermaid empty source renders an informative empty viewport",
    emptyInfo);

  await page.locator("[data-testid='mermaid-error-example'] [data-testid='mermaid-error']").waitFor({ state: "visible", timeout: 30000 });
  const errorInfo = await page.locator("[data-testid='mermaid-error-example']").evaluate(element => ({
    text: element.textContent?.replace(/\s+/g, " ").trim(),
    paragraphs: Array.from(element.querySelectorAll("[data-testid='mermaid-error'] p")).map(node => node.textContent?.replace(/\s+/g, " ").trim()),
    excerpt: element.querySelector("[data-testid='mermaid-error'] pre")?.textContent ?? null,
    hasSvg: Boolean(element.querySelector("svg")),
    hasError: Boolean(element.querySelector("[data-testid='mermaid-error']")),
    leakedMermaidErrorSvgs: Array.from(document.body.querySelectorAll("svg[aria-roledescription='error']")).filter(svg => !element.contains(svg)).length,
    leakedMermaidErrorText: document.body.innerText.includes("Syntax error in text")
  }));
  await check(
    errorInfo.hasError
      && !errorInfo.hasSvg
      && errorInfo.text?.includes("Mermaid syntax error")
      && errorInfo.paragraphs?.[0] === "Mermaid could not parse this diagram."
      && errorInfo.text?.includes("Line 3, column 14")
      && errorInfo.excerpt?.includes("start --> ??? bad target")
      && errorInfo.leakedMermaidErrorSvgs === 0
      && !errorInfo.leakedMermaidErrorText,
    "Mermaid parser errors show structured diagnostics without leaking Mermaid fallback SVGs",
    errorInfo);
}

try {
  await assertPageNoHorizontalOverflow(
    "/groups/data-display?scenario=dense-content&proof=script",
    { width: 1366, height: 900 },
    "data-display-summary-strip",
    [
      "sandbox-demo-surface",
      "data-display-summary-strip",
      "data-display-card-grid",
      "data-display-selection-rows"
    ]);

  await assertPageNoHorizontalOverflow(
    "/groups/data-display?scenario=long-text&frame=mobile&proof=script",
    { width: 390, height: 844 },
    "data-display-summary-strip",
    [
      "sandbox-demo-surface",
      "data-display-summary-strip",
      "data-display-card-grid",
      "data-display-selection-rows"
    ]);
  await assertSelectionTitlesWrap(
    "/groups/data-display?scenario=long-text&frame=mobile&proof=script",
    { width: 390, height: 844 });

  await assertPageNoHorizontalOverflow(
    "/groups/data-display?scenario=empty-state&frame=mobile&proof=script",
    { width: 390, height: 844 },
    "data-display-empty-state",
    [
      "sandbox-demo-surface",
      "data-display-empty-state"
    ]);

  await assertPageNoHorizontalOverflow(
    "/groups/charts?scenario=dense-content&proof=script",
    { width: 1366, height: 900 },
    "chart-area",
    [
      "sandbox-demo-surface",
      "chart-area",
      "chart-pie",
      "chart-multiline",
      "chart-labels",
      "chart-colors"
    ]);
  for (const testId of ["chart-area", "chart-pie", "chart-multiline", "chart-labels", "chart-colors"]) {
    await assertApexChartNonBlank(testId);
  }

  await assertPageNoHorizontalOverflow(
    "/groups/charts?scenario=dense-content&frame=mobile&proof=script",
    { width: 390, height: 844 },
    "chart-area",
    [
      "sandbox-demo-surface",
      "chart-area",
      "chart-pie",
      "chart-multiline",
      "chart-labels",
      "chart-colors"
    ]);
  await assertApexChartNonBlank("chart-area");
  await assertApexChartNonBlank("chart-pie");
  await assertChartEmptyState();

  await assertPageNoHorizontalOverflow(
    "/groups/mermaid?proof=script",
    { width: 1366, height: 900 },
    "mermaid-flowchart",
    [
      "sandbox-demo-surface",
      "mermaid-flowchart",
      "mermaid-architecture",
      "mermaid-gallery"
    ]);
  await assertMermaidClickZoomAndPan();

  await assertPageNoHorizontalOverflow(
    "/groups/mermaid?frame=mobile&proof=script",
    { width: 390, height: 844 },
    "mermaid-flowchart",
    [
      "sandbox-demo-surface",
      "mermaid-flowchart",
      "mermaid-architecture",
      "mermaid-gallery"
    ]);
  await assertMermaidNonBlank("mermaid-flowchart");

  await assertMermaidEmptyAndErrorStates();

  await check(consoleErrors.length === 0, "No browser console errors during SB09 verifier", { consoleErrors });
} catch (error) {
  checks.push({
    name: "SB09 verifier fatal error",
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
