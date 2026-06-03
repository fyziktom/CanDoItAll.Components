async page => {
    const screenshotPath = "C:/repositories/CanDoItAll.Components/codex/bundles/WebGlEngine_Economy_Followup_Hardening_v5/proof/SB07/browser/simulation-sandbox-pathless-catalog-after.png";
    const assertionsPath = "C:/repositories/CanDoItAll.Components/codex/bundles/WebGlEngine_Economy_Followup_Hardening_v5/proof/SB07/browser/simulation-sandbox-pathless-catalog-assertions.json";

    await page.setViewportSize({ width: 1920, height: 1080 });
    await page.goto("http://127.0.0.1:5312/economy/simulation-sandbox", { waitUntil: "networkidle" });
    await page.getByTestId("economy-simulation-sandbox-page").waitFor({ state: "visible", timeout: 10000 });
    await page.waitForTimeout(500);

    const sample = await page.evaluate(() => {
        const text = document.body.textContent || "";
        const selectedScenario = document.querySelector("[data-testid='sandbox-selected-scenario']")?.textContent?.trim() || "";
        const packHash = document.querySelector("[data-testid='sandbox-scenario-pack-hash']")?.textContent?.trim() || "";
        const validity = document.querySelector("[data-testid='sandbox-scenario-validity']")?.textContent?.trim() || "";
        const status = document.querySelector("[data-testid='sandbox-status']")?.textContent?.trim() || "";
        return {
            selectedScenario,
            packHash,
            validity,
            status,
            hasExperimentJsonPathText: /ExperimentJsonPath/i.test(text),
            hasTestFixturePathText: /CanDoItAll\.Economy\.Tests|ExperimentInputs|\\tests\\|\/tests\//i.test(text),
            hasAbsoluteExperimentJsonText: /[A-Z]:\\.*experiment\.json|\/.*experiment\.json/i.test(text),
            bodyTextPrefix: text.slice(0, 2000)
        };
    });

    await page.screenshot({ path: screenshotPath, fullPage: true });

    const result = {
        route: "http://127.0.0.1:5312/economy/simulation-sandbox",
        viewport: { width: 1920, height: 1080 },
        assertionsFile: assertionsPath,
        screenshot: screenshotPath,
        sample,
        assertions: {
            pageLoadedSharedWellScenario: sample.selectedScenario === "shared-well",
            packHashRendered: sample.packHash.startsWith("sha256:"),
            scenarioValid: sample.validity === "valid",
            legacyExperimentPathNotRendered: sample.hasExperimentJsonPathText === false,
            testFixturePathNotRendered: sample.hasTestFixturePathText === false,
            absoluteExperimentJsonPathNotRendered: sample.hasAbsoluteExperimentJsonText === false
        }
    };

    const failedAssertions = Object.entries(result.assertions)
        .filter(([, passed]) => !passed)
        .map(([name]) => name);
    if (failedAssertions.length > 0) {
        throw new Error(`SB07 pathless catalog browser proof failed assertions: ${failedAssertions.join(", ")}`);
    }

    return result;
}
