using CanDoItAll.Components.WebGlLib;
using CanDoItAll.Components.WebGlRunLib;

namespace CanDoItAll.Components.WebGlRunLib.Tests;

public sealed class WebGlRunActionPlannerTests
{
    [Fact]
    public void Planner_moves_actor_to_target_object_anchor_and_back_home()
    {
        var planner = new WebGlRunActionPlanner();
        WebGlRunPlanningContext context = CreatePlanningContext();

        WebGlRunActionPlan move = planner.Plan(new()
        {
            ActionId = "move.actor.to.target",
            Kind = WebGlRunActionKinds.MoveToObject,
            ObjectId = "actor",
            Target = new() { ObjectId = "target", AnchorKey = WebGlRunAnchorKeys.Use, Offset = new WebGlVector3(0.25, 0, 0) },
            DurationSeconds = 0.5
        }, context);
        WebGlRunActionPlan back = planner.Plan(new()
        {
            ActionId = "return.actor.home",
            Kind = WebGlRunActionKinds.ReturnToAnchor,
            ObjectId = "actor",
            Target = new() { AnchorKey = WebGlRunAnchorKeys.Home },
            DurationSeconds = 0.5
        }, context);

        Assert.True(move.IsValid, string.Join(Environment.NewLine, move.Errors));
        Assert.True(back.IsValid, string.Join(Environment.NewLine, back.Errors));
        Assert.Equal(new WebGlVector3(4.25, 0, 1), move.Motions.Single().TargetPosition);
        Assert.Equal(new WebGlVector3(-3, 0, 0), back.Motions.Single().TargetPosition);
    }

    [Fact]
    public void Planner_maps_sequence_pose_symbol_and_movement_through_catalog()
    {
        var planner = new WebGlRunActionPlanner();
        WebGlRunPlanningContext context = CreatePlanningContext();

        WebGlRunActionPlan plan = planner.Plan(new()
        {
            ActionId = "sequence.work",
            Kind = WebGlRunActionKinds.Sequence,
            Steps =
            [
                new() { ActionId = "pose.work", Kind = WebGlRunActionKinds.ChangePose, ObjectId = "actor", PoseKey = "working" },
                new() { ActionId = "symbol.resource", Kind = WebGlRunActionKinds.ShowSymbol, ObjectId = "actor", SymbolKey = "resource" },
                new() { ActionId = "move.target", Kind = WebGlRunActionKinds.MoveToObject, ObjectId = "actor", Target = new() { ObjectId = "target" }, DurationSeconds = 0.25 }
            ]
        }, context);

        Assert.True(plan.IsValid, string.Join(Environment.NewLine, plan.Errors));
        Assert.Single(plan.Actions);
        Assert.Equal(3, plan.Actions[0].Steps.Count);
        Assert.Single(plan.Motions);
        Assert.Equal(2, plan.Patches.Count);
        Assert.Equal("asset.person.working", plan.Patches[0].ObjectPatches[0].AssetId);
        Assert.Equal("resource", plan.Patches[1].ObjectPatches[0].Symbols![0].SemanticKind);
    }

    [Fact]
    public void Planner_normalizes_aliases_once_and_warns_on_conflicts()
    {
        var planner = new WebGlRunActionPlanner();

        WebGlRunActionPlan plan = planner.Plan(new()
        {
            ActionId = "alias.conflict",
            Kind = WebGlRunActionKinds.Wait,
            ActionKind = WebGlRunActionKinds.MoveToObject,
            ObjectId = "legacy-object",
            SubjectObjectId = "actor",
            TargetObjectId = "legacy-target",
            Target = new() { ObjectId = "target", AnchorKey = WebGlRunAnchorKeys.Use }
        }, CreatePlanningContext());

        Assert.True(plan.IsValid, string.Join(Environment.NewLine, plan.Errors));
        Assert.Single(plan.Motions);
        Assert.Equal("actor", plan.Motions[0].ObjectId);
        Assert.Contains(plan.Warnings, warning => warning.Contains("conflicting aliases", StringComparison.Ordinal));
    }

