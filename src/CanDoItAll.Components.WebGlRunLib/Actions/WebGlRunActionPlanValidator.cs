using CanDoItAll.Components.WebGlLib;

namespace CanDoItAll.Components.WebGlRunLib;

public sealed class WebGlRunActionPlanValidationResult
{
    public List<string> Errors { get; } = [];

    public List<string> Warnings { get; } = [];

    public bool IsValid => Errors.Count == 0;
}

public sealed class WebGlRunActionPlanValidator
{
    private readonly WebGlRunActionNormalizer actionNormalizer = new();
    private readonly WebGlRunGenericBoundaryOptions boundaryOptions;

    public WebGlRunActionPlanValidator()
        : this(WebGlRunGenericBoundaryOptions.None)
    {
    }

    public WebGlRunActionPlanValidator(WebGlRunGenericBoundaryOptions? boundaryOptions)
    {
        this.boundaryOptions = boundaryOptions ?? WebGlRunGenericBoundaryOptions.None;
    }

    public WebGlRunActionPlanValidationResult Validate(WebGlRunActionPlan plan)
    {
        ArgumentNullException.ThrowIfNull(plan);
        var result = new WebGlRunActionPlanValidationResult();
        if (plan.FrameRate <= 0)
        {
            result.Errors.Add("Frame rate must be positive.");
        }

        result.Errors.AddRange(plan.Errors);
        result.Warnings.AddRange(plan.Warnings);
        WebGlRunDocumentValidator.ValidateDomainTerms("plan.metadata", plan.Metadata, result.Errors, boundaryOptions: boundaryOptions);
        ValidateActions(plan.Actions, result);
        ValidateDirectCommands(plan, result);
        return result;
    }

    private void ValidateActions(IEnumerable<WebGlRunAction> actions, WebGlRunActionPlanValidationResult result)
    {
        var actionIds = new HashSet<string>(StringComparer.Ordinal);
        foreach (WebGlRunAction action in actions)
        {
            WebGlRunActionNormalizationResult normalization = actionNormalizer.Normalize(action);
            result.Errors.AddRange(normalization.Errors);
            result.Warnings.AddRange(normalization.Warnings);
            ValidateActionTree(normalization.Action, actionIds, result, boundaryOptions);
        }
    }

    private static void ValidateActionTree(
        WebGlRunAction action,
        HashSet<string> actionIds,
        WebGlRunActionPlanValidationResult result,
        WebGlRunGenericBoundaryOptions boundaryOptions)
    {
        if (string.IsNullOrWhiteSpace(action.ActionId))
        {
            result.Errors.Add("Action id is required.");
        }
        else if (!actionIds.Add(action.ActionId))
        {
            result.Errors.Add($"Duplicate action id '{action.ActionId}'.");
        }

        WebGlRunDocumentValidator.ValidateDomainValue($"action:{action.ActionId}.kind", action.ActionKind, result.Errors, boundaryOptions);
        WebGlRunDocumentValidator.ValidateDomainValue($"action:{action.ActionId}.id", action.ActionId, result.Errors, boundaryOptions);
        WebGlRunDocumentValidator.ValidateDomainTerms($"action:{action.ActionId}.metadata", action.Metadata, result.Errors, boundaryOptions: boundaryOptions);
        WebGlRunDocumentValidator.ValidateDomainTerms(
            $"action:{action.ActionId}.parameters",
            action.Parameters,
            result.Errors,
            allowSourceProvenance: false,
            boundaryOptions: boundaryOptions);
        if (!double.IsFinite(action.StartsAtSeconds) || action.StartsAtSeconds < 0)
        {
            result.Errors.Add($"Action '{action.ActionId}' start time must be finite and non-negative.");
        }

        if (!double.IsFinite(action.DurationSeconds) || action.DurationSeconds < 0)
        {
            result.Errors.Add($"Action '{action.ActionId}' duration must be finite and non-negative.");
        }

        if (!IsKnownCoalescingScope(action.CoalescingScope))
        {
            result.Errors.Add($"Action '{action.ActionId}' uses unsupported coalescing scope '{action.CoalescingScope}'.");
        }

        if (!IsKnownExecutionPolicy(action.ExecutionPolicy))
        {
            result.Errors.Add($"Action '{action.ActionId}' uses unsupported execution policy '{action.ExecutionPolicy}'.");
        }

        foreach (WebGlRunAction child in action.Steps)
        {
            ValidateActionTree(child, actionIds, result, boundaryOptions);
        }
    }

    private void ValidateDirectCommands(WebGlRunActionPlan plan, WebGlRunActionPlanValidationResult result)
    {
        foreach (WebGlScenePatch patch in plan.Patches)
        {
            WebGlRunDocumentValidator.ValidateDomainTerms("plan.patch.metadata", patch.Metadata, result.Errors, boundaryOptions: boundaryOptions);
        }

        foreach (WebGlObjectMotionCommand motion in plan.Motions)
        {
            if (string.IsNullOrWhiteSpace(motion.ObjectId))
            {
                result.Errors.Add("Direct plan motion requires an object id.");
            }

            WebGlRunDocumentValidator.ValidateDomainTerms($"plan.motion:{motion.MotionId}.metadata", motion.Metadata, result.Errors, boundaryOptions: boundaryOptions);
        }
    }

    private static bool IsKnownCoalescingScope(string scope)
        => scope is WebGlRunCoalescingScopes.None
            or WebGlRunCoalescingScopes.StageOnly
            or WebGlRunCoalescingScopes.Frame;

    private static bool IsKnownExecutionPolicy(string policy)
        => policy is WebGlRunStageExecutionPolicies.PreserveOrder
            or WebGlRunStageExecutionPolicies.Parallel
            or WebGlRunStageExecutionPolicies.CoalesceWithinStage;
}
