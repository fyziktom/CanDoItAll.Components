using System.Text.Json;
using CanDoItAll.Components.WebGlLib;
using CanDoItAll.Components.WebGlRunLib;

namespace CanDoItAll.Components.WebGlRunLib.Tests;

public sealed class WebGlRunValidatorTests
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private sealed class DomainLeakageTermConfig
    {
        public string[] ForbiddenDomainTerms { get; set; } = [];
    }

    private static WebGlRunGenericBoundaryOptions EconomyBoundaryOptions()
    {
        string path = Path.Combine(AppContext.BaseDirectory, "fixtures", "domain-leakage-terms.json");
        DomainLeakageTermConfig config = JsonSerializer.Deserialize<DomainLeakageTermConfig>(File.ReadAllText(path), JsonOptions) ?? new();
        return new WebGlRunGenericBoundaryOptions
        {
            ForbiddenDomainTerms = config.ForbiddenDomainTerms
        };
    }

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

        WebGlRunDocumentValidationResult validation = new WebGlRunDocumentValidator(EconomyBoundaryOptions()).Validate(document);

        Assert.False(validation.IsValid);
        Assert.Contains(validation.Errors, error => error.Contains("schema", StringComparison.OrdinalIgnoreCase));
        Assert.Contains(validation.Errors, error => error.Contains("Run id", StringComparison.OrdinalIgnoreCase));
        Assert.Contains(validation.Errors, error => error.Contains("Timeline frame rate", StringComparison.Ordinal));
        Assert.Contains(validation.Errors, error => error.Contains("Duplicate frame index", StringComparison.Ordinal));
        Assert.Contains(validation.Errors, error => error.Contains("domain-specific", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Run_document_validator_default_boundary_policy_does_not_embed_economy_terms()
    {
        var document = new WebGlRunDocument
        {
            RunId = new WebGlRunId("run.default-boundary"),
            InitialScene = SceneDocument(),
            Timeline = { FrameRate = 1 },
            Metadata =
            {
                ["economy.ledger"] = "market-clearing"
            }
        };

        WebGlRunDocumentValidationResult validation = new WebGlRunDocumentValidator().Validate(document);

        Assert.True(validation.IsValid, string.Join(Environment.NewLine, validation.Errors));
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

        WebGlRunDocumentValidationResult validation = new WebGlRunDocumentValidator(EconomyBoundaryOptions()).Validate(document);

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

        WebGlRunDocumentValidationResult validation = new WebGlRunDocumentValidator(EconomyBoundaryOptions()).Validate(document);

        Assert.True(validation.IsValid, string.Join(Environment.NewLine, validation.Errors));
    }

    [Fact]
    public void Run_document_validator_allows_typed_source_provenance_with_opaque_values()
    {
        var document = new WebGlRunDocument
        {
            RunId = new WebGlRunId("run.source-provenance"),
            InitialScene = SceneDocument(),
            Metadata =
            {
                ["source.kind"] = "run",
                ["source.traceId"] = "trace.aaaaaaaaaaaaaaaa",
                ["source.inputPackHash"] = StrictHash('1'),
                ["source.traceMapHash"] = StrictHash('2')
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
                            ["source.simulationFrameId"] = "frame.bbbbbbbbbbbbbbbb",
                            ["source.sequence"] = "0"
                        },
                        Stages =
                        {
                            new()
                            {
                                StageId = "stage.visual",
                                StageIndex = 0,
                                Metadata =
                                {
                                    ["source.visualActionId"] = "visual-action.cccccccccccccccc",
                                    ["source.eventId"] = "event.dddddddddddddddd",
                                    ["source.inputPackHash"] = StrictHash('1'),
                                    ["source.parentId"] = "frame.bbbbbbbbbbbbbbbb"
                                }
                            }
                        }
                    }
                }
            }
        };

        WebGlRunDocumentValidationResult validation = new WebGlRunDocumentValidator(EconomyBoundaryOptions()).Validate(document);

        Assert.True(validation.IsValid, string.Join(Environment.NewLine, validation.Errors));
    }

    [Fact]
    public void Run_document_validator_rejects_malformed_source_provenance_keys_and_oversized_values()
    {
        var document = new WebGlRunDocument
        {
            RunId = new WebGlRunId("run.source-provenance-invalid"),
            InitialScene = SceneDocument(),
            Metadata =
            {
                ["source.sourceId"] = "legacy-id",
                ["source.hash"] = "sha256:legacy",
                [$"source.{new string('x', 100)}"] = "oversized-key",
                ["source.traceId"] = new string('x', 513)
            },
            Timeline = { FrameRate = 1 }
        };

        WebGlRunDocumentValidationResult validation = new WebGlRunDocumentValidator(EconomyBoundaryOptions()).Validate(document);

        Assert.False(validation.IsValid);
        Assert.Contains(validation.Errors, error => error.Contains("source.sourceId", StringComparison.Ordinal));
        Assert.Contains(validation.Errors, error => error.Contains("source.hash", StringComparison.Ordinal));
        Assert.Contains(validation.Errors, error => error.Contains("96 characters", StringComparison.Ordinal));
        Assert.Contains(validation.Errors, error => error.Contains("512 characters", StringComparison.Ordinal));
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

        WebGlRunDocumentValidationResult validation = new WebGlRunDocumentValidator(EconomyBoundaryOptions()).Validate(document);

        Assert.False(validation.IsValid);
        Assert.Contains(validation.Errors, error => error.Contains("stage.market.transfer", StringComparison.Ordinal));
        Assert.Contains(validation.Errors, error => error.Contains("domain-specific", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Frame_apply_result_reports_mixed_commands_instead_of_silently_dropping_direct_commands()
    {
        WebGlRunFrameApplyResult result = WebGlRunFrameApplyResult.FromFrame(MixedFrame());

        Assert.Contains(result.Errors, error => error.Contains("cannot mix frame-level commands with staged commands", StringComparison.OrdinalIgnoreCase));
        Assert.Empty(result.CommandBatch.Stages);
        Assert.Empty(result.CommandBatch.Patches);
        Assert.Empty(result.CommandBatch.Motions);
        Assert.Equal("mixed-direct-and-staged-commands", result.CommandBatch.Metadata["blockedByPolicy"]);
    }

    [Fact]
    public void Run_document_validator_rejects_executable_source_policy_metadata()
    {
        var document = new WebGlRunDocument
        {
            RunId = new WebGlRunId("run.source-policy"),
            InitialScene = SceneDocument(),
            Metadata =
            {
                ["source.batchingPolicy"] = WebGlSceneBatchingPolicies.Parallel
            },
            Timeline = { FrameRate = 1 }
        };

        WebGlRunDocumentValidationResult validation = new WebGlRunDocumentValidator().Validate(document);

        Assert.False(validation.IsValid);
        Assert.Contains(validation.Errors, error => error.Contains("traceability only", StringComparison.OrdinalIgnoreCase));
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

        WebGlRunActionPlanValidationResult validation = new WebGlRunActionPlanValidator(EconomyBoundaryOptions()).Validate(plan);

        Assert.False(validation.IsValid);
        Assert.Contains(validation.Errors, error => error.Contains("Frame rate", StringComparison.OrdinalIgnoreCase));
        Assert.Contains(validation.Errors, error => error.Contains("Unsupported WebGL run action kind", StringComparison.Ordinal));
        Assert.Contains(validation.Errors, error => error.Contains("subject object id", StringComparison.Ordinal));
        Assert.Contains(validation.Errors, error => error.Contains("domain-specific", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Action_plan_validator_allows_typed_source_provenance_with_opaque_values()
    {
        var plan = new WebGlRunActionPlan
        {
            FrameRate = 1,
            ActionId = "plan.source-provenance",
            Metadata =
            {
                ["source.kind"] = "run",
                ["source.traceId"] = "trace.aaaaaaaaaaaaaaaa",
                ["source.inputPackHash"] = StrictHash('1')
            },
            Actions =
            [
                new()
                {
                    ActionId = "action.wait",
                    ActionKind = WebGlRunActionKinds.Wait,
                    Metadata =
                    {
                        ["source.visualActionId"] = "visual-action.cccccccccccccccc",
                        ["source.eventId"] = "event.dddddddddddddddd",
                        ["source.simulationFrameId"] = "frame.bbbbbbbbbbbbbbbb",
                        ["source.inputPackHash"] = StrictHash('1')
                    }
                }
            ],
            Patches =
            {
                new()
                {
                    Metadata =
                    {
                        ["source.visualActionId"] = "visual-action.eeeeeeeeeeeeeeee",
                        ["source.eventId"] = "event.ffffffffffffffff",
                        ["source.simulationFrameId"] = "frame.bbbbbbbbbbbbbbbb",
                        ["source.inputPackHash"] = StrictHash('1')
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
                        ["source.visualActionId"] = "visual-action.cccccccccccccccc",
                        ["source.eventId"] = "event.dddddddddddddddd",
                        ["source.simulationFrameId"] = "frame.bbbbbbbbbbbbbbbb",
                        ["source.inputPackHash"] = StrictHash('1')
                    }
                }
            }
        };

        WebGlRunActionPlanValidationResult validation = new WebGlRunActionPlanValidator(EconomyBoundaryOptions()).Validate(plan);

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

        WebGlRunActionPlanValidationResult validation = new WebGlRunActionPlanValidator(EconomyBoundaryOptions()).Validate(plan);

        Assert.False(validation.IsValid);
        Assert.Contains(validation.Errors, error => error.Contains("domain-specific", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Action_plan_validator_rejects_domain_specific_target_metadata()
    {
        var plan = new WebGlRunActionPlan
        {
            FrameRate = 1,
            ActionId = "plan.target-metadata",
            Actions =
            [
                new()
                {
                    ActionId = "action.target-metadata",
                    ActionKind = WebGlRunActionKinds.MoveToObject,
                    ObjectId = "actor",
                    Target = new()
                    {
                        ObjectId = "target",
                        Metadata =
                        {
                            ["marketRole"] = "buyer"
                        }
                    }
                }
            ]
        };

        WebGlRunActionPlanValidationResult validation = new WebGlRunActionPlanValidator(EconomyBoundaryOptions()).Validate(plan);

        Assert.False(validation.IsValid);
        Assert.Contains(validation.Errors, error => error.Contains("target.metadata", StringComparison.OrdinalIgnoreCase));
        Assert.Contains(validation.Errors, error => error.Contains("domain-specific", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Run_document_validator_rejects_domain_specific_initial_scene_metadata()
    {
        WebGlRunDocument document = ObserverDocument();
        document.InitialScene.Scene.Objects[0].Metadata["marketRole"] = "buyer";
        document.InitialScene.Scene.Links.Add(new()
        {
            Id = "link.market-flow",
            SourceObjectId = "actor",
            TargetObjectId = "actor",
            Kind = "market"
        });

        WebGlRunDocumentValidationResult validation = new WebGlRunDocumentValidator(EconomyBoundaryOptions()).Validate(document);

        Assert.False(validation.IsValid);
        Assert.Contains(validation.Errors, error => error.Contains("initialScene.scene.objects.actor.metadata.marketRole", StringComparison.Ordinal));
        Assert.Contains(validation.Errors, error => error.Contains("initialScene.scene.links.link.market-flow", StringComparison.Ordinal));
    }

    [Fact]
    public void Domain_mapping_driver_manifest_hash_validation_and_scrubber_support_non_economy_driver()
    {
        IWebGlRunDomainMappingDriver driver = new LogisticsWebGlRunDomainMappingDriver();

        WebGlRunDomainMappingDriverValidationResult validation = driver.Validate();
        WebGlRunDomainMappingDriverManifest manifest = driver.Manifest;
        IReadOnlyDictionary<string, string> scrubbed = driver.ScrubMetadata(new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["source.eventId"] = "shipment.opaque-source-id",
            ["source.policy"] = "should-not-pass",
            ["shipment.status"] = "loaded",
            ["genericStatus"] = "ready"
        });

        Assert.True(validation.IsValid, string.Join(Environment.NewLine, validation.Errors));
        Assert.Equal("logistics-webgl-run", manifest.DriverId);
        Assert.Equal(driver.DriverHash, manifest.DriverHash);
        Assert.StartsWith("sha256:", manifest.DriverHash, StringComparison.Ordinal);
        Assert.Equal(WebGlRunActionKinds.MoveToObject, manifest.ActionKindMappings["load-pickup"]);
        Assert.Equal(WebGlRunActionKinds.DirectedFlowVisual, manifest.ActionKindMappings["route-leg"]);
        Assert.Equal("shipment.opaque-source-id", scrubbed["source.eventId"]);
        Assert.Equal("ready", scrubbed["genericStatus"]);
        Assert.DoesNotContain("source.policy", scrubbed.Keys);
        Assert.DoesNotContain("shipment.status", scrubbed.Keys);
    }

    [Fact]
    public void Observer_proof_compares_expected_and_browser_documents_without_mutating_source_truth()
    {
        WebGlRunDocument expected = ObserverDocument();
        WebGlRunDocument browserLoaded = ObserverDocument();
        string expectedHashBefore = WebGlRunObserverProof.ComputeDocumentHash(expected);

        WebGlRunObserverProofReport valid = WebGlRunObserverProof.Compare(
            expected,
            browserLoaded,
            ValidObserverSnapshot());

        Assert.Equal("webgl-run-observer-proof/v1", valid.SchemaVersion);
        Assert.True(valid.ObserverProofValid, string.Join(Environment.NewLine, valid.Errors));
        Assert.True(valid.DocumentHashesMatch);
        Assert.Equal("observer-valid", valid.ClaimStatus);
        Assert.Equal("run-playback", valid.Route);
        Assert.Equal("1920x1080", valid.Viewport);
        Assert.True(valid.RuntimeIdle);
        Assert.Equal(["stage.visual"], valid.CompletedStageIds);
        Assert.Equal(new WebGlVector3(1, 0, 0), valid.ExpectedFinalObjectPositions["actor"]);
        Assert.Equal(new WebGlVector3(1, 0, 0), valid.BrowserFinalObjectPositions["actor"]);
        Assert.Equal(expectedHashBefore, valid.ExpectedDocumentHash);
        Assert.Equal(expectedHashBefore, WebGlRunObserverProof.ComputeDocumentHash(expected));

        browserLoaded.Timeline.Frames[0].Stages[0].Motions[0].TargetPosition = new WebGlVector3(9, 0, 0);
        WebGlRunObserverProofReport mismatch = WebGlRunObserverProof.Compare(
            expected,
            browserLoaded,
            ValidObserverSnapshot(finalPosition: new WebGlVector3(9, 0, 0)));

        Assert.False(mismatch.ObserverProofValid);
        Assert.False(mismatch.DocumentHashesMatch);
        Assert.Equal("observer-failed", mismatch.ClaimStatus);
        Assert.Contains("browser-document-hash-mismatch", mismatch.Errors);
        Assert.Contains("final-object-position-mismatch:actor", mismatch.Errors);

        WebGlRunObserverProofReport runtimeFailure = WebGlRunObserverProof.Compare(
            expected,
            ObserverDocument(),
            ValidObserverSnapshot(runtimeError: "webgl context failed"));

        Assert.False(runtimeFailure.ObserverProofValid);
        Assert.True(runtimeFailure.DocumentHashesMatch);
        Assert.Equal("observer-failed", runtimeFailure.ClaimStatus);
        Assert.Contains("browser-runtime-error:webgl context failed", runtimeFailure.Errors);

        WebGlRunObserverProofReport shallowBooleanOnly = WebGlRunObserverProof.Compare(
            expected,
            ObserverDocument(),
            new()
            {
                BrowserRuntimeExercised = true,
                UiExercised = true
            });

        Assert.False(shallowBooleanOnly.ObserverProofValid);
        Assert.Contains("runtime-idle-proof-missing", shallowBooleanOnly.Errors);
        Assert.Contains("completed-stage-missing:stage.visual", shallowBooleanOnly.Errors);
        Assert.Contains("final-object-position-mismatch:actor", shallowBooleanOnly.Errors);
    }

    [Fact]
    public void Observer_proof_rejects_missing_browser_exported_positions_even_when_other_evidence_passes()
    {
        WebGlRunDocument expected = ObserverDocument();
        WebGlRunObserverSnapshot browserExportMissingPositions = ValidObserverSnapshot();
        browserExportMissingPositions.FinalObjectPositions.Clear();

        WebGlRunObserverProofReport report = WebGlRunObserverProof.Compare(
            expected,
            ObserverDocument(),
            browserExportMissingPositions);

        Assert.False(report.ObserverProofValid);
        Assert.True(report.DocumentHashesMatch);
        Assert.True(report.RuntimeIdle);
        Assert.Equal(["stage.visual"], report.CompletedStageIds);
        Assert.Empty(report.BrowserFinalObjectPositions);
        Assert.Contains("final-object-position-mismatch:actor", report.Errors);
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

    private static WebGlRunDocument ObserverDocument()
    {
        var document = new WebGlRunDocument
        {
            RunId = new("run.observer-proof"),
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
                                StageId = "stage.visual",
                                StageIndex = 0,
                                Motions =
                                {
                                    Motion("motion.observer", new WebGlVector3(1, 0, 0))
                                }
                            }
                        }
                    }
                }
            },
            Metadata =
            {
                ["boundary"] = "generic-webgl-observer"
            }
        };
        WebGlRunDriverMetadataKeys.Stamp(document.Metadata, WebGlRunPassThroughDomainMappingDriver.Instance, StrictHash('9'));
        return document;
    }

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

    private static WebGlRunObserverSnapshot ValidObserverSnapshot(
        WebGlVector3? finalPosition = null,
        string runtimeError = "")
    {
        var snapshot = new WebGlRunObserverSnapshot
        {
            BrowserRuntimeExercised = true,
            UiExercised = true,
            Route = "run-playback",
            Viewport = "1920x1080",
            ScreenshotPath = "bundle://proof/SB04/screenshot.png",
            RuntimeIdleResult = new()
            {
                Success = true,
                Idle = true,
                TimedOut = false,
                Reason = "observer-proof",
                Diagnostics = new()
            },
            CompletedStageIds = ["stage.visual"],
            FinalObjectPositions =
            {
                ["actor"] = finalPosition ?? new WebGlVector3(1, 0, 0)
            }
        };
        if (!string.IsNullOrWhiteSpace(runtimeError))
        {
            snapshot.RuntimeErrors.Add(runtimeError);
        }

        return snapshot;
    }

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

    private static string StrictHash(char value)
        => $"sha256:{new string(value, 64)}";

    private sealed class LogisticsWebGlRunDomainMappingDriver : IWebGlRunDomainMappingDriver
    {
        public string DriverId => "logistics-webgl-run";
        public string DriverVersion => "1.0.0";
        public string DisplayName => "Logistics WebGL run mapping";
        public WebGlRunGenericBoundaryOptions BoundaryOptions { get; } = new()
        {
            ForbiddenDomainTerms = ["shipment", "warehouse", "truck"]
        };

        public IReadOnlyCollection<string> DriverActionKinds { get; } =
        [
            "load-pickup",
            "route-leg",
            "delivery-wait"
        ];

        public string MapToGenericActionKind(string driverActionKind)
            => driverActionKind switch
            {
                "load-pickup" => WebGlRunActionKinds.MoveToObject,
                "route-leg" => WebGlRunActionKinds.DirectedFlowVisual,
                "delivery-wait" => WebGlRunActionKinds.Wait,
                _ => WebGlRunActionKinds.Wait
            };
    }
}
