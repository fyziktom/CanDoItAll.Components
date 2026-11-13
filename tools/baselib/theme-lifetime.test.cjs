const assert = require("node:assert/strict");
const path = require("node:path");
const { after, before, test } = require("node:test");
const { chromium } = require("playwright");

let browser;
before(async () => {
    browser = await chromium.launch();
});
after(async () => {
    await browser?.close();
});

async function fixture(t) {
    const page = await browser.newPage();
    t.after(() => page.close());
    await page.setContent('<main data-ui-theme="light"><div id="chart"></div></main>');
    await page.evaluate(() => {
        window.themeProbe = { calls: [], disconnects: 0, retired: false };
        const NativeMutationObserver = window.MutationObserver;
        window.MutationObserver = class extends NativeMutationObserver {
            disconnect() {
                window.themeProbe.disconnects++;
                super.disconnect();
            }
        };
    });
    await page.addScriptTag({ path: path.join(__dirname,
        "../../src/CanDoItAll.Components.BaseLib/wwwroot/js/theme/theme-tokens.js") });
    await page.addScriptTag({ path: path.join(__dirname,
        "../../src/CanDoItAll.Components.Charts/wwwroot/js/charts-theme.js") });
    await page.evaluate(() => {
        window.chartHost = document.querySelector("#chart");
        window.chartReference = {
            invokeMethodAsync(method) {
                if (window.themeProbe.retired) {
                    return Promise.reject(new Error("The chart reference has been disposed."));
                }
                window.themeProbe.calls.push(method);
                return Promise.resolve();
            }
        };
    });
    return page;
}

test("removed chart retires its real observer before a later theme change", async t => {
    const page = await fixture(t);
    const errors = [];
    page.on("pageerror", error => errors.push(error.message));

    const result = await page.evaluate(async () => {
        CanDoItAll.charts.watchTheme(window.chartHost, window.chartReference);
        document.querySelector("main").remove();
        window.themeProbe.retired = true;
        CanDoItAll.charts.unwatchTheme(null);
        await new Promise(resolve => requestAnimationFrame(resolve));
        const disconnectedBeforeThemeChange = window.themeProbe.disconnects;
        document.documentElement.setAttribute("data-ui-theme", "dark");
        await new Promise(resolve => requestAnimationFrame(resolve));
        document.documentElement.setAttribute("data-ui-theme", "light");
        await new Promise(resolve => requestAnimationFrame(resolve));
        return { disconnectedBeforeThemeChange, calls: window.themeProbe.calls };
    });

    assert.equal(result.disconnectedBeforeThemeChange, 1);
    assert.deepEqual(result.calls, []);
    assert.deepEqual(errors, []);
});

test("a removed chart ignores a queued theme mutation", async t => {
    const page = await fixture(t);
    const errors = [];
    page.on("pageerror", error => errors.push(error.message));

    const result = await page.evaluate(async () => {
        CanDoItAll.charts.watchTheme(window.chartHost, window.chartReference);
        document.querySelector("main").setAttribute("data-ui-theme", "dark");
        window.chartHost.remove();
        window.themeProbe.retired = true;
        await new Promise(resolve => requestAnimationFrame(resolve));
        return window.themeProbe;
    });

    assert.equal(result.disconnects, 1);
    assert.deepEqual(result.calls, []);
    assert.deepEqual(errors, []);
});

test("hidden mounted charts follow their nearest theme scope", async t => {
    const page = await fixture(t);
    const calls = await page.evaluate(async () => {
        window.chartHost.hidden = true;
        CanDoItAll.charts.watchTheme(window.chartHost, window.chartReference);
        document.documentElement.setAttribute("data-ui-theme", "dark");
        await new Promise(resolve => requestAnimationFrame(resolve));
        const outsideScopeCalls = window.themeProbe.calls.length;
        document.querySelector("main").setAttribute("data-ui-theme", "dark");
        await new Promise(resolve => requestAnimationFrame(resolve));
        document.querySelector("main").setAttribute("data-ui-theme", "light");
        await new Promise(resolve => requestAnimationFrame(resolve));
        return { outsideScopeCalls, probe: window.themeProbe };
    });

    assert.equal(calls.outsideScopeCalls, 0);
    assert.equal(calls.probe.disconnects, 0);
    assert.deepEqual(calls.probe.calls, ["HandleChartThemeChangedAsync", "HandleChartThemeChangedAsync"]);
});

test("explicit disconnect is idempotent and cancels queued callbacks", async t => {
    const page = await fixture(t);
    const result = await page.evaluate(async () => {
        CanDoItAll.charts.watchTheme(window.chartHost, window.chartReference);
        document.querySelector("main").setAttribute("data-ui-theme", "dark");
        CanDoItAll.charts.unwatchTheme(window.chartHost);
        CanDoItAll.charts.unwatchTheme(window.chartHost);
        window.themeProbe.retired = true;
        await new Promise(resolve => requestAnimationFrame(resolve));
        return window.themeProbe;
    });

    assert.equal(result.disconnects, 1);
    assert.deepEqual(result.calls, []);
});

test("already detached hosts do not retain an active observer", async t => {
    const page = await fixture(t);
    const result = await page.evaluate(async () => {
        window.chartHost.remove();
        CanDoItAll.charts.watchTheme(window.chartHost, window.chartReference);
        document.querySelector("main").setAttribute("data-ui-theme", "dark");
        await new Promise(resolve => requestAnimationFrame(resolve));
        return window.themeProbe;
    });

    assert.equal(result.disconnects, 1);
    assert.deepEqual(result.calls, []);
});

test("active chart callback failures remain observable", async t => {
    const page = await fixture(t);
    const failure = page.waitForEvent("pageerror", { timeout: 5000 });
    await page.evaluate(() => {
        CanDoItAll.charts.watchTheme(window.chartHost, {
            invokeMethodAsync() {
                return Promise.reject(new Error("Active chart callback failure."));
            }
        });
        document.querySelector("main").setAttribute("data-ui-theme", "dark");
    });

    assert.equal((await failure).message, "Active chart callback failure.");
    assert.equal(await page.evaluate(() => window.themeProbe.disconnects), 0);
});