    [Fact]
    public void Planner_adds_deterministic_distance_estimate_to_targeted_motion()
    {
        var planner = new WebGlRunActionPlanner();

        WebGlRunActionPlan plan = planner.Plan(new()
        {
            ActionId = "distance.move",
            ActionKind = WebGlRunActionKinds.MoveToObject,
            SubjectObjectId = "actor",
            Target = new() { ObjectId = "target", AnchorKey = WebGlRunAnchorKeys.Use },
            Parameters =
            {
                ["speedUnitsPerSecond"] = "2"
            }
        }, CreatePlanningContext());

        Assert.True(plan.IsValid, string.Join(Environment.NewLine, plan.Errors));
        Assert.Equal("6.083", plan.Motions[0].Metadata["distanceEstimate"]);
        Assert.Equal("3.041", plan.Motions[0].Metadata["estimatedDurationSeconds"]);
    }

    [Fact]
    public void Planner_rejects_unsupported_missing_subject_and_missing_target_actions_after_normalization()
    {
        var planner = new WebGlRunActionPlanner();

        WebGlRunActionPlan unsupported = planner.Plan(new()
        {
            ActionId = "bad.kind",
            ActionKind = "economy-specific-action"
        }, CreatePlanningContext());
        WebGlRunActionPlan missingSubject = planner.Plan(new()
        {
            ActionId = "bad.subject",
            ActionKind = WebGlRunActionKinds.MoveToObject,
            Target = new() { ObjectId = "target" }
        }, CreatePlanningContext());
        WebGlRunActionPlan missingTarget = planner.Plan(new()
        {
            ActionId = "bad.target",
            ActionKind = WebGlRunActionKinds.MoveToObject,
            SubjectObjectId = "actor"
        }, CreatePlanningContext());

        Assert.False(unsupported.IsValid);
        Assert.False(missingSubject.IsValid);
        Assert.False(missingTarget.IsValid);
        Assert.Contains(unsupported.Errors, error => error.Contains("Unsupported", StringComparison.Ordinal));
        Assert.Contains(missingSubject.Errors, error => error.Contains("subject object id", StringComparison.Ordinal));
        Assert.Contains(missingTarget.Errors, error => error.Contains("Target.ObjectId", StringComparison.Ordinal));
    }

    [Fact]
    public void Run_document_provenance_validator_keeps_generic_input_refs_domain_neutral()
    {
        var validator = new WebGlRunDocumentProvenanceValidator(["water", "well", "tax"]);
        var source = new RunSourceRef
        {
            SourceKind = "simulation-input-pack",
            SourceId = "pack.shared-resource",
            Inputs =
            {
                new() { Kind = "scenario", Path = "inputs/scenario.definition.json", ContentHash = "sha256:abc" },
                new() { Kind = "placement", Path = "inputs/placement.json", ContentHash = "sha256:def" }
            },
            Hashes =
            {
                new() { Kind = "scenario", ContentHash = "sha256:abc" }
            }
        };
        WebGlRunDocument document = new()
        {
            Metadata =
            {
                ["inputPackHash"] = "sha256:pack",
                ["sourceKind"] = "simulation-input-pack"
            }
        };
        WebGlRunDocument domainSpecific = new()
        {
            Metadata =
            {
                ["water.well.tax"] = "leaks domain vocabulary"
            }
        };

        Assert.True(validator.Validate(source).IsValid);
        Assert.True(validator.Validate(document).IsValid);
        Assert.False(validator.Validate(domainSpecific).IsValid);
    }

    [Fact]
    public void Planner_falls_back_to_object_center_when_anchor_is_missing()
    {
        var planner = new WebGlRunActionPlanner();

        WebGlRunActionPlan plan = planner.Plan(new()
        {
            ActionId = "anchor.missing",
            ActionKind = WebGlRunActionKinds.MoveToObject,
            SubjectObjectId = "actor",
            Target = new() { ObjectId = "target", AnchorKey = "missing-anchor" }
        }, CreatePlanningContext());

        Assert.True(plan.IsValid, string.Join(Environment.NewLine, plan.Errors));
        Assert.Equal(new WebGlVector3(4, 0, 0), plan.Motions[0].TargetPosition);
        Assert.Contains(plan.Warnings, warning => warning.Contains("missing-anchor", StringComparison.Ordinal));
    }

