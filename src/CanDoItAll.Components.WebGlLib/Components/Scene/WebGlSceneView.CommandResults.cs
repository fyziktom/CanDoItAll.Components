using System.Globalization;

namespace CanDoItAll.Components.WebGlLib;

public partial class WebGlSceneView
{
    private static void AnnotateRuntimeIdleResult(
        WebGlSceneCommandResult result,
        WebGlRuntimeIdleResult idleResult,
        string reason,
        bool requireIdle)
    {
        result.Metadata["runtimeIdle"] = idleResult.Idle.ToString(CultureInfo.InvariantCulture);
        result.Metadata["runtimeIdlePolicyMode"] = idleResult.PolicyMode;
        result.Metadata["runtimeIdleTimedOut"] = idleResult.TimedOut.ToString(CultureInfo.InvariantCulture);
        result.Metadata["runtimeIdleElapsedMs"] = idleResult.ElapsedMs.ToString(CultureInfo.InvariantCulture);
        result.Diagnostics["runtimeIdle"] = idleResult.Idle.ToString(CultureInfo.InvariantCulture);
        result.Diagnostics["runtimeIdlePolicyMode"] = idleResult.PolicyMode;
        result.Diagnostics["runtimeIdleTimedOut"] = idleResult.TimedOut.ToString(CultureInfo.InvariantCulture);
        result.Diagnostics["runtimeIdleElapsedMs"] = idleResult.ElapsedMs.ToString(CultureInfo.InvariantCulture);

        if (idleResult.Blockers.Count > 0)
        {
            result.Metadata["runtimeIdleBlockers"] = string.Join(",", idleResult.Blockers);
            result.Diagnostics["runtimeIdleBlockers"] = string.Join(",", idleResult.Blockers);
            result.Warnings.Add($"Runtime did not settle before timeout. Blockers: {string.Join(", ", idleResult.Blockers)}.");
        }

        if (idleResult.Diagnostics is not null)
        {
            result.Diagnostics["activeMotionCount"] = idleResult.Diagnostics.ActiveMotionCount.ToString(CultureInfo.InvariantCulture);
            result.Diagnostics["queuedMotionCount"] = idleResult.Diagnostics.QueuedMotionCount.ToString(CultureInfo.InvariantCulture);
            result.Diagnostics["queuedCommandStageCount"] = idleResult.Diagnostics.QueuedCommandStageCount.ToString(CultureInfo.InvariantCulture);
            result.Diagnostics["currentCommandBatchId"] = idleResult.Diagnostics.CurrentCommandBatchId;
            result.Diagnostics["currentCommandStageId"] = idleResult.Diagnostics.CurrentCommandStageId;
            result.Diagnostics["commandStageBarrierPolicy"] = idleResult.Diagnostics.CommandStageBarrierPolicy;
            result.Diagnostics["commandStageBarrierBlockers"] = string.Join(",", idleResult.Diagnostics.CommandStageBarrierBlockers);
            result.Diagnostics["runtimeStopGeneration"] = idleResult.Diagnostics.RuntimeStopGeneration.ToString(CultureInfo.InvariantCulture);
            result.Diagnostics["lastRuntimeStopIdle"] = idleResult.Diagnostics.LastRuntimeStopIdle.ToString(CultureInfo.InvariantCulture);
            result.Diagnostics["lastRuntimeStopTimedOut"] = idleResult.Diagnostics.LastRuntimeStopTimedOut.ToString(CultureInfo.InvariantCulture);
            result.Diagnostics["lastRuntimeStopIdleElapsedMs"] = idleResult.Diagnostics.LastRuntimeStopIdleElapsedMs.ToString(CultureInfo.InvariantCulture);
        }

        if (requireIdle && idleResult.Idle != true)
        {
            result.Success = false;
            result.Succeeded = false;
            result.Settled = false;
            result.LifecycleState = WebGlSceneCommandLifecycleStates.Failed;
            result.Errors.Add($"Runtime idle proof failed for '{reason}'. Blockers: {string.Join(", ", idleResult.Blockers)}.");
            result.Metadata["runtimeIdleRequired"] = true.ToString(CultureInfo.InvariantCulture);
            result.Diagnostics["runtimeIdleRequired"] = true.ToString(CultureInfo.InvariantCulture);
        }
    }
}
