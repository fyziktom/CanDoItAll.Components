// Watches the rendered /test?raw DOM and writes an offline visual before/after report
// whenever hot reload changes it. Usage: npm run watch:test-page -- --url http://localhost:55174/test?raw
const fs = require("node:fs");
const path = require("node:path");
const { chromium } = require("playwright");

const options = readOptions(process.argv.slice(2));
const outputDir = path.resolve(options.output || "artifacts/test-page-diff");
const pollMs = Number(options.interval || 750);

main().catch(error => {
  console.error(error.stack || error.message || String(error));
  process.exitCode = 1;
});

async function main() {
  fs.mkdirSync(outputDir, { recursive: true });
  const browser = await chromium.launch({ headless: true });
  const page = await browser.newPage({ viewport: { width: 1600, height: 1000 } });
  let previous = await capture(page, options.url);
  console.log(`Watching ${options.url}`);
  console.log(`Baseline captured. Reports will be written to ${outputDir}`);

  const close = async () => {
    await browser.close();
    process.exit(0);
  };
  process.once("SIGINT", close);
  process.once("SIGTERM", close);

  for (;;) {
    await delay(pollMs);
    let current;
    try {
      current = await capture(page, options.url);
    } catch (error) {
      console.warn(`Waiting for /test: ${error.message}`);
      continue;
    }

    if (current.fingerprint === previous.fingerprint) continue;
    const file = path.join(outputDir, `test-diff-${timestamp()}.html`);
    fs.writeFileSync(file, buildReport(previous, current));
    console.log(`Changed: ${file}`);
    previous = current;
  }
}

async function capture(page, url) {
  await page.goto(url, { waitUntil: "networkidle", timeout: 30000 });
  await page.waitForSelector("[data-testid='test-page']", { timeout: 30000 });
  await page.evaluate(() => document.fonts.ready);
  await page.waitForTimeout(200);
  const html = await page.evaluate(() => {
    const clone = document.documentElement.cloneNode(true);
    clone.querySelectorAll("script[src*='_framework']").forEach(element => element.remove());
    clone.querySelectorAll("[src], [href]").forEach(element => {
      const attribute = element.hasAttribute("src") ? "src" : "href";
      const value = element.getAttribute(attribute);
      if (value && !value.startsWith("data:") && !value.startsWith("#")) element.setAttribute(attribute, new URL(value, document.baseURI).href);
    });
    const script = document.createElement("script");
    script.textContent = `document.addEventListener('keydown', event => { if (event.key.toLowerCase() !== 'd' || event.repeat) return; const root = document.querySelector('[data-ui-theme]') || document.documentElement; root.dataset.uiTheme = root.dataset.uiTheme === 'dark' ? 'light' : 'dark'; });`;
    clone.querySelector("body").append(script);
    return "<!doctype html>\n" + clone.outerHTML;
  });
  return { html, fingerprint: hash(html), capturedAt: new Date().toISOString() };
}

function buildReport(before, after) {
  const srcdoc = value => value.replaceAll("&", "&amp;").replaceAll('"', "&quot;");
  return `<!doctype html><html><head><meta charset="utf-8"><title>Test page visual diff</title><style>body{margin:0;background:#e2e8f0;color:#0f172a;font:14px system-ui}header{padding:12px 16px;background:#0f172a;color:white}main{display:grid;grid-template-columns:1fr 1fr;gap:12px;padding:12px}.pane{min-width:0;background:white;border-radius:8px;overflow:hidden}.pane h2{margin:0;padding:8px 12px;font-size:14px;background:#f8fafc}iframe{display:block;border:0;width:100%;height:calc(100vh - 110px)}code{color:#cbd5e1}</style></head><body><header><strong>/test visual change</strong> <code>${before.capturedAt} → ${after.capturedAt}</code> — press D inside either pane to switch its theme.</header><main><section class="pane"><h2>Before</h2><iframe title="Before" srcdoc="${srcdoc(before.html)}"></iframe></section><section class="pane"><h2>After</h2><iframe title="After" srcdoc="${srcdoc(after.html)}"></iframe></section></main></body></html>`;
}

function readOptions(args) {
  const values = { url: "http://localhost:55174/test?raw" };
  for (let index = 0; index < args.length; index += 2) {
    const key = args[index];
    if (!key.startsWith("--")) throw new Error(`Unexpected argument: ${key}`);
    values[key.slice(2)] = args[index + 1];
  }
  return values;
}

function hash(value) { let result = 2166136261; for (let index = 0; index < value.length; index += 1) result = Math.imul(result ^ value.charCodeAt(index), 16777619); return result >>> 0; }
function delay(ms) { return new Promise(resolve => setTimeout(resolve, ms)); }
function timestamp() { return new Date().toISOString().replaceAll(":", "-").replaceAll(".", "-"); }
