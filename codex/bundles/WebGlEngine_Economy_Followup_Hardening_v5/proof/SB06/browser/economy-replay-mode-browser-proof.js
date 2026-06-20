async page => {
    const screenshotPath = "C:/repositories/CanDoItAll.Components/codex/bundles/WebGlEngine_Economy_Followup_Hardening_v5/proof/SB06/browser/economy-replay-mode-after.png";
    const assertionsPath = "C:/repositories/CanDoItAll.Components/codex/bundles/WebGlEngine_Economy_Followup_Hardening_v5/proof/SB06/browser/economy-replay-mode-assertions.json";

    async function sample(label) {
        return await page.evaluate(currentLabel => {
            function readDefinitionList(testId) {
                const section = document.querySelector(`[data-testid='${testId}']`);
                const values = {};
                for (const row of section?.querySelectorAll("dl > div") || []) {
                    const key = row.querySelector("dt")?.textContent?.trim() || "";
                    const value = row.querySelector("dd")?.textContent?.trim() || "";
                    if (key) {
                        values[key] = value;
                    }
                }

                return values;
            }

            const summary = {};
            for (const item of document.querySelectorAll("[data-testid='sandbox-summary'] > div")) {
                const key = item.querySelector("span")?.textContent?.trim() || "";
                const value = item.querySelector("strong")?.textContent?.trim() || "";
                if (key) {
                    summary[key] = value;
                }
            }

            return {
                label: currentLabel,
                status: document.querySelector("[data-testid='sandbox-status']")?.textContent?.trim() || "",
                summary,
                browserDiagnostics: readDefinitionList("sandbox-browser-runtime"),
                pageText: document.body.textContent || ""
            };
        }, label);
    }

    async function waitForDiagnostic(expectedMode, predicate, timeoutMs = 30000) {
        const deadline = Date.now() + timeoutMs;
        let last = await sample(`wait-${expectedMode}-initial`);
        while (Date.now() < deadline) {
            last = await sample(`wait-${expectedMode}`);
            const diagnostics = last.browserDiagnostics || {};
            if (diagnostics.replayMode === expectedMode && predicate(diagnostics)) {
                return last;
            }

            await page.waitForTimeout(100);
        }

        return last;
    }

    await page.setViewportSize({ width: 1920, height: 1080 });
    await page.goto("http://127.0.0.1:5311/economy/simulation-sandbox", { waitUntil: "networkidle" });
    await page.getByTestId("economy-simulation-sandbox-page").waitFor({ state: "visible", timeout: 10000 });
    const initial = await sample("initial");

    await page.getByRole("button", { name: /Apply frame/i }).click();
    const afterManualApply = await waitForDiagnostic("full", diagnostics =>
        Number(diagnostics.frameReplayCount || 0) >= 1 &&
        diagnostics.resetApplied === "True" &&
        diagnostics.targetFrameIndex === "0" &&
        diagnostics.appliedFrameIndexes === "0" &&
        diagnostics.lastStableFrameIndex === "0");

    await page.getByRole("button", { name: /^Step$/i }).click();
    const afterForwardStep = await waitForDiagnostic("incremental", diagnostics =>
        Number(diagnostics.frameReplayCount || 0) === 1 &&
        diagnostics.resetApplied === "False" &&
        diagnostics.targetFrameIndex === "1" &&
        diagnostics.appliedFrameIndexes === "1" &&
        diagnostics.lastStableFrameIndex === "1");

    await page.getByRole("button", { name: /^Last$/i }).click();
    const afterSeekLast = await waitForDiagnostic("full", diagnostics =>
        Number(diagnostics.frameReplayCount || 0) > Number(afterForwardStep.browserDiagnostics.frameReplayCount || 0) &&
        diagnostics.resetApplied === "True" &&
        diagnostics.targetFrameIndex === "2" &&
        diagnostics.appliedFrameIndexes === "0,1,2" &&
        diagnostics.lastStableFrameIndex === "2");

    await page.screenshot({ path: screenshotPath, fullPage: true });

    const manual = afterManualApply.browserDiagnostics;
    const forward = afterForwardStep.browserDiagnostics;
    const seek = afterSeekLast.browserDiagnostics;
    const result = {
        route: "http://127.0.0.1:5311/economy/simulation-sandbox",
        viewport: { width: 1920, height: 1080 },
        assertionsFile: assertionsPath,
        screenshot: screenshotPath,
        samples: {
            initial,
            afterManualApply,
            afterForwardStep,
            afterSeekLast
        },
        assertions: {
            manualApplyUsesFullReplay: manual.replayMode === "full",
            manualApplyUsesReset: manual.resetApplied === "True",
            forwardStepUsesIncrementalReplay: forward.replayMode === "incremental",
            forwardStepAppliesSingleDeltaFrame: Number(forward.frameReplayCount || 0) === 1,
            forwardStepDoesNotResetScene: forward.resetApplied === "False",
            forwardStepHasStableFrame: forward.lastStableFrameIndex === "1",
            forwardStepTargetFrameApplied: forward.targetFrameIndex === "1" && forward.appliedFrameIndexes === "1",
            seekLastUsesFullReplay: seek.replayMode === "full",
            seekLastUsesReset: seek.resetApplied === "True",
            seekLastReplaysMoreThanForwardDelta: Number(seek.frameReplayCount || 0) > Number(forward.frameReplayCount || 0),
            seekLastAppliesThroughTargetFrame: seek.targetFrameIndex === "2" && seek.appliedFrameIndexes === "0,1,2",
            seekLastStableFrameUpdated: seek.lastStableFrameIndex === "2",
            diagnosticsExposeRequiredFields:
                "appliedFrameIndexes" in seek &&
                "requestedFrameIndexes" in seek &&
                "frameReplayCount" in seek &&
                "lastStableFrameIndex" in seek &&
                "replayMode" in seek &&
                "resetApplied" in seek
        }
    };

    const failedAssertions = Object.entries(result.assertions)
        .filter(([, passed]) => !passed)
        .map(([name]) => name);
    if (failedAssertions.length > 0) {
        throw new Error(`SB06 replay proof failed assertions: ${failedAssertions.join(", ")}`);
    }

    return result;
}
