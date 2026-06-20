namespace CanDoItAll.Components.WebGlRunLib;

public sealed class WebGlRunVisualStateResolver
{
    public WebGlPoseDefinition? ResolvePose(WebGlRunAction action, WebGlRunPlanningContext context)
        => context.VisualStates.Poses.FirstOrDefault(item =>
            string.Equals(item.PoseKey, FirstNonEmpty(action.PoseKey, action.Parameters.GetValueOrDefault("poseKey")), StringComparison.Ordinal)) ??
           context.VisualStates.Poses.FirstOrDefault(static item => item.IsNoOpFallback) ??
           WebGlVisualStateCatalogFallbacks.NoOpPose;

    public WebGlSymbolDefinition? ResolveSymbol(WebGlRunAction action, WebGlRunPlanningContext context)
        => context.VisualStates.Symbols.FirstOrDefault(item =>
            string.Equals(item.SymbolKey, FirstNonEmpty(action.SymbolKey, action.Parameters.GetValueOrDefault("symbolKey"), action.Parameters.GetValueOrDefault("symbolKind"), "status"), StringComparison.Ordinal)) ??
           context.VisualStates.Symbols.FirstOrDefault(static item => item.IsNoOpFallback) ??
           WebGlVisualStateCatalogFallbacks.NoOpSymbol;

    private static string FirstNonEmpty(params string?[] values)
        => values.FirstOrDefault(value => !string.IsNullOrWhiteSpace(value)) ?? string.Empty;
}
