async page => {
    await page.waitForTimeout(1600);
    return await page.evaluate(async () => {
        const host = document.querySelector('[data-testid="webgl-scene-host"]');
        const api = window.CanDoItAll.webglScene;
        const diagnostics = api.getDiagnostics(host);
        const idle = await api.waitForRuntimeIdle(host, {
            timeoutMs: 2000,
            pollIntervalMs: 16,
            reason: "post-failure-long-window"
        });
        const inspector = document.querySelector('[aria-label="Run playback inspector"]')?.innerText ?? "";
        return JSON.stringify({ diagnostics, idle, inspector }, null, 2);
    });
}
