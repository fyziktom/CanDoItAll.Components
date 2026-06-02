const patchTransactionModes = Object.freeze({
    strict: "strict",
    permissiveInvalidLinks: "permissive-invalid-links"
});

export function resolvePatchTransactionPolicy(patch) {
    const patchTransactionMode = resolvePatchTransactionMode(patch);
    return {
        patchTransactionMode,
        missingLinkEndpointMode: resolveMissingLinkEndpointMode(patch, patchTransactionMode)
    };
}

export function recordPatchResultMetadata(result, normalized, classification) {
    result.metadata.patchTransactionMode = normalized.patchTransactionMode;
    result.metadata.missingLinkEndpointMode = normalized.missingLinkEndpointMode;
    result.metadata.patchClassification = classification.kind;
}

export function recordSkippedLinkId(result, linkId) {
    if (!linkId) {
        return;
    }

    const values = String(result.metadata.skippedLinkIds || "")
        .split(",")
        .map(value => value.trim())
        .filter(Boolean);
    if (!values.includes(linkId)) {
        values.push(linkId);
    }

    result.metadata.skippedLinkIds = values.join(",");
}

function resolvePatchTransactionMode(patch) {
    const explicitMode = String(patch?.metadata?.patchTransactionMode || "").toLowerCase();
    const missingLinkEndpointMode = String(patch?.metadata?.missingLinkEndpointMode || "").toLowerCase();
    if (explicitMode === patchTransactionModes.permissiveInvalidLinks ||
        explicitMode === "warn-invalid-links" ||
        explicitMode === "warning-invalid-links" ||
        missingLinkEndpointMode === "warn") {
        return patchTransactionModes.permissiveInvalidLinks;
    }

    return patchTransactionModes.strict;
}

function resolveMissingLinkEndpointMode(patch, patchTransactionMode) {
    if (patchTransactionMode === patchTransactionModes.permissiveInvalidLinks) {
        return "warn";
    }

    return String(patch?.metadata?.missingLinkEndpointMode || "").toLowerCase() === "warn"
        ? "warn"
        : "fail";
}
