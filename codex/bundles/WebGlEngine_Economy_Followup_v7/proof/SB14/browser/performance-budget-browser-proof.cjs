const fs = require("fs");
const path = require("path");
const { performance } = require("perf_hooks");
const { chromium } = require("playwright");

const outputPath = path.resolve(__dirname, "performance-budget-browser-proof.json");
const screenshotPath = path.resolve(__dirname, "performance-budget-browser-proof.png");
const url = process.env.SB14_WEBGL_URL || "http://127.0.0.1:5298/run-playback";
const chromePath = process.env.PLAYWRIGHT_CHROME_PATH || "C:\\Program Files\\Google\\Chrome\\Application\\chrome.exe";
const browserLoadBudgetMs = Number(process.env.SB14_BROWSER_LOAD_BUDGET_MS || 10000);
const browserBatchSettleBudgetMs = Number(process.env.SB14_BROWSER_BATCH_SETTLE_BUDGET_MS || 5000);

function assert(condition, message) {
    if (!condition) {
        throw new Error(message);
    }
}

async function readDiagnosticsJson(page) {
    const text = await page.locator('[data-testid="webgl-run-diagnostics-json"]').textContent();
    return JSON.parse(text || "{}");
}

async function waitForRuntimeIdle(page, reason) {
    return page.evaluate(async idleReason => {
        const host = document.querySelector('[data-testid="webgl-scene-host"]');
        if (!host || !window.CanDoItAll?.webglScene) {
            return { available: false, idle: null, diagnostics: null };
        }

        const idle = await window.CanDoItAll.webglScene.waitForRuntimeIdle(host, {
            timeoutMs: 5000,
            pollIntervalMs: 16,
            reason: idleReason
        });
        const diagnostics = window.CanDoItAll.webglScene.getDiagnostics(host);
        return { available: true, idle, diagnostics };
    }, reason);
}

(async () => {
    const browser = await chromium.launch({
        headless: true,
        executablePath: fs.existsSync(chromePath) ? chromePath : undefined
    });
    const page = await browser.newPage({ viewport: { width: 1440, height: 900 } });
    const consoleMessages = [];
    const pageErrors = [];
    page.on("console", message => consoleMessages.push({ type: message.type(), text: message.text() }));
    page.on("pageerror", error => pageErrors.push(error.message));

    try {
        const loadStart = performance.now();
        await page.goto(url, { waitUntil: "networkidle" });
        await page.locator('[data-testid="webgl-scene-host"]').waitFor({ state: "visible", timeout: 30000 });
        await page.waitForFunction(() => !!window.CanDoItAll?.webglScene?.getDiagnostics, null, { timeout: 30000 });
        const browserLoadElapsedMs = performance.now() - loadStart;

        await page.getByRole("button", { name: "Snapshot" }).click();
        await page.waitForFunction(() => {
            const node = document.querySelector('[data-testid="webgl-run-diagnostics-json"]');
            if (!node?.textContent) {
                return false;
            }

            const diagnostics = JSON.parse(node.textContent);
            return diagnostics?.observer?.observerProofValid === true;
        }, null, { timeout: 30000 });

        const batchStart = performance.now();
        await page.getByRole("button", { name: "Step" }).click();
        await page.waitForFunction(() => {
            const frame = document.querySelector('[data-testid="webgl-run-frame"]')?.textContent?.trim();
            return frame === "1";
        }, null, { timeout: 30000 });
        const idle = await waitForRuntimeIdle(page, "sb14-browser-batch-settle");
        await page.getByRole("button", { name: "Snapshot" }).click();
        const browserBatchSettleElapsedMs = performance.now() - batchStart;
        const diagnosticsJson = await readDiagnosticsJson(page);

        await page.screenshot({ path: screenshotPath, fullPage: true });

        const artifact = {
            schemaVersion: "sb14-browser-performance-budget-proof/v1",
            route: url,
            viewport: { width: 1440, height: 900 },
            budgets: {
                browserLoadMs: browserLoadBudgetMs,
                browserBatchSettleMs: browserBatchSettleBudgetMs
            },
            measurements: [
                {
                    name: "browser-load",
                    category: "browser",
                    elapsedMilliseconds: Math.round(browserLoadElapsedMs * 1000) / 1000,
                    budgetMilliseconds: browserLoadBudgetMs,
                    hardFailure: false,
                    message: browserLoadElapsedMs > browserLoadBudgetMs ? "browser budget 'browser-load' exceeded." : ""
                },
                {
                    name: "browser-batch-settle",
                    category: "browser",
                    elapsedMilliseconds: Math.round(browserBatchSettleElapsedMs * 1000) / 1000,
                    budgetMilliseconds: browserBatchSettleBudgetMs,
                    hardFailure: false,
                    message: browserBatchSettleElapsedMs > browserBatchSettleBudgetMs ? "browser budget 'browser-batch-settle' exceeded." : ""
                }
            ],
            runtimeIdle: idle,
            diagnosticsJson,
            assertions: {
                browserLoadWithinBudget: browserLoadElapsedMs <= browserLoadBudgetMs,
                browserBatchSettledWithinBudget: browserBatchSettleElapsedMs <= browserBatchSettleBudgetMs,
                runtimeIdleAvailable: idle.available === true,
                runtimeIdleSettled: idle.idle?.idle === true && idle.idle?.timedOut === false,
                noRuntimeErrors:
                    diagnosticsJson?.observer?.browserRuntimeValid === true &&
                    diagnosticsJson?.observer?.errors?.length === 0
            },
            consoleMessages,
            pageErrors
        };

        fs.writeFileSync(outputPath, `${JSON.stringify(artifact, null, 2)}\n`, "utf8");

        for (const [name, passed] of Object.entries(artifact.assertions)) {
            assert(passed, `Assertion failed: ${name}`);
        }
        assert(pageErrors.length === 0, `Page errors were captured: ${pageErrors.join("; ")}`);

        console.log(JSON.stringify({
            outputPath,
            screenshotPath,
            measurements: artifact.measurements,
            passedAssertions: Object.keys(artifact.assertions)
        }, null, 2));
    } finally {
        await browser.close();
    }
})().catch(error => {
    console.error(error);
    process.exit(1);
});
