(function () {
    const root = window.CanDoItAll = window.CanDoItAll || {};
    const sandbox = root.webglSandbox = root.webglSandbox || {};

    sandbox.runPlayback = {
        reference: null,
        immediateStopHookInstalled: false,
        register(reference) {
            this.reference = reference;
            this.installImmediateStopHook();
        },
        play() {
            return this.invoke("ProofPlayAsync");
        },
        async loadDocumentJson(documentJson) {
            const text = String(documentJson || "");
            if (text.length <= 24000) {
                return this.invoke("ProofLoadDocumentJsonAsync", text);
            }

            await this.invoke("ProofBeginDocumentJsonLoadAsync");
            for (let offset = 0; offset < text.length; offset += 12000) {
                await this.invoke("ProofAppendDocumentJsonChunkAsync", text.slice(offset, offset + 12000));
            }

            return this.invoke("ProofCommitDocumentJsonLoadAsync");
        },
        pause() {
            return this.invoke("ProofPauseAsync");
        },
        snapshot() {
            return this.invoke("ProofSnapshotAsync");
        },
        invoke(method, ...args) {
            if (!this.reference) {
                throw new Error("Run playback proof bridge is not registered.");
            }

            return this.reference.invokeMethodAsync(method, ...args);
        },
        installImmediateStopHook() {
            if (this.immediateStopHookInstalled) {
                return;
            }

            this.immediateStopHookInstalled = true;
            document.addEventListener("pointerdown", event => {
                const button = event.target?.closest?.("button");
                const action = resolveImmediateStopAction(button);
                if (!button || !action) {
                    return;
                }

                const result = this.stopBrowserRuntimeImmediately(`run-playback-${action}-pointerdown`);
                this.syncRuntimeStopGeneration(result);
            }, true);
        },
        stopBrowserRuntimeImmediately(reason) {
            const host = document.querySelector("[data-testid='webgl-run-playback-stage'] [data-testid='webgl-scene-host']");
            if (!host?.__webglSceneState || !root.webglScene?.stopRuntimeActivity) {
                return null;
            }

            return root.webglScene.stopRuntimeActivity(host, reason);
        },
        syncRuntimeStopGeneration(result) {
            const generation = Number(result?.metadata?.runtimeStopGeneration || result?.diagnostics?.runtimeStopGeneration || 0);
            if (!Number.isFinite(generation) || generation <= 0 || !this.reference) {
                return;
            }

            this.invoke("ProofSyncRuntimeStopGenerationAsync", generation).catch(() => {
            });
        }
    };

    function resolveImmediateStopAction(button) {
        const text = normalizeButtonLabel(button?.getAttribute?.("aria-label") || button?.title || button?.textContent);
        if (!text) {
            return "";
        }

        return ["pause", "cancel", "reset", "step"].find(action =>
            text === action ||
            text.endsWith(action) ||
            text.includes(action)) || "";
    }

    function normalizeButtonLabel(text) {
        return String(text || "").trim().toLowerCase().replace(/\s+/g, "");
    }
})();
