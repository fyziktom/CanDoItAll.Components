async page => {
    const screenshotPath = "C:/repositories/CanDoItAll.Components/codex/bundles/WebGlEngine_Economy_Followup_Hardening_v5/proof/SB02/browser/runtime-stop-after.png";

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
                (runtime.queuedCommandStageCount || 0) > 0) {
                return last;
            }

            await page.waitForTimeout(25);
        }

        return last;
    }

    async function stopRuntime(reason) {
        return await page.evaluate(currentReason => {
            const host = document.querySelector("[data-testid='webgl-scene-host']");
            return window.CanDoItAll?.webglScene?.stopRuntimeActivity?.(host, currentReason) || null;
        }, reason);
    }

    await page.setViewportSize({ width: 1920, height: 1080 });
    await page.goto("http://127.0.0.1:5298/run-playback", { waitUntil: "networkidle" });
    await page.getByRole("button", { name: /^Reset$/ }).click();
    await page.waitForTimeout(250);
    await page.getByRole("button", { name: /^Play$/ }).click();

    const beforeStop = await waitForRuntimeWork(3000);
    const firstStopResult = await stopRuntime("sb02-proof-stop");
    await page.waitForTimeout(120);
    const afterFirstStop = await sample("after-first-stop");
    const secondStopResult = await stopRuntime("sb02-proof-stop-again");
    await page.waitForTimeout(80);
    const afterSecondStop = await sample("after-second-stop");
    await page.screenshot({ path: screenshotPath, fullPage: true });

    function counts(snapshot) {
        const runtime = snapshot.diagnostics || {};
        return {
            activeMotionCount: runtime.activeMotionCount || 0,
            queuedMotionCount: runtime.queuedMotionCount || 0,
            queuedCommandStageCount: runtime.queuedCommandStageCount || 0,
            commandStageBarrierPolicy: runtime.commandStageBarrierPolicy || "",
            runtimeStopCount: runtime.runtimeStopCount || 0,
            lastRuntimeStopReason: runtime.lastRuntimeStopReason || "",
            clearedMotionCount: runtime.clearedMotionCount || 0,
            lastRuntimeStopClearedMotionCount: runtime.lastRuntimeStopClearedMotionCount || 0,
            lastRuntimeStopCancelledCommandStageCount: runtime.lastRuntimeStopCancelledCommandStageCount || 0
        };
    }

    return {
        route: "http://127.0.0.1:5298/run-playback",
        viewport: { width: 1920, height: 1080 },
        screenshot: screenshotPath,
        samples: { beforeStop, afterFirstStop, afterSecondStop },
        stopResults: { firstStopResult, secondStopResult },
        assertions: {
            runtimeWorkObservedBeforeStop:
                counts(beforeStop).activeMotionCount > 0 ||
                counts(beforeStop).queuedMotionCount > 0 ||
                counts(beforeStop).queuedCommandStageCount > 0,
            firstStopSucceeded: firstStopResult?.success === true,
            secondStopSucceeded: secondStopResult?.success === true,
            firstStopClearedRuntimeWork:
                counts(afterFirstStop).activeMotionCount === 0 &&
                counts(afterFirstStop).queuedMotionCount === 0 &&
                counts(afterFirstStop).queuedCommandStageCount === 0 &&
                counts(afterFirstStop).commandStageBarrierPolicy === "",
            secondStopRemainedIdle:
                counts(afterSecondStop).activeMotionCount === 0 &&
                counts(afterSecondStop).queuedMotionCount === 0 &&
                counts(afterSecondStop).queuedCommandStageCount === 0,
            stopReasonRecorded: counts(afterFirstStop).lastRuntimeStopReason === "sb02-proof-stop",
            secondStopReasonRecorded: counts(afterSecondStop).lastRuntimeStopReason === "sb02-proof-stop-again"
        }
    };
}
