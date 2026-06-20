export function resolveSceneRevision(scene) {
    const sceneRevision = Number(scene?.revision);
    if (Number.isFinite(sceneRevision) && sceneRevision > 0) {
        return Math.floor(sceneRevision);
    }

    const uiRevision = Number(scene?.uiState?.revision);
    return Number.isFinite(uiRevision) && uiRevision > 0 ? Math.floor(uiRevision) : 0;
}

export function commitSceneRevision(scene, revision) {
    const nextRevision = Math.max(0, Math.floor(Number(revision) || 0));
    scene.revision = nextRevision;
    scene.uiState = scene.uiState || {};
    scene.uiState.revision = nextRevision;
    return nextRevision;
}