    [Fact]
    public void Planner_returns_failed_diagnostic_for_unresolved_target_without_throwing()
    {
        var planner = new WebGlRunActionPlanner();

        WebGlRunActionPlan plan = planner.Plan(new()
        {
            ActionId = "move.missing",
            Kind = WebGlRunActionKinds.MoveToObject,
            ObjectId = "actor",
            Target = new() { ObjectId = "missing" }
        }, CreatePlanningContext());

        Assert.False(plan.IsValid);
        Assert.Empty(plan.Motions);
        Assert.Contains(plan.Errors, error => error.Contains("missing", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Target_resolver_reports_resolution_metadata_for_trade_anchor_and_explicit_position()
    {
        var resolver = new WebGlRunTargetResolver();
        WebGlRunTargetResolution trade = resolver.ResolveTarget(new()
        {
            ObjectId = "target",
            AnchorKey = WebGlRunAnchorKeys.Trade
        }, CreatePlanningContext());

        Assert.True(trade.Succeeded, string.Join(Environment.NewLine, trade.Errors));
        Assert.Equal("target", trade.TargetObjectId);
        Assert.Equal(WebGlRunAnchorKeys.Trade, trade.AnchorKey);
        Assert.Equal("built-in-anchor", trade.AnchorKind);

        var explicitResolver = new WebGlRunTargetResolver();
        WebGlRunTargetResolution explicitPosition = explicitResolver.ResolveTarget(new()
        {
            Position = new WebGlVector3(9, 0, 2)
        }, CreatePlanningContext());

        Assert.True(explicitPosition.Succeeded);
        Assert.Equal(new WebGlVector3(9, 0, 2), explicitPosition.Position);
        Assert.Equal("explicit-position", explicitPosition.AnchorKind);
    }

    [Fact]
    public void Visual_state_catalog_validator_checks_duplicates_assets_and_bindings()
    {
        var catalog = new WebGlVisualStateCatalog
        {
            Poses =
            {
                new() { PoseKey = "carry", AssetId = "asset.person.carry", AssetVariantId = "variant.carry" },
                new() { PoseKey = "carry", AssetId = "asset.person.carry" },
                new() { PoseKey = "admin-writing", AssetId = "missing.asset" },
                new() { PoseKey = "noop", IsNoOpFallback = true }
            },
            Symbols =
            {
                new() { SymbolKey = "signal", SymbolAssetId = "asset.symbol.signal" },
                new() { SymbolKey = "risk", SymbolAssetId = "missing.symbol" },
                new() { SymbolKey = "noop", IsNoOpFallback = true }
            },
            ActionBindings =
            {
                new() { ActionKind = "collect", PoseKey = "carry", SymbolKey = "signal" },
                new() { ActionKind = "admin", PoseKey = "admin-writing", SymbolKey = "missing-symbol-key" }
            }
        };
        var assets = new WebGlAssetCatalog
        {
            Assets =
            {
                new()
                {
                    Id = "asset.person.carry",
                    Variants = { new() { Id = "variant.carry" } }
                },
                new() { Id = "asset.symbol.signal" }
            }
        };

        WebGlVisualStateCatalogValidationResult result = new WebGlVisualStateCatalogValidator().Validate(catalog, assets);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.Contains("Duplicate pose", StringComparison.Ordinal));
        Assert.Contains(result.Errors, error => error.Contains("missing.asset", StringComparison.Ordinal));
        Assert.Contains(result.Errors, error => error.Contains("missing.symbol", StringComparison.Ordinal));
        Assert.Contains(result.Errors, error => error.Contains("missing-symbol-key", StringComparison.Ordinal));
    }

    private static WebGlRunPlanningContext CreatePlanningContext()
        => new()
        {
            Scene = new()
            {
                SceneId = "planner-scene",
                Objects =
                [
                    new()
                    {
                        Id = "actor",
                        Position = new WebGlVector3(-2, 0, 0),
                        Anchors =
                        [
                            new() { Key = WebGlRunAnchorKeys.Home, Position = new WebGlVector3(-3, 0, 0) }
                        ]
                    },
                    new()
                    {
                        Id = "target",
                        Position = new WebGlVector3(4, 0, 0),
                        Size = new WebGlVector3(2, 2, 2),
                        Anchors =
                        [
                            new() { Key = WebGlRunAnchorKeys.Use, Position = new WebGlVector3(4, 0, 1) }
                        ]
                    }
                ]
            },
            VisualStates = new()
            {
                Poses =
                [
                    new() { PoseKey = "working", AssetId = "asset.person.working" }
                ],
                Symbols =
                [
                    new() { SymbolKey = "resource", SemanticKind = "resource", Color = "#22c55e" }
                ]
            }
        };
}
