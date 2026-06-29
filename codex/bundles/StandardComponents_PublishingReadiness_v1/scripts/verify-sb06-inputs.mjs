import { chromium } from "playwright";
import { mkdirSync, writeFileSync } from "node:fs";
import { dirname, resolve } from "node:path";

const baseUrl = process.env.SB06_BASE_URL ?? "http://127.0.0.1:5225";
const proofRoot = resolve("codex/bundles/StandardComponents_PublishingReadiness_v1/proof/SB06");
const reportPath = resolve(proofRoot, "data/sb06-input-validation.json");
const fixturePath = resolve(proofRoot, "data/sb06-upload-fixture.musicxml");

mkdirSync(dirname(reportPath), { recursive: true });
writeFileSync(
  fixturePath,
  "<?xml version=\"1.0\" encoding=\"UTF-8\"?><score-partwise version=\"4.0\"><part-list /></score-partwise>",
  "utf8");

const browser = await chromium.launch({ channel: "msedge", headless: true });
const page = await browser.newPage();
const consoleErrors = [];
page.on("console", message => {
  if (message.type() === "error") {
    consoleErrors.push(message.text());
  }
});

const checks = [];

async function check(condition, name, details = {}) {
  checks.push({ name, passed: Boolean(condition), details });
  if (!condition) {
    throw new Error(`${name} failed: ${JSON.stringify(details)}`);
  }
}

async function gotoInputs(path, viewport) {
  await page.setViewportSize(viewport);
  await page.goto(`${baseUrl}${path}`, { waitUntil: "networkidle" });
  await page.locator("[data-testid='sandbox-inputs-specialized']").waitFor({ state: "visible" });
}

async function collectOverflow(testId) {
  return page.locator(`[data-testid='${testId}']`).evaluate(element => {
    const rect = element.getBoundingClientRect();
    return {
      testId: element.getAttribute("data-testid"),
      viewportWidth: window.innerWidth,
      documentScrollWidth: document.documentElement.scrollWidth,
      rect: {
        left: rect.left,
        right: rect.right,
        width: rect.width
      },
      clientWidth: element.clientWidth,
      scrollWidth: element.scrollWidth
    };
  });
}

async function assertNoHorizontalOverflow(path, viewport, testIds) {
  await gotoInputs(path, viewport);
  const documentOverflow = await page.evaluate(() => ({
    viewportWidth: window.innerWidth,
    documentScrollWidth: document.documentElement.scrollWidth
  }));
  await check(
    documentOverflow.documentScrollWidth <= documentOverflow.viewportWidth + 1,
    `No page horizontal overflow at ${path} ${viewport.width}x${viewport.height}`,
    documentOverflow);

  for (const testId of testIds) {
    await page.locator(`[data-testid='${testId}']`).scrollIntoViewIfNeeded();
    const overflow = await collectOverflow(testId);
    await check(
      overflow.scrollWidth <= overflow.clientWidth + 1 && overflow.rect.right <= overflow.viewportWidth + 1,
      `No component horizontal overflow for ${testId} at ${path}`,
      overflow);
  }
}

await assertNoHorizontalOverflow(
  "/groups/inputs?proof=script",
  { width: 1366, height: 900 },
  [
    "sandbox-inputs-specialized",
    "sandbox-inputs-slider",
    "sandbox-inputs-prefixed-field",
    "sandbox-inputs-tag-editor",
    "sandbox-inputs-entity-picker",
    "sandbox-inputs-settings-switch",
    "sandbox-inputs-file-upload"
  ]);

await assertNoHorizontalOverflow(
  "/groups/inputs?scenario=long-text&proof=script",
  { width: 390, height: 844 },
  [
    "sandbox-inputs-specialized",
    "sandbox-inputs-prefixed-field",
    "sandbox-inputs-tag-editor",
    "sandbox-inputs-entity-picker",
    "sandbox-inputs-settings-switch",
    "sandbox-inputs-file-upload"
  ]);

await gotoInputs("/groups/inputs?proof=actions", { width: 1366, height: 900 });

await page.locator("[data-testid='sandbox-inputs-slider-control']").fill("85");
await page.getByText("85%", { exact: true }).waitFor({ state: "visible" });
await check(
  await page.getByText("85%", { exact: true }).isVisible(),
  "Slider change callback updates displayed value");

await page.locator("[data-testid='sandbox-inputs-tag-editor-input']").click();
await page.getByRole("button", { name: /#api-contract/i }).click();
await page
  .locator("[data-testid='sandbox-inputs-tag-editor'] .cad-tag-textedit__chip", { hasText: "api-contract" })
  .first()
  .waitFor({ state: "visible" });
await check(
  await page.locator("[data-testid='sandbox-inputs-tag-editor'] .cad-tag-textedit__chip", { hasText: "api-contract" }).count() === 1,
  "TagEditor suggestion click adds a chip");

await page.getByRole("option", { name: /Platform foundation/i }).click();
await page.waitForFunction(() => {
  const options = Array.from(document.querySelectorAll("[role='option']"));
  return options.some(option =>
    option.textContent?.includes("Platform foundation")
    && option.getAttribute("aria-selected")?.toLowerCase() === "true");
});
const platformSelected = await page.getByRole("option", { name: /Platform foundation/i }).getAttribute("aria-selected");
await check(
  platformSelected?.toLowerCase() === "true",
  "EntityPicker selection updates aria-selected",
  { ariaSelected: platformSelected });

await page.locator("[data-testid='sandbox-inputs-settings-switch-control']").click();
await page.getByText(/quiet/i).waitFor({ state: "visible" });
await check(
  await page.getByText(/quiet/i).isVisible(),
  "Settings switch click updates summary");

await page.locator("[data-testid='sandbox-inputs-prefixed-field-input']").fill("PUBLISH-READY");
await check(
  await page.locator("[data-testid='sandbox-inputs-prefixed-field-input']").inputValue() === "PUBLISH-READY",
  "PrefixedField child input accepts typed value");

await page.locator("[data-testid='sandbox-inputs-file-upload-input']").setInputFiles(fixturePath);
await page.getByText(/sb06-upload-fixture\.musicxml/i).waitFor({ state: "visible" });
await check(
  await page.getByText(/sb06-upload-fixture\.musicxml/i).isVisible(),
  "FileUpload native input routes to shared callback");

await gotoInputs("/groups/inputs?scenario=disabled-state&proof=disabled", { width: 390, height: 844 });
const disabledStates = await page.evaluate(() => ({
  slider: document.querySelector("[data-testid='sandbox-inputs-slider-control']")?.disabled ?? false,
  tagInput: document.querySelector("[data-testid='sandbox-inputs-tag-editor-input']")?.disabled ?? false,
  fileInput: document.querySelector("[data-testid='sandbox-inputs-file-upload-input']")?.disabled ?? false,
  switchInput: document.querySelector("[data-testid='sandbox-inputs-settings-switch-control'] input")?.disabled ?? false
}));
await check(
  Object.values(disabledStates).every(Boolean),
  "Disabled scenario disables specialized controls",
  disabledStates);

await browser.close();

const report = {
  generatedAt: new Date().toISOString(),
  baseUrl,
  browserChannel: "msedge",
  checks,
  consoleErrors
};
writeFileSync(reportPath, `${JSON.stringify(report, null, 2)}\n`, "utf8");
console.log(JSON.stringify({
  reportPath,
  checks: checks.length,
  consoleErrors: consoleErrors.length
}, null, 2));
