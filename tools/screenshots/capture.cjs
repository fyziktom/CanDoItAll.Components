// Screenshots every route x viewport x theme x alternative from config into
// config.outputDir/current/{viewport}_{theme}_{alternative}/{page}.png, using Playwright.
// Writes data/manifest.json alongside the images. Does not touch git.
// Usage: node capture.cjs [--config <path>]

const fs = require("node:fs");
const path = require("node:path");
const { chromium } = require("playwright");
const { loadConfig, resolveRoutes, buildJobs } = require("./lib/config.cjs");
const { parseArgs } = require("./lib/args.cjs");

const args = parseArgs(process.argv.slice(2));
const configPath = args.config || path.join(__dirname, "screenshots.config.json");

console.log(`Config file: ${configPath}`);

main().catch(error => {
  console.error(error.stack || error.message || String(error));
  process.exitCode = 1;
});

async function main() {
  const config = loadConfig(configPath);
  const routes = await resolveRoutes(config);

  if (routes.length === 0) {
    throw new Error("No routes resolved from \"routes\"/\"routesEndpoint\" — nothing to capture.");
  }

  const jobs = buildJobs(config, routes);

  fs.rmSync(config.outputDir, { recursive: true, force: true });
  fs.mkdirSync(config.outputDir, { recursive: true });

  console.log(`Capturing ${jobs.length} screenshots from ${routes.length} route(s)...`);

  const browser = await chromium.launch({ headless: true });
  const manifest = [];

  try {
    for (const [index, job] of jobs.entries()) {
      const entry = await captureJob(browser, config, job);
      manifest.push(entry);
      const progress = `${index + 1}/${jobs.length}`;
      console.log(entry.error ? `  FAIL ${progress}   ${job.key}: ${entry.error}` : `  ok   ${progress}   ${job.key}`);
    }
  } finally {
    await browser.close();
  }

  const dataDir = path.join(config.outputDir, "data");
  fs.mkdirSync(dataDir, { recursive: true });
  const manifestPath = path.join(dataDir, "manifest.json");
  fs.writeFileSync(
    manifestPath,
    `${JSON.stringify({ generatedAtUtc: new Date().toISOString(), jobs: manifest }, null, 2)}\n`
  );

  const failed = manifest.filter(entry => entry.error);
  console.log(`Captured ${manifest.length - failed.length}/${manifest.length} screenshots. Manifest: ${manifestPath}`);

  if (failed.length > 0) {
    throw new Error(`${failed.length} capture job(s) failed: ${failed.map(entry => entry.key).join(", ")}`);
  }
}

async function captureJob(browser, config, job) {
  const context = await browser.newContext({
    viewport: { width: job.viewport.width, height: job.viewport.height },
    reducedMotion: "reduce",
    ignoreHTTPSErrors: true,
  });
  await context.addInitScript(storage => {
    for (const [key, value] of Object.entries(storage)) {
      window.sessionStorage.setItem(key, value);
    }
  }, config.sessionStorage ?? {});

  const consoleErrors = [];
  const base = {
    key: job.key,
    combo: job.combo,
    page: job.page,
    file: job.file,
    route: job.route.path,
    title: job.route.title,
    group: job.route.group,
    viewport: job.viewport.name,
    theme: job.themeName,
    alternative: job.alternativeName,
    url: job.url,
  };

  try {
    const page = await context.newPage();
    page.on("console", message => {
      if (message.type() === "error") {
        consoleErrors.push(message.text());
      }
    });
    page.on("pageerror", error => consoleErrors.push(error.message));

    await page.goto(job.url, { waitUntil: "networkidle", timeout: 30000 });
    await page.waitForSelector(config.readySelector, { timeout: 30000 });
    await page.evaluate(() => document.fonts.ready);
    await page.waitForTimeout(config.defaultSettleMs ?? 300);

    const filePath = path.join(config.outputDir, "current", base.file);
    fs.mkdirSync(path.dirname(filePath), { recursive: true });
    await page.screenshot({ path: filePath, animations: "disabled", fullPage: true });

    return { ...base, consoleErrors };
  } catch (error) {
    return { ...base, file: undefined, consoleErrors, error: error.message };
  } finally {
    await context.close();
  }
}
