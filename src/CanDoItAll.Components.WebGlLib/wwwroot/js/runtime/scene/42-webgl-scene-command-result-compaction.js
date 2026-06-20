const defaultMaxBatchChildResults = 5;
const defaultMaxBatchMessages = 5;
const defaultMaxBatchProofSnapshotPositions = 10;

export function compactBatchResultForInterop(state, result) {
    const totalCommandResultCount = result.commandResults.length;
    const totalWarningCount = result.warnings.length;
    const totalErrorCount = result.errors.length;
    const childLimit = normalizeLimit(state?.options?.maxCommandBatchChildResults, defaultMaxBatchChildResults);
    const messageLimit = normalizeLimit(state?.options?.maxCommandBatchMessages, defaultMaxBatchMessages);
    compactBatchProofSnapshotForInterop(state, result);

    result.commandResults = result.commandResults.slice(0, childLimit).map(compactChildCommandResult);
    result.warnings = limitWithOverflow(result.warnings, messageLimit, totalWarningCount, "warning");
    result.errors = limitWithOverflow(result.errors, messageLimit, totalErrorCount, "error");
    result.metadata = {
        ...result.metadata,
        totalCommandResultCount: String(totalCommandResultCount),
        returnedCommandResultCount: String(result.commandResults.length),
        totalWarningCount: String(totalWarningCount),
        returnedWarningCount: String(Math.min(totalWarningCount, messageLimit)),
        totalErrorCount: String(totalErrorCount),
        returnedErrorCount: String(Math.min(totalErrorCount, messageLimit))
    };
}

export function limitWithOverflow(values, limit, totalCount, label) {
    const items = Array.isArray(values) ? values : [];
    if (items.length <= limit) {
        return items;
    }

    return [
        ...items.slice(0, limit),
        `${Math.max(0, totalCount - limit)} additional ${label} item(s) omitted from the interop result.`
    ];
}

function compactBatchProofSnapshotForInterop(state, result) {
    const snapshot = result?.proofSnapshot;
    if (!snapshot?.objectPositions) {
        return;
    }

    const entries = Object.entries(snapshot.objectPositions);
    const totalPositionCount = entries.length;
    const limit = normalizeLimit(
        state?.options?.maxCommandBatchProofSnapshotPositions,
        defaultMaxBatchProofSnapshotPositions);
    if (totalPositionCount <= limit) {
        result.metadata = {
            ...result.metadata,
            proofSnapshotPositionCount: String(totalPositionCount),
            returnedProofSnapshotPositionCount: String(totalPositionCount)
        };
        return;
    }

    snapshot.objectPositions = Object.fromEntries(entries.slice(0, limit));
    snapshot.metadata = {
        ...(snapshot.metadata || {}),
        compactedForInterop: "true",
        totalObjectPositionCount: String(totalPositionCount),
        returnedObjectPositionCount: String(limit),
        omittedObjectPositionCount: String(totalPositionCount - limit)
    };
    result.metadata = {
        ...result.metadata,
        proofSnapshotCompactedForInterop: "true",
        proofSnapshotPositionCount: String(totalPositionCount),
        returnedProofSnapshotPositionCount: String(limit),
        omittedProofSnapshotPositionCount: String(totalPositionCount - limit)
    };
}

function compactChildCommandResult(result) {
    return {
        commandId: result?.commandId || "",
        success: result?.success !== false,
        succeeded: result?.success !== false,
        lifecycleState: result?.lifecycleState || "settled",
        settled: result?.settled === true,
        sceneId: result?.sceneId || "",
        commandKind: result?.commandKind || "",
        revision: Number(result?.revision) || 0,
        errors: limitWithOverflow(result?.errors || [], defaultMaxBatchMessages, result?.errors?.length || 0, "error"),
        warnings: limitWithOverflow(result?.warnings || [], defaultMaxBatchMessages, result?.warnings?.length || 0, "warning"),
        affectedObjectIds: limitWithOverflow(result?.affectedObjectIds || [], defaultMaxBatchMessages, result?.affectedObjectIds?.length || 0, "affected object"),
        affectedLinkIds: limitWithOverflow(result?.affectedLinkIds || [], defaultMaxBatchMessages, result?.affectedLinkIds?.length || 0, "affected link"),
        diagnostics: result?.diagnostics || {},
        metadata: {
            ...(result?.metadata || {}),
            totalAffectedObjectCount: String(result?.affectedObjectIds?.length || 0),
            totalAffectedLinkCount: String(result?.affectedLinkIds?.length || 0)
        }
    };
}

export function normalizeLimit(value, fallback) {
    const limit = Number(value);
    return Number.isFinite(limit) && limit > 0 ? Math.min(Math.floor(limit), 1000) : fallback;
}
