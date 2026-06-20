async page => {
    await page.setViewportSize({ width: 1600, height: 1000 });
    await page.waitForSelector('[data-testid="webgl-scene-host"]', { timeout: 10000 });

    const collect = async label => page.evaluate(async snapshotLabel => {
        const host = document.querySelector('[data-testid="webgl-scene-host"]');
        const api = window.CanDoItAll?.webglScene;
        const diagnostics = api?.getDiagnostics(host) ?? null;
        const idle = api?.waitForRuntimeIdle
            ? await api.waitForRuntimeIdle(host, {
                timeoutMs: 50,
                pollIntervalMs: 5,
                reason: `SB01-quick-check:${snapshotLabel}`
            })
            : null;
        const inspector = document.querySelector('[aria-label="Run playback inspector"]');
        return {
            label: snapshotLabel,
            diagnostics,
            quickIdle: idle,
            inspectorText: inspector?.innerText ?? "",
            timestamp: new Date().toISOString()
        };
    }, label);

    const beforePlay = await collect("before-play");
    await page.getByRole("button", { name: /^Play$/ }).click();
    await page.waitForFunction(() => {
        const host = document.querySelector('[data-testid="webgl-scene-host"]');
        const diagnostics = window.CanDoItAll?.webglScene?.getDiagnostics(host);
        return diagnostics &&
            (diagnostics.activeMotionCount > 0 ||
             diagnostics.queuedMotionCount > 0 ||
             diagnostics.queuedCommandStageCount > 0 ||
             diagnostics.isRenderLoopActive);
    }, null, { timeout: 5000 });

    const duringPlay = await collect("during-play");
    await page.getByRole("button", { name: /^Pause$/ }).click();
    const afterPauseClick = await collect("after-pause-click");
    const idleAfterPause = await page.evaluate(async () => {
        const host = document.querySelector('[data-testid="webgl-scene-host"]');
        return await window.CanDoItAll.webglScene.waitForRuntimeIdle(host, {
            timeoutMs: 2000,
            pollIntervalMs: 16,
            reason: "SB01-browser-pause-proof"
        });
    });
    const afterIdle = await collect("after-idle-wait");
    const statusAfterIdle = afterIdle.inspectorText;
    await page.waitForTimeout(1800);
    const afterStaleWindow = await collect("after-stale-callback-window");

    const payload = {
        proofId: "SB01-browser-pause-idle",
        route: page.url(),
        viewport: page.viewportSize(),
        beforePlay,
        duringPlay,
        afterPauseClick,
        idleAfterPause,
        afterIdle,
        afterStaleWindow,
        assertions: {
            sawActiveWorkDuringPlay:
                (duringPlay.diagnostics?.activeMotionCount ?? 0) > 0 ||
                (duringPlay.diagnostics?.queuedMotionCount ?? 0) > 0 ||
                (duringPlay.diagnostics?.queuedCommandStageCount ?? 0) > 0 ||
                duringPlay.diagnostics?.isRenderLoopActive === true,
            idleAfterPauseSucceeded: idleAfterPause?.success === true,
            noActiveMotionsAfterPause: (afterIdle.diagnostics?.activeMotionCount ?? -1) === 0,
            noQueuedMotionsAfterPause: (afterIdle.diagnostics?.queuedMotionCount ?? -1) === 0,
            noQueuedStagesAfterPause: (afterIdle.diagnostics?.queuedCommandStageCount ?? -1) === 0,
            notPlayingAfterPauseWindow: !afterStaleWindow.inspectorText.includes("PLAYING\nTrue"),
            idleAfterStaleCallbackWindow: afterStaleWindow.quickIdle?.success === true,
            noActiveStageAfterStaleCallbackWindow: !afterStaleWindow.quickIdle?.blockers?.some(blocker =>
                blocker.startsWith("command-stage:") || blocker.startsWith("motion:")),
            noStaleMotionCompletedStatus:
                afterStaleWindow.inspectorText === statusAfterIdle ||
                !afterStaleWindow.inspectorText.includes("Motion completed:")
        }
    };
    const failedAssertions = Object.entries(payload.assertions)
        .filter(([, value]) => value !== true)
        .map(([key]) => key);
    if (failedAssertions.length > 0) {
        throw new Error(`SB01 browser pause proof failed: ${failedAssertions.join(", ")}\n${JSON.stringify(payload, null, 2)}`);
    }

    return JSON.stringify(payload, null, 2);
}
