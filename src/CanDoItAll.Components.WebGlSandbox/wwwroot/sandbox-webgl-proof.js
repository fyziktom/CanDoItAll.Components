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
                const label = normalizeButtonLabel(button?.textContent);
                if (!button || !["pause", "cancel", "reset", "step"].includes(label)) {
                    return;
                }

                this.stopBrowserRuntimeImmediately(`run-playback-${label}-pointerdown`);
            }, true);
        },
        stopBrowserRuntimeImmediately(reason) {
            const host = document.querySelector("[data-testid='webgl-run-playback-stage'] [data-testid='webgl-scene-host']");
            if (!host?.__webglSceneState || !root.webglScene?.stopRuntimeActivity) {
                return null;
            }

            return root.webglScene.stopRuntimeActivity(host, reason);
        }
    };

    function normalizeButtonLabel(text) {
        return String(text || "").trim().toLowerCase();
    }
})();
