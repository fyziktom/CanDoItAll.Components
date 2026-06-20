async page => {
    const route = "http://127.0.0.1:5313/economy/simulation-sandbox";
    const screenshotPath = "C:/repositories/CanDoItAll.Components/codex/bundles/WebGlEngine_Economy_Followup_Hardening_v5/proof/SB08/browser/simulation-sandbox-manifest-hash-after.png";
    const assertionsPath = "C:/repositories/CanDoItAll.Components/codex/bundles/WebGlEngine_Economy_Followup_Hardening_v5/proof/SB08/browser/simulation-sandbox-manifest-hash-assertions.json";

    await page.setViewportSize({ width: 1920, height: 1080 });
    await page.goto(route, { waitUntil: "networkidle" });

    if (/\/Account\/Login/i.test(page.url())) {
        await page.locator("input[autocomplete='username']").fill("admin@admin.com");
        await page.locator("input[autocomplete='current-password']").fill("Admin@1234");
        await page.getByRole("button", { name: /log in/i }).click();
    }

    await page.getByTestId("economy-simulation-sandbox-page").waitFor({ state: "visible", timeout: 20000 });
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
            hasInvalidScenarioText: /scenario.+invalid/i.test(text),
            bodyTextPrefix: text.slice(0, 2000)
        };
    });

    await page.screenshot({ path: screenshotPath, fullPage: true });

    const result = {
        route,
        viewport: { width: 1920, height: 1080 },
        assertionsFile: assertionsPath,
        screenshot: screenshotPath,
        sample,
        assertions: {
            pageLoadedSharedWellScenario: sample.selectedScenario === "shared-well",
            manifestPackHashRendered: /^sha256:/.test(sample.packHash),
            hardenedManifestPackHashPrefixRendered: sample.packHash.includes("3d14b"),
            hardenedManifestPackHashSuffixRendered: sample.packHash.includes("746a0e"),
            scenarioValidAfterManifestHashVerification: sample.validity === "valid",
            noInvalidScenarioCopy: sample.hasInvalidScenarioText === false
        }
    };

    const failedAssertions = Object.entries(result.assertions)
        .filter(([, passed]) => !passed)
        .map(([name]) => name);
    if (failedAssertions.length > 0) {
        throw new Error(`SB08 manifest hash browser proof failed assertions: ${failedAssertions.join(", ")}`);
    }

    return result;
}
