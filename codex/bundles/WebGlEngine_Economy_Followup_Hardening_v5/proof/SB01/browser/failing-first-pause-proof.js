async page => {
    const screenshotPath = "C:/repositories/CanDoItAll.Components/codex/bundles/WebGlEngine_Economy_Followup_Hardening_v5/proof/SB01/browser/failing-first-pause-after.png";

    async function sample(label) {
        return await page.evaluate(currentLabel => {
            const host = document.querySelector("[data-testid='webgl-scene-host']");
            const frameText = document.querySelector("[data-testid='webgl-run-frame']")?.textContent || "";
            const playingText = document.querySelector("[data-testid='webgl-run-playing']")?.textContent || "";
            const statusText = document.querySelector("[data-testid='webgl-run-status']")?.textContent || "";
            const diagnosticsText = document.querySelector("[data-testid='webgl-run-diagnostics-json']")?.textContent || "{}";
            const facade = window.CanDoItAll?.webglScene;
            const diagnostics = host && facade?.getDiagnostics ? facade.getDiagnostics(host) : null;
            const proofSnapshot = host && facade?.getProofSnapshot ? facade.getProofSnapshot(host) : null;
            const scene = host && facade?.exportScene ? facade.exportScene(host) : null;
            const runner = scene?.objects?.find?.(object => object.id === "object.runner") || null;

            let diagnosticsPanel = null;
            try {
                diagnosticsPanel = JSON.parse(diagnosticsText);
            } catch {
                diagnosticsPanel = null;
            }

            return {
                label: currentLabel,
                browserTimeMs: Math.round(performance.now()),
                frameIndex: Number(frameText.trim()),
                playingText: playingText.trim(),
                statusText: statusText.trim(),
                diagnosticsPanel,
                runtimeDiagnostics: diagnostics,
                proofSnapshot,
                runnerPosition: runner?.position || null
            };
        }, label);
    }

    async function waitForRuntimeWork(timeoutMs) {
        const deadline = Date.now() + timeoutMs;
        let last = await sample("poll-initial");
        while (Date.now() < deadline) {
            last = await sample("poll");
            const runtime = last.runtimeDiagnostics || {};
            if ((runtime.activeMotionCount || 0) > 0 ||
                (runtime.queuedMotionCount || 0) > 0 ||
                (runtime.queuedCommandStageCount || 0) > 0 ||
                last.frameIndex > 0) {
                return last;
            }

            await page.waitForTimeout(25);
        }

        return last;
    }

    await page.setViewportSize({ width: 1920, height: 1080 });
    await page.goto("http://127.0.0.1:5298/run-playback", { waitUntil: "networkidle" });
    await page.getByRole("button", { name: /^Reset$/ }).click();
    await page.waitForTimeout(250);

    const initial = await sample("initial");
    await page.getByRole("button", { name: /^Play$/ }).click();
    const beforePause = await waitForRuntimeWork(3000);
    const pauseClickStartedAt = Date.now();
    await page.getByRole("button", { name: /^Pause$/ }).click();
    const pauseClickReturnedMs = Date.now() - pauseClickStartedAt;
    const afterPauseImmediate = await sample("after-pause-immediate");
    await page.waitForTimeout(120);
    const afterPause120ms = await sample("after-pause-120ms");
    await page.waitForTimeout(520);
    const afterPause640ms = await sample("after-pause-640ms");
    await page.screenshot({
        path: screenshotPath,
        fullPage: true
    });

    function runtimeCounts(snapshot) {
        const runtime = snapshot.runtimeDiagnostics || {};
        return {
            activeMotionCount: runtime.activeMotionCount || 0,
            queuedMotionCount: runtime.queuedMotionCount || 0,
            queuedCommandStageCount: runtime.queuedCommandStageCount || 0,
            renderCount: runtime.renderCount || 0,
            motionCompletedCount: runtime.motionCompletedCount || 0,
            commandStageJournalCount: runtime.commandStageJournalCount || 0,
            currentCommandBatchId: runtime.currentCommandBatchId || "",
            currentCommandStageId: runtime.currentCommandStageId || "",
            lastStageCancelReason: runtime.lastStageCancelReason || ""
        };
    }

    function positionChanged(left, right) {
        if (!left.runnerPosition || !right.runnerPosition) {
            return false;
        }

        return ["x", "y", "z"].some(axis => Number(left.runnerPosition[axis]) !== Number(right.runnerPosition[axis]));
    }

    const result = {
        route: "http://127.0.0.1:5298/run-playback",
        viewport: { width: 1920, height: 1080 },
        screenshot: screenshotPath,
        pauseClickReturnedMs,
        samples: {
            initial,
            beforePause,
            afterPauseImmediate,
            afterPause120ms,
            afterPause640ms
        },
        assertions: {
            runtimeWorkObservedBeforePause:
                runtimeCounts(beforePause).activeMotionCount > 0 ||
                runtimeCounts(beforePause).queuedMotionCount > 0 ||
                runtimeCounts(beforePause).queuedCommandStageCount > 0 ||
                beforePause.frameIndex > 0,
            runtimeStillBusyImmediatelyAfterPause:
                runtimeCounts(afterPauseImmediate).activeMotionCount > 0 ||
                runtimeCounts(afterPauseImmediate).queuedMotionCount > 0 ||
                runtimeCounts(afterPauseImmediate).queuedCommandStageCount > 0,
            runtimeStillBusy120msAfterPause:
                runtimeCounts(afterPause120ms).activeMotionCount > 0 ||
                runtimeCounts(afterPause120ms).queuedMotionCount > 0 ||
                runtimeCounts(afterPause120ms).queuedCommandStageCount > 0,
            frameProgressedAfterPauseClick: afterPause640ms.frameIndex > afterPauseImmediate.frameIndex,
            renderProgressedAfterPauseClick:
                runtimeCounts(afterPause640ms).renderCount > runtimeCounts(afterPauseImmediate).renderCount,
            runnerMovedAfterPauseClick: positionChanged(afterPauseImmediate, afterPause640ms),
            csharpStateReachedPaused: afterPause640ms.playingText === "False",
            runtimeStopSignalMissing:
                runtimeCounts(afterPause640ms).lastStageCancelReason === "" &&
                runtimeCounts(afterPause640ms).activeMotionCount === 0 &&
                runtimeCounts(afterPause640ms).queuedCommandStageCount === 0
        }
    };

    return result;
}
