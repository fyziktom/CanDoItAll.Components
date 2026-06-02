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
    public void Run_document_validator_rejects_mixed_direct_and_staged_frame_commands()
    {
        var document = new WebGlRunDocument
        {
            RunId = new WebGlRunId("run.mixed"),
            InitialScene = SceneDocument(),
            Timeline =
            {
                FrameRate = 1,
                Frames =
                {
                    MixedFrame()
                }
            }
        };

        WebGlRunDocumentValidationResult validation = new WebGlRunDocumentValidator().Validate(document);

        Assert.False(validation.IsValid);
        Assert.Contains(validation.Errors, error => error.Contains("cannot mix frame-level commands with staged commands", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Run_document_validator_allows_direct_only_and_staged_only_frame_commands()
    {
        var document = new WebGlRunDocument
        {
            RunId = new WebGlRunId("run.allowed"),
            InitialScene = SceneDocument(),
            Timeline =
            {
                FrameRate = 1,
                Frames =
                {
                    new()
                    {
                        Index = 0,
                        TimeSeconds = 0,
                        Motions =
                        {
                            Motion("direct.motion", new WebGlVector3(1, 0, 0))
                        }
                    },
                    new()
                    {
                        Index = 1,
                        TimeSeconds = 1,
                        Stages =
                        {
                            new()
                            {
                                StageId = "stage.motion",
                                StageIndex = 0,
                                Motions =
                                {
                                    Motion("stage.motion", new WebGlVector3(2, 0, 0))
                                }
                            }
                        }
                    }
                }
            }
        };

        WebGlRunDocumentValidationResult validation = new WebGlRunDocumentValidator().Validate(document);

        Assert.True(validation.IsValid, string.Join(Environment.NewLine, validation.Errors));
    }

    [Fact]
    public void Run_document_validator_allows_source_provenance_metadata()
    {
        var document = new WebGlRunDocument
        {
            RunId = new WebGlRunId("run.source-provenance"),
            InitialScene = SceneDocument(),
            Metadata =
            {
                ["source.domain"] = "economy-market-run",
                ["source.ledgerId"] = "ledger-alpha"
            },
            Timeline =
            {
                FrameRate = 1,
                Frames =
                {
                    new()
                    {
                        Index = 0,
                        TimeSeconds = 0,
                        Metadata =
                        {
                            ["source.frameKind"] = "market-tick"
                        },
                        Stages =
                        {
                            new()
                            {
                                StageId = "stage.visual",
                                StageIndex = 0,
                                Metadata =
                                {
                                    ["source.eventKind"] = "market-clearing",
                                    ["source.ledgerId"] = "ledger-alpha"
                                }
                            }
                        }
                    }
                }
            }
        };

        WebGlRunDocumentValidationResult validation = new WebGlRunDocumentValidator().Validate(document);

        Assert.True(validation.IsValid, string.Join(Environment.NewLine, validation.Errors));
    }

    [Fact]
    public void Run_document_validator_rejects_domain_specific_stage_id()
    {
        var document = new WebGlRunDocument
        {
            RunId = new WebGlRunId("run.stage-domain"),
            InitialScene = SceneDocument(),
            Timeline =
            {
                FrameRate = 1,
                Frames =
                {
                    new()
                    {
                        Index = 0,
                        TimeSeconds = 0,
                        Stages =
                        {
                            new()
                            {
                                StageId = "stage.market.transfer",
                                StageIndex = 0
                            }
                        }
                    }
                }
            }
        };

        WebGlRunDocumentValidationResult validation = new WebGlRunDocumentValidator().Validate(document);

        Assert.False(validation.IsValid);
        Assert.Contains(validation.Errors, error => error.Contains("stage.market.transfer", StringComparison.Ordinal));
        Assert.Contains(validation.Errors, error => error.Contains("domain-specific", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Frame_apply_result_reports_mixed_commands_instead_of_silently_dropping_direct_commands()
    {
        WebGlRunFrameApplyResult result = WebGlRunFrameApplyResult.FromFrame(MixedFrame());

        Assert.Contains(result.Errors, error => error.Contains("cannot mix frame-level commands with staged commands", StringComparison.OrdinalIgnoreCase));
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

    [Fact]
    public void Action_plan_validator_allows_source_provenance_metadata()
    {
        var plan = new WebGlRunActionPlan
        {
            FrameRate = 1,
            ActionId = "plan.source-provenance",
            Metadata =
            {
                ["source.domain"] = "economy-market-run",
                ["source.ledgerId"] = "ledger-alpha"
            },
            Actions =
            [
                new()
                {
                    ActionId = "action.wait",
                    ActionKind = WebGlRunActionKinds.Wait,
                    Metadata =
                    {
                        ["source.eventKind"] = "market-clearing"
                    }
                }
            ],
            Patches =
            {
                new()
                {
                    Metadata =
                    {
                        ["source.patchKind"] = "ledger-adjustment"
                    }
                }
            },
            Motions =
            {
                new()
                {
                    MotionId = "motion.proof",
                    ObjectId = "actor",
                    Metadata =
                    {
                        ["source.eventKind"] = "market-clearing"
                    }
                }
            }
        };

        WebGlRunActionPlanValidationResult validation = new WebGlRunActionPlanValidator().Validate(plan);

        Assert.True(validation.IsValid, string.Join(Environment.NewLine, validation.Errors));
    }

    [Fact]
    public void Action_plan_validator_rejects_domain_specific_source_parameters()
    {
        var plan = new WebGlRunActionPlan
        {
            FrameRate = 1,
            ActionId = "plan.source-parameters",
            Actions =
            [
                new()
                {
                    ActionId = "action.wait",
                    ActionKind = WebGlRunActionKinds.Wait,
                    Parameters =
                    {
                        ["source.eventKind"] = "market-clearing"
                    }
                }
            ]
        };

        WebGlRunActionPlanValidationResult validation = new WebGlRunActionPlanValidator().Validate(plan);

        Assert.False(validation.IsValid);
        Assert.Contains(validation.Errors, error => error.Contains("domain-specific", StringComparison.OrdinalIgnoreCase));
    }

    private static WebGlSceneDocument SceneDocument()
        => new()
        {
            Scene = new WebGlSceneModel
            {
                SceneId = "scene.mixed",
                Objects =
                {
                    new()
                    {
                        Id = "actor",
                        Kind = "actor",
                        AssetId = "asset.actor"
                    }
                }
            }
        };

    private static WebGlRunFrame MixedFrame()
        => new()
        {
            Index = 0,
            TimeSeconds = 0,
            Motions =
            {
                Motion("direct.motion", new WebGlVector3(1, 0, 0))
            },
            ScenePatches =
            {
                Patch("direct.patch", "#22c55e")
            },
            Stages =
            {
                new()
                {
                    StageId = "stage.motion",
                    StageIndex = 0,
                    Motions =
                    {
                        Motion("stage.motion", new WebGlVector3(2, 0, 0))
                    }
                }
            }
        };

    private static WebGlObjectMotionCommand Motion(string id, WebGlVector3 targetPosition)
        => new()
        {
            MotionId = id,
            ObjectId = "actor",
            TargetPosition = targetPosition
        };

    private static WebGlRunFramePatch Patch(string id, string color)
        => new()
        {
            Id = id,
            Patch = new WebGlScenePatch
            {
                SceneId = "scene.mixed",
                ObjectPatches =
                {
                    new()
                    {
                        ObjectId = "actor",
                        Color = color
                    }
                }
            }
        };
}
