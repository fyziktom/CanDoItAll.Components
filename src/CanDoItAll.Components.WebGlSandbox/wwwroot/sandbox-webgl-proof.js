(function () {
    const root = window.CanDoItAll = window.CanDoItAll || {};
    const sandbox = root.webglSandbox = root.webglSandbox || {};

    sandbox.runPlayback = {
        reference: null,
        register(reference) {
            this.reference = reference;
        },
        play() {
            return this.invoke("ProofPlayAsync");
        },
        pause() {
            return this.invoke("ProofPauseAsync");
        },
        snapshot() {
            return this.invoke("ProofSnapshotAsync");
        },
        invoke(method) {
            if (!this.reference) {
                throw new Error("Run playback proof bridge is not registered.");
            }

            return this.reference.invokeMethodAsync(method);
        }
    };
})();
