using CanDoItAll.Components.WebGlLib;
using CanDoItAll.Components.WebGlRunLib;

namespace CanDoItAll.Components.WebGlRunLib.Tests;

public sealed class WebGlRunValidatorTests
{
    [Fact]
    public void Run_document_validator_rejects_schema_timeline_and_domain_metadata()
    {
        var document = new WebGlRunDocument
        {
            SchemaVersion = "webgl-run-document/legacy",
            RunId = new WebGlRunId(string.Empty),
            InitialScene = new WebGlSceneDocument
            {
                Scene = new WebGlSceneModel { SceneId = "scene.validator" }
            },
            Timeline =
            {
                FrameRate = 0,
                Frames =
                [
                    new WebGlRunFrame { Index = 1, TimeSeconds = 1 },
                    new WebGlRunFrame { Index = 1, TimeSeconds = 2 }
                ]
            },
            Metadata =
            {
                ["economy.ledger"] = "domain leak"
            }
        };

        WebGlRunDocumentValidationResult validation = new WebGlRunDocumentValidator().Validate(document);

        Assert.False(validation.IsValid);
        Assert.Contains(validation.Errors, error => error.Contains("schema", StringComparison.OrdinalIgnoreCase));
        Assert.Contains(validation.Errors, error => error.Contains("Run id", StringComparison.OrdinalIgnoreCase));
        Assert.Contains(validation.Errors, error => error.Contains("Timeline frame rate", StringComparison.Ordinal));
        Assert.Contains(validation.Errors, error => error.Contains("Duplicate frame index", StringComparison.Ordinal));
        Assert.Contains(validation.Errors, error => error.Contains("domain-specific", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Action_plan_validator_rejects_domain_specific_and_structural_errors()
    {
        var plan = new WebGlRunActionPlan
        {
            FrameRate = 0,
            ActionId = "plan.domain",
            Metadata =
            {
                ["ledger"] = "market-clearing"
            },
            Actions =
            [
                new()
                {
                    ActionId = "action.domain",
                    ActionKind = "economy-market-transfer"
                },
                new()
                {
                    ActionId = "action.missing.subject",
                    ActionKind = WebGlRunActionKinds.MoveToObject,
                    TargetObjectId = "target"
                }
            ]
        };

        WebGlRunActionPlanValidationResult validation = new WebGlRunActionPlanValidator().Validate(plan);

        Assert.False(validation.IsValid);
        Assert.Contains(validation.Errors, error => error.Contains("Frame rate", StringComparison.OrdinalIgnoreCase));
        Assert.Contains(validation.Errors, error => error.Contains("Unsupported WebGL run action kind", StringComparison.Ordinal));
        Assert.Contains(validation.Errors, error => error.Contains("subject object id", StringComparison.Ordinal));
        Assert.Contains(validation.Errors, error => error.Contains("domain-specific", StringComparison.OrdinalIgnoreCase));
    }
}
