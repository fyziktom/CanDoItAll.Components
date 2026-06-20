async page => {
    const screenshotPath = "C:/repositories/CanDoItAll.Components/codex/bundles/WebGlEngine_Economy_Followup_Hardening_v5/proof/SB03/browser/runplayback-pause-after.png";

    async function sample(label) {
        return await page.evaluate(currentLabel => {
            const host = document.querySelector("[data-testid='webgl-scene-host']");
            const facade = window.CanDoItAll?.webglScene;
            const diagnostics = host && facade?.getDiagnostics ? facade.getDiagnostics(host) : null;
            const proofSnapshot = host && facade?.getProofSnapshot ? facade.getProofSnapshot(host) : null;
            const frameText = document.querySelector("[data-testid='webgl-run-frame']")?.textContent || "";
            const playingText = document.querySelector("[data-testid='webgl-run-playing']")?.textContent || "";
            const statusText = document.querySelector("[data-testid='webgl-run-status']")?.textContent || "";
            return {
                label: currentLabel,
                browserTimeMs: Math.round(performance.now()),
                frameIndex: Number(frameText.trim()),
                playingText: playingText.trim(),
                statusText: statusText.trim(),
                diagnostics,
                proofSnapshot
            };
        }, label);
    }

    async function waitForRuntimeWork(timeoutMs) {
        const deadline = Date.now() + timeoutMs;
        let last = await sample("poll-initial");
        while (Date.now() < deadline) {
            last = await sample("poll");
            const runtime = last.diagnostics || {};
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

    function counts(snapshot) {
        const runtime = snapshot.diagnostics || {};
        return {
            activeMotionCount: runtime.activeMotionCount || 0,
            queuedMotionCount: runtime.queuedMotionCount || 0,
            queuedCommandStageCount: runtime.queuedCommandStageCount || 0,
            commandStageBarrierPolicy: runtime.commandStageBarrierPolicy || "",
            runtimeStopCount: runtime.runtimeStopCount || 0,
            lastRuntimeStopReason: runtime.lastRuntimeStopReason || "",
            commandStageJournalCount: runtime.commandStageJournalCount || 0,
            motionCompletedCount: runtime.motionCompletedCount || 0,
            renderCount: runtime.renderCount || 0
        };
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
    await page.waitForTimeout(160);
    const afterPause160ms = await sample("after-pause-160ms");
    await page.waitForTimeout(640);
    const afterPause800ms = await sample("after-pause-800ms");
    await page.screenshot({ path: screenshotPath, fullPage: true });

    return {
        route: "http://127.0.0.1:5298/run-playback",
        viewport: { width: 1920, height: 1080 },
        screenshot: screenshotPath,
        pauseClickReturnedMs,
        samples: {
            initial,
            beforePause,
            afterPauseImmediate,
            afterPause160ms,
            afterPause800ms
        },
        assertions: {
            runtimeWorkObservedBeforePause:
                counts(beforePause).activeMotionCount > 0 ||
                counts(beforePause).queuedMotionCount > 0 ||
                counts(beforePause).queuedCommandStageCount > 0 ||
                beforePause.frameIndex > 0,
            pauseClickReturnedWithinBound: pauseClickReturnedMs < 1000,
            csharpStatePausedAfterDeadline: afterPause800ms.playingText === "False",
            statusNotOverwrittenAfterPause: afterPause800ms.statusText === "Paused.",
            runtimeStopReasonRecorded: counts(afterPause800ms).lastRuntimeStopReason === "Paused.",
            runtimeStopCountRecorded: counts(afterPause800ms).runtimeStopCount >= 1,
            runtimeWorkClearedAfterPause:
                counts(afterPause800ms).activeMotionCount === 0 &&
                counts(afterPause800ms).queuedMotionCount === 0 &&
                counts(afterPause800ms).queuedCommandStageCount === 0 &&
                counts(afterPause800ms).commandStageBarrierPolicy === "",
            frameStableAfterPauseDeadline: afterPause800ms.frameIndex === afterPause160ms.frameIndex,
            stageJournalStableAfterPauseDeadline:
                counts(afterPause800ms).commandStageJournalCount === counts(afterPause160ms).commandStageJournalCount,
            motionCompletedStableAfterPauseDeadline:
                counts(afterPause800ms).motionCompletedCount === counts(afterPause160ms).motionCompletedCount
        }
    };
}
